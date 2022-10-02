using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
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
    public Musketevent OnFire, OnReloadComplete;

    public GameObject smokePrefab;
    private WaitForSeconds DetectorDelay = new WaitForSeconds(1f);

    private Collider c;
    private Vector2 lastPosition;
    private float minimumMoveThresholdToRecalculate = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        c = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb2 = GetComponent<Rigidbody2D>();
        if (rb2 != null) is2D = true;
        bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
    }

    private void Start()
    {
        StartCoroutine(DetectDangerZoneCoroutine());
    }

    private IEnumerator DetectDangerZoneCoroutine()
    {
        while (true)
        {
            if(Vector3.Distance(transform.position, lastPosition) > minimumMoveThresholdToRecalculate)
            {
                TargetingGrid.Instance.CalculateGrid(c);
                lastPosition = transform.position;
            }
            yield return DetectorDelay;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleShooting();
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
}
