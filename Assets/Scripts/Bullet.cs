using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;

    private Vector2 m_Direction;

    void Start()
    {
        // save direction by offsetting the target position and the initial object's position.
        m_Direction = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y) + m_Direction, speed * Time.deltaTime);
    }
}
