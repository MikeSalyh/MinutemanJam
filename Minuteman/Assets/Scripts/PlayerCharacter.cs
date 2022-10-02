using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public float inputForce = 9000f;
    public float dodgeCooldown = 1f;
    public float dodgeAcceleration = 100f;

    private float lastDodgeTime = 0f;
    private Vector3 _inputVector = new Vector2();
    private Vector3 _dodgeVector = new Vector2();
    private Rigidbody rb;
    private Rigidbody2D rb2;
    private bool is2D = false;

    public float recoilForce = 1000f;

    public GameObject bulletPrefab;
    private GameObject bulletParent;
    public float recoilShakeDuration = 0.5f, recoilShakePower = 800f;
    public int recoilShakeVibrato = 5;


    public float reloadCooldownTime = 10f;
    private float reloadCooldownRemaining = 0f;

    public bool CanShoot => reloadCooldownRemaining <= 0f;

    public float ReloadTimeNormalized => reloadCooldownRemaining / reloadCooldownTime;

    public delegate void Musketevent();
    public Musketevent OnFire, OnReloadComplete, OnAssessDanger;

    public GameObject smokePrefab;
    private WaitForSeconds DetectorDelay = new WaitForSeconds(1f);

    private Collider c;
    private Vector2 lastPosition;
    private float minimumMoveThresholdToRecalculate = 1f;

    public TargetingTile currentTile;

    public bool alive = true;
    public GameObject deathParticles;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        c = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb2 = GetComponent<Rigidbody2D>();
        if (rb2 != null) is2D = true;
    }

    private void Start()
    {
        bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
    }

    private void OnEnable()
    {
        alive = true;
        StartCoroutine(DetectDangerZoneCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DetectDangerZoneCoroutine()
    {
        yield return DetectorDelay;
        while (gameObject.activeSelf)
        {
            if(Vector3.Distance(transform.position, lastPosition) > minimumMoveThresholdToRecalculate)
            {
                TargetingGrid.Instance.CalculateGrid(c);
                lastPosition = transform.position;
                if (OnAssessDanger != null)
                    OnAssessDanger.Invoke();
            }
            yield return DetectorDelay;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleShooting();
        UpdateSprite();
    }

    private void HandleShooting()
    {
        if(reloadCooldownRemaining > 0f)
        {
            reloadCooldownRemaining -= Time.deltaTime;
            if(reloadCooldownRemaining <= 0f)
            {
                if(OnReloadComplete != null)
                    OnReloadComplete.Invoke();
            }
        }

        if (Input.GetMouseButtonDown(0) && CanShoot)
        {
            //Shoot the musket!
            ShootMusket();
        }
    }

    private void ShootMusket()
    {
        Bullet b = GameObject.Instantiate(bulletPrefab, bulletParent.transform).GetComponent<Bullet>();
        Vector3 shotDirection = Reticle.Instance.transform.position - transform.position;
        shotDirection.y = 0f;
        shotDirection.Normalize();

        b.transform.position = this.transform.position + shotDirection;
        b.Shoot(shotDirection);

        if (is2D)
        {
            rb2.AddForce(-shotDirection * recoilForce);
        }
        else
        {
            rb.AddForce(-shotDirection * recoilForce);
        }
        CameraFollower.Instance.HandleRecoil(-shotDirection);
        CameraFollower.Instance.DoShake(recoilShakeDuration, recoilShakePower, recoilShakeVibrato);

        reloadCooldownRemaining = reloadCooldownTime;

        if(OnFire != null)
        {
            OnFire.Invoke();
        }
    }

    private void FixedUpdate()
    {
        HandleMovementPhysics();
    }

    public bool IsDodging => Time.time < lastDodgeTime + dodgeCooldown;

    private void HandleMovementInput()
    {
        //Move, accelerate + decelerate the character
        _inputVector = new Vector3();
        if (Time.time < lastDodgeTime + dodgeCooldown)
            return;

        if (Input.GetKey(KeyCode.W))
        {
            _inputVector += Vector3.back * inputForce;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _inputVector += Vector3.forward * inputForce;
        }


        if (Input.GetKey(KeyCode.A))
        {
            _inputVector += Vector3.left * inputForce;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _inputVector += Vector3.right * inputForce;
        }
        _inputVector.Normalize();

    }

    private void HandleMovementPhysics()
    {
        if (is2D)
        {
            rb2.AddForce(_inputVector * inputForce);
        }
        else
        {
            rb.AddForce(_inputVector * inputForce);
        }

        //Dodging
        if (Input.GetMouseButton(1) && _inputVector.magnitude >= 0.1f)
        {
            if (Time.time > lastDodgeTime + dodgeCooldown)
            {
                lastDodgeTime = Time.time;
                _dodgeVector = _inputVector * dodgeAcceleration;

                if (is2D)
                {
                    rb2.AddForce(_dodgeVector, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(_dodgeVector, ForceMode.Impulse);
                }

                GameObject smoke = GameObject.Instantiate(smokePrefab, GameObject.FindGameObjectWithTag("ParticleParent").transform);
                smoke.transform.position = this.transform.position;
                smoke.transform.LookAt(transform.position - (Vector3)_dodgeVector, Vector2.up);
                Destroy(smoke, 1f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Tile")
        {
            currentTile = other.GetComponent<TargetingTile>();
        }
    }

    public void Die()
    {
        CameraFollower.Instance.DoShake(recoilShakeDuration * 2, recoilShakePower * 2, recoilShakeVibrato);
        StopAllCoroutines();
        gameObject.SetActive(false);
        GameObject deathpart = GameObject.Instantiate(deathParticles, GameObject.FindGameObjectWithTag("ParticleParent").transform);
        deathpart.transform.position = this.transform.position;
        Destroy(deathpart, 1f);
        alive = false;
    }

    [Header("Sprites")]
    public GameObject bodyTransform;
    public GameObject armsTransform, legsTransform;
    public SpriteRenderer legs, arms;
    private Vector3 left = new Vector3(-1f, 1f, 1f);
    private Vector3 right = new Vector3(1f, 1f, 1f);
    public Sprite stillLegs, movingLegs, regularArms, recoilArms;

    private void UpdateSprite()
    {
        bodyTransform.transform.localScale = (Reticle.Instance.transform.position.x > transform.position.x) ? left : right;
        if (IsDodging)
        {
            legs.sprite = movingLegs;
        }
        else if(rb.velocity.magnitude > 0.5f)
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
        arms.sprite = ReloadTimeNormalized > 0.95f ? recoilArms : regularArms;

        armsTransform.transform.LookAt(Reticle.Instance.transform);
    }
}
