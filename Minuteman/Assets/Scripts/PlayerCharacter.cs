using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float inputForce = 9000f;
    public float dodgeCooldown = 1f;
    public float dodgeAcceleration = 100f;

    private float lastDodgeTime = 0f;
    private Vector2 _inputVector = new Vector2();
    private Vector2 _dodgeVector = new Vector2();
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        HandleMovementPhysics();
    }

    public bool IsDodging => Time.time < lastDodgeTime + dodgeCooldown;

    private void HandleMovementInput()
    {
        //Move, accelerate + decelerate the character
        _inputVector = new Vector2();
        if (Time.time < lastDodgeTime + dodgeCooldown)
            return;

        if (Input.GetKey(KeyCode.W))
        {
            _inputVector += Vector2.up * inputForce;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _inputVector += Vector2.down * inputForce;
        }


        if (Input.GetKey(KeyCode.A))
        {
            _inputVector += Vector2.left * inputForce;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _inputVector += Vector2.right * inputForce;
        }
        _inputVector.Normalize();

    }

    private void HandleMovementPhysics()
    {
        rb.AddForce(_inputVector * inputForce);

        //Dodging
        if (Input.GetMouseButton(1) && _inputVector.magnitude >= 0.1f)
        {
            if (Time.time > lastDodgeTime + dodgeCooldown)
            {
                lastDodgeTime = Time.time;
                _dodgeVector = _inputVector * dodgeAcceleration;
                Debug.Log("Dodging!");
                rb.AddForce(_dodgeVector, ForceMode2D.Impulse);
            }
        }
    }
}
