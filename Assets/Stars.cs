using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    public GameObject stars1, stars2;
    public float speed = 2.0f;
    public Color color;
    public SpriteRenderer sr;
    public SpriteRenderer sr2;


    // Start is called before the first frame update
    void Start()
    {
        StartPosition();
        sr.color = color;
        sr2.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        StarMovement();
    }

    private void StartPosition()
    {
        stars1.transform.position = new Vector3(0, 0, 0);
        stars2.transform.position = new Vector3(0, 10f, 0);
    }

    private void StarMovement()
    {
        stars1.transform.Translate(Vector3.down * speed * Time.deltaTime);
        if(stars1.transform.position.y <  -11)
        {
            stars1.transform.position = new Vector3(0, 10f, 0);
        }
        stars2.transform.Translate(Vector3.down * speed * Time.deltaTime);
        if(stars2.transform.position.y <  -11)
        {
            stars2.transform.position = new Vector3(0, 10f, 0);
        }

    }
}
