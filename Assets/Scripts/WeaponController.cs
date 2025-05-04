using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public GameObject patrone;
    public GameObject spawnPunkt;
    public FiringMode mode = FiringMode.SINGLE;
    public int burstAmount;
    public float cadence = 700f;

    public enum FiringMode
    {
        SINGLE,
        BURST,
        AUTOMATIC,
        SAFETY
    }

    private float _timePassed;
    private bool _isBursting;
    private float _fireRate;
    private Text fireRateDisplayText;

    private void Start()
    {
        GameObject obj = GameObject.Find("Canvas");
        Canvas canvas = obj.GetComponent<Canvas>();
        if (canvas != null)
        {
            fireRateDisplayText = canvas.GetComponentInChildren<Text>();
        }
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        _fireRate = 60f / cadence;
        fireRateDisplayText.text = mode.ToString();
        
        switch (mode)
        {
            case FiringMode.AUTOMATIC:
            {
                if (_timePassed >= _fireRate && Input.GetButton("Fire1"))
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

            case FiringMode.SAFETY:
            {
                break;
            } 
        }
        SwitchFireMode();
    }

    private void SetProjectileAngels()
    {
        float xy = transform.rotation.eulerAngles.x;
        float xz = -transform.rotation.eulerAngles.y - 90;
        ProjectileSimulation.angXY = xy;
        ProjectileSimulation.angXZ = xz;
        
        ProjectileSimulation.startPoint = spawnPunkt.transform.position;
    }

    private void FireBullet()
    {
        SetProjectileAngels();
        Instantiate(patrone, spawnPunkt.transform.position, spawnPunkt.transform.rotation);
    }

    private void SwitchFireMode()
    {
        if (!Input.GetKeyDown(KeyCode.V))
        {
            return;
        }
        
        if ((int) mode >= 3)
        {
            mode = 0;
        }
        else
        {
            mode++;
        }
    }

    private IEnumerator BurstFire()
    {
        _isBursting = true;
        for (int i = 0; i < burstAmount; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(_fireRate);
        }
        _isBursting = false;
    }
}
