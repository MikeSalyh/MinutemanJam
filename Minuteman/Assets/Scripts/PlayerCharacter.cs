using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float maxSpeed = 150f;
    public float acceleration = 15f;
    public float speedFriction = 1.1f;
    public float dodgeCooldown = 1f;
    public float dodgeAcceleration = 100f;

    private float lastDodgeTime = 0f;
    private Vector2 _inputVector = new Vector2();
    private Vector2 _moveVector = new Vector2();
    private Vector2 _dodgeVector = new Vector2();

    // Start is called before the first frame update
    void Start()
    {

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

    private void HandleMovementInput()
    {
        //Move, accelerate + decelerate the character
        _inputVector = new Vector2();
        if (Time.time < lastDodgeTime + dodgeCooldown)
            return;

        if (Input.GetKey(KeyCode.W))
        {
            _inputVector += Vector2.up * acceleration;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _inputVector += Vector2.down * acceleration;
        }


        if (Input.GetKey(KeyCode.A))
        {
            _inputVector += Vector2.left * acceleration;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _inputVector += Vector2.right * acceleration;
        }
        _inputVector.Normalize();

    }

    private void HandleMovementPhysics()
    {
        _moveVector += _inputVector * acceleration;
        if (_inputVector.x == 0)
            _moveVector.x /= speedFriction;

        if (_inputVector.y == 0)
            _moveVector.y /= speedFriction;

        _moveVector.x = Mathf.Clamp(_moveVector.x, -maxSpeed, maxSpeed);
        _moveVector.y = Mathf.Clamp(_moveVector.y, -maxSpeed, maxSpeed);

        transform.Translate(_moveVector);


        //Dodging
        if (Input.GetMouseButton(1) && _moveVector.magnitude > 1f)
        {
            if (Time.time > lastDodgeTime + dodgeCooldown)
            {
                lastDodgeTime = Time.time;
                _dodgeVector = _moveVector.normalized * dodgeAcceleration;
                Debug.Log("Dodging!");
            }
        }

        transform.Translate(_dodgeVector);
        _dodgeVector /= speedFriction;
    }
}
