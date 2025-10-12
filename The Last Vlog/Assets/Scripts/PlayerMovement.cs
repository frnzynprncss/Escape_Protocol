using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float horizontal;
    private float vertical;
    public float Speed = 5f;
    private Vector3 Movement;
    // Start is called before the first frame update
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        horizontal =Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Movement = new Vector3(horizontal, vertical, 0).normalized;
        rb.velocity = new Vector3(Movement.x * Speed, Movement.y * Speed, Movement.z * Speed);
    }
}
