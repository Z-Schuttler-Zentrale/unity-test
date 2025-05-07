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
        t_lastspread += Time.deltaTime;

        if (t_lastspread > 1f && schusszahl > 0)
        {
            schusszahl = 0;
        }

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

    private bool PlayerIsMoving()
    {
        return Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
    }
    private float t_lastspread;
    private float schusszahl;

    private float CalculateSpread()
    {
        return 2f / Mathf.Max(1f + t_lastspread, 3.5f - schusszahl * 0.2f);
    }
    private Vector3 GenerateGaussianRecoil(float spread)
    
    {
        float u1 = Random.value;
        float u2 = Random.value;
        float u3 = Random.value;

        float z0 = Mathf.Sqrt(-Mathf.Log(u1)) * Mathf.Cos(Mathf.PI * u2);
        float z1 = Mathf.Sqrt(-Mathf.Log(u1)) * Mathf.Sin(Mathf.PI * u2);
        float z2 = Mathf.Sqrt(-Mathf.Log(u3)) * Mathf.Sin(Mathf.PI * u3);

        return new Vector3(z0 * spread, z1 * spread, z2 * spread);
    }

    private Vector3 RecoilDirection(float spread)
    {
        Vector3 baseDir = spawnPunkt.transform.forward;
        Vector3 recoil = GenerateGaussianRecoil(spread);

        // Recoil leicht in der lokalen Waffe-Richtung versetzt
        Vector3 finalDir = Quaternion.Euler(recoil) * baseDir;
        return finalDir.normalized;
    }
    private void FireBullet()
    {
        float spread = CalculateSpread();
        Vector3 direction = RecoilDirection(spread);
        BulletMovement bm = BulletPool.Instance.GetBullet();
        bm.Init(spawnPunkt.transform.position, -direction);
        t_lastspread = 0;
        schusszahl = Mathf.Min(schusszahl + 1, 10); // Maximalwert als Puffer
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
