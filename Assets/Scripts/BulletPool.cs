using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;
    public BulletMovement bulletPrefab;
    public int poolSize = 60;

    private Queue<BulletMovement> pool = new Queue<BulletMovement>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            BulletMovement bullet = Instantiate(bulletPrefab);
            bullet.gameObject.SetActive(false);
            pool.Enqueue(bullet);
        }
    }

    public BulletMovement GetBullet()
    {
        if (pool.Count > 0)
        {
            var bullet = pool.Dequeue();
            bullet.gameObject.SetActive(true);
            return bullet;
        }
        else
        {
            var bullet = Instantiate(bulletPrefab);
            return bullet;
        }
    }

    public void ReturnBullet(BulletMovement bullet)
    {
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
    }
}