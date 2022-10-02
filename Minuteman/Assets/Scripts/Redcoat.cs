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
    private float shootCooldown = 10f;

    public bool IsReadyToShoot => shootCooldownRemaining <= 0f;

    public GameObject bulletPrefab;
    private GameObject bulletParent;
    public GameObject smokePrefab;

    public float recoilForce = 1000f;
    private Rigidbody rb;
    public Renderer rend;
    private float targetingDistance = 1f;
    private bool isCommando = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
        PlayerCharacter.Instance.OnAssessDanger += HandleDanger;
        yield return new WaitForSeconds(3f);
        TakeFiringPosition();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (initialized)
        {
            if (agent.remainingDistance < targetingDistance && IsReadyToShoot && !midFiringAtPlayer && currentTile.isTargeted && rend.isVisible)
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
    }

    private void HandleDanger()
    {
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
        Debug.Log("Firing!");
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
    }

    public void TakeCover()
    {
        TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, true);
        if (output != null)
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
}
