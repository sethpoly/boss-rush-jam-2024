using UnityEngine;

public class Gun: MonoBehaviour {
    public GameObject bullet;
    private bool canShoot = false;
    private float lastShotTime;
    private float baseFireRate;
    private float fireRateBuff = 0;
    private GunType type;

    // Update is called once per frame
    void Update()
    {
        ShootTimer();
    }

    // Cooldown timer for bullet fire rate
    private void ShootTimer()
    {
        if(Time.time > lastShotTime)
        {
            lastShotTime = Time.time + baseFireRate - fireRateBuff;
            canShoot = true;
        } 
        else 
        {
            canShoot = false;
        }
    }

    public void Setup(GameObject bulletPrefab, float fireRate, GunType gunType)
    {
        bullet = bulletPrefab;
        baseFireRate = fireRate;
        this.type = gunType;
    }

    public void Shoot(float fireRateBuff)
    {
        this.fireRateBuff = fireRateBuff;
        if (canShoot && bullet != null)
        {
            Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, 0f), transform.rotation);
            Debug.Log("Shooting with " + type + "with firerate buff: " + fireRateBuff);
        }
    }
}