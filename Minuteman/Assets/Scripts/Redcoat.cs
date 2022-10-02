using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Redcoat : MonoBehaviour
{
    public TargetingTile currentTile;
    private NavMeshAgent agent;
    public WaitForSeconds postShotDisorientation = new WaitForSeconds(1f);

    private bool initialized = false;
    private bool midFiringAtPlayer = false;

    public float shootCooldownRemaining = 0f;
    public float shootCooldown = 10f;

    public bool IsReadyToShoot => shootCooldownRemaining <= 0f;

    public GameObject bulletPrefab;
    private GameObject bulletParent;
    public GameObject smokePrefab;

    public float recoilForce = 1000f;
    private Rigidbody rb;
    public Renderer rend;
    private float targetingDistance = 1f;
    private bool isCommando = false;

    public GameObject deathParticles;
    public AudioClip[] deathSounds;
    public AudioClip deathExplosionSound;
    public int hp = 1;
    //public float startDelay = 3f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
    }

    private void OnEnable()
    {
        StartCoroutine(DoDelayedInit());
    }

    private IEnumerator DoDelayedInit()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => GameStartManager.Instance.gameOn);
        PlayerCharacter.Instance.OnAssessDanger += HandleDanger;
        TakeFiringPosition();
        initialized = true;
    }

    private void OnDisable()
    {
        PlayerCharacter.Instance.OnAssessDanger -= HandleDanger;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStartManager.Instance.gameOn) return;
        if (initialized)
        {
            if (agent.remainingDistance < targetingDistance && IsReadyToShoot && !midFiringAtPlayer && currentTile.isTargeted && rend.isVisible && PlayerCharacter.Instance.alive)
            {
                StartCoroutine(FireAtPlayerCoroutine());
            }
        }

        if(shootCooldownRemaining > 0f)
        {
            shootCooldownRemaining -= Time.deltaTime;
            if (shootCooldownRemaining <= 0f)
            {
                //Reloading is complete!
                TakeFiringPosition();
            }
        }

        UpdateSprite();
    }

    private void HandleDanger()
    {
        if (!GameStartManager.Instance.gameOn) return;
        if (!IsReadyToShoot)
        {
            TakeCover();
        } else if(isCommando)
        {
            TakeCommandoFiringPosition();
        }
    }

    private IEnumerator FireAtPlayerCoroutine()
    {
        midFiringAtPlayer = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(UnityEngine.Random.value + 0.25f);
        ShootMusket();
        yield return postShotDisorientation;
        midFiringAtPlayer = false;
        TakeCover();
    }


    private void ShootMusket()
    {
        Bullet b = GameObject.Instantiate(bulletPrefab, bulletParent.transform).GetComponent<Bullet>();
        Vector3 shotDirection = PlayerCharacter.Instance.transform.position - transform.position;
        shotDirection.y = 0f;
        shotDirection.Normalize();

        b.transform.position = this.transform.position + shotDirection;
        b.Shoot(shotDirection);

        rb.AddForce(-shotDirection * recoilForce);
        //CameraFollower.Instance.HandleRecoil(-shotDirection);
        CameraFollower.Instance.DoShake(0.5f, 1f, 4);

        shootCooldownRemaining = shootCooldown;

        arms.gameObject.SetActive(false);
        reloadingArms.gameObject.SetActive(true);
        reloadingArms.Play("Reload");
    }

    public void TakeCover()
    {
        TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, true);
        if (output != null && gameObject.activeSelf)
            agent.SetDestination(output.transform.position);

        agent.isStopped = false;
    }

    public void TakeFiringPosition()
    {
        if(Random.value > 0.25f)
        {
            TakeCommandoFiringPosition();
        } else
        {
            TakeSafeFiringPosition();
        }
    }

    public void TakeSafeFiringPosition()
    {
        TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, false);
        if (output != null)
            agent.SetDestination(output.transform.position);

        agent.isStopped = false;
        targetingDistance = 1f;
        isCommando = false;
    }

    public void TakeCommandoFiringPosition()
    {
        targetingDistance = 3f;
        agent.SetDestination(PlayerCharacter.Instance.transform.position);
        agent.isStopped = false;
        isCommando = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            currentTile = other.GetComponent<TargetingTile>();
            if (currentTile.isTargeted && IsReadyToShoot) {
                agent.SetDestination(currentTile.transform.position);
                //If the agent gets a shot, it's good!
            }
        }
    }

    public void GetHit()
    {
        GameObject deathpart = GameObject.Instantiate(deathParticles, GameObject.FindGameObjectWithTag("ParticleParent").transform);
        deathpart.transform.position = this.transform.position;
        Destroy(deathpart, 1f);

        hp--;
        if(hp <= 0)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            AudioManager.Instance.PlaySound(deathSounds);
            AudioManager.Instance.PlayWobblePitch(deathExplosionSound, 0.1f);
        }
        
    }

    [Header("Sprites")]
    public GameObject bodyTransform;
    public GameObject armsTransform, legsTransform;
    public SpriteRenderer legs, arms;
    public Animator reloadingArms;
    private Vector3 left = new Vector3(-1f, 1f, 1f);
    private Vector3 right = new Vector3(1f, 1f, 1f);
    public Sprite stillLegs, movingLegs;

    private void UpdateSprite()
    {
        bodyTransform.transform.localScale = (PlayerCharacter.Instance.transform.position.x > transform.position.x) ? left : right;
        {
            legs.sprite = movingLegs;
        }
        
        if (agent.velocity.magnitude > 0.5f)
        {
            Sprite legSprite = Time.time % 0.5f < 0.25f ? movingLegs : stillLegs;
            if (legs.sprite != legSprite)
            {
                //A step was taken; SFX here.
                legs.sprite = legSprite;
            }
            legsTransform.transform.localScale = rb.velocity.x > 0 ? left : right;
        }
        else
        {
            legs.sprite = stillLegs;
        }
        float yPos = legs.sprite == movingLegs ? 0.2f : 0f;
        bodyTransform.transform.localPosition = new Vector3(0f, yPos, 0f);

        if (IsReadyToShoot && reloadingArms.gameObject.activeSelf)
        {
            reloadingArms.gameObject.SetActive(false);
            arms.gameObject.SetActive(true);

        }
        armsTransform.transform.LookAt(PlayerCharacter.Instance.transform);
    }
}
