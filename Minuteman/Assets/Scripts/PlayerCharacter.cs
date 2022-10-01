using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float maxSpeed = 150f;
    public float acceleration = 15f;
    public float speedFalloff = 2f;
    private Vector2 _moveVector = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 directionalVector = new Vector2();
        if (Input.GetKey(KeyCode.W))
        {
            directionalVector += Vector2.up * acceleration;
        } else if (Input.GetKey(KeyCode.S))
        {
            directionalVector += Vector2.down * acceleration;
        }
        

        if (Input.GetKey(KeyCode.A))
        {
            directionalVector += Vector2.left * acceleration;
        } else if (Input.GetKey(KeyCode.D))
        {
            directionalVector += Vector2.right * acceleration;
        }
        directionalVector.Normalize();


        _moveVector += directionalVector * acceleration;
        if (directionalVector.x == 0)
            _moveVector.x /= speedFalloff;

        if (directionalVector.y == 0)
            _moveVector.y /= speedFalloff;

        _moveVector.x = Mathf.Clamp(_moveVector.x, -maxSpeed, maxSpeed);
        _moveVector.y = Mathf.Clamp(_moveVector.y, -maxSpeed, maxSpeed);

        transform.Translate(_moveVector * Time.deltaTime);
    }
}
