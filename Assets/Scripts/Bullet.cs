using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;

    private Vector2 m_Direction;
    [SerializeField] private ParticleSystem hitParticleSystem;
    public float bulletDamage = 1;
    public GunType gunType;

    void Start()
    {
        // Get random offset 
        float xOffset = Random.Range(-.1f, .1f);

        // save direction by offsetting the target position and the initial object's position.
        m_Direction = Vector2.up;
        m_Direction.x += xOffset;

        StartCoroutine(DestroySelf());
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y) + m_Direction, speed * Time.deltaTime), Quaternion.Euler(m_Direction.x, m_Direction.y, 0));
    }

    void OnDestroy()
    {
        hitParticleSystem.transform.parent = null;
        hitParticleSystem.Play();
        Destroy(hitParticleSystem, 2);
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this);
    }
}
