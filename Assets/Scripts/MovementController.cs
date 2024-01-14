using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;

    [Space]
    [Header("Stats")]
    public float speed = 4;

    [Space]
    [Header("Booleans")]
    public bool canMove;


    [Space]
    [Header("Playtest settings")]
    public bool godMode;

    public int side = 1;

    private float horizontal;
    private float vertical;

    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        godMode = Input.GetKey(KeyCode.G);
    }
}
