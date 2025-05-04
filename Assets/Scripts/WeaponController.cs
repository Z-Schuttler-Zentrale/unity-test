using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject patrone;
    public GameObject spawnPunkt;
    public FiringMode mode = FiringMode.SINGLE;
    public int burstAmount;
    public float fireRate = 0.1f;

    public enum FiringMode
    {
        SINGLE,
        BURST,
        AUTOMATIC,
        SAFETY
    }

    private float _timePassed;
    private bool _isBursting;

    void Update()
    {
        _timePassed += Time.deltaTime;
        if (mode != FiringMode.SAFETY)
        {
            switch (mode)
            {
                case FiringMode.AUTOMATIC:
                {
                    if (_timePassed >= fireRate && Input.GetButton("Fire1"))
                    {
                        FireBullet();
                        _timePassed = 0;
                    }

                    break;
                }
                case FiringMode.SINGLE:
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        FireBullet();
                    }

                    break;
                }
                case FiringMode.BURST:
                {
                    if (Input.GetButtonDown("Fire1") && !_isBursting)
                    {
                        StartCoroutine(BurstFire());
                    }

                    break;
                }
            }
        }
    }

    void SetzeWinkel()
    {
        float xy = transform.rotation.eulerAngles.x;
        float xz = -transform.rotation.eulerAngles.y - 90;
        ProjectileSimulation.angXY = xy;
        ProjectileSimulation.angXZ = xz;
        
        ProjectileSimulation.startPoint = spawnPunkt.transform.position;
    }

    void FireBullet()
    {
        SetzeWinkel();
        Instantiate(patrone, spawnPunkt.transform.position, spawnPunkt.transform.rotation);
    }

    private IEnumerator BurstFire()
    {
        _isBursting = true;
        for (int i = 0; i < burstAmount; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(fireRate);
        }
        _isBursting = false;
    }
}
