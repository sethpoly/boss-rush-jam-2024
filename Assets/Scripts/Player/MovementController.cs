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
    public float defaultSpeed;
    public float speed = 4;

    [Space]
    [Header("Booleans")]
    public bool canMove;


    [Space]
    [Header("Playtest settings")]
    public bool godMode;

    private float horizontal;
    private float vertical;

    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        defaultSpeed = speed;
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector2(horizontal, vertical);
        move = speed * Time.fixedDeltaTime * move.normalized;
        rb.velocity = move;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        godMode = Input.GetKey(KeyCode.G);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void ResetSpeed()
    {
        this.speed = defaultSpeed;
    }
}
