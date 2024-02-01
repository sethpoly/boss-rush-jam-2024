using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public float maxHitPoints;
    public float currentHitPoints;
    public Image healthBar;
    public float damageOutput = 1f;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1f);
        }
    }

    public void TakeDamage(float hitPoints) 
    {
        currentHitPoints -= hitPoints;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
    }
}
