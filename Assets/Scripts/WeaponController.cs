using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject patrone;
    public GameObject spawnPunkt;

    // Update is called once per frame
    void Update()
    {
        // ggf in methode auslagern
        if (Input.GetButtonDown("Fire1"))
        {
            FireBullet();
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
}
