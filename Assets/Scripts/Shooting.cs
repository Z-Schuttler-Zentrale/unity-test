using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject niger;
    public GameObject wuerfel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shot();
    }
    void Shot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(niger, wuerfel.transform.position, wuerfel.transform.rotation);
            transform.rotation = lookRotation * Quaternion.Euler(-90f, 0f, 0f); //so eine kacke
        }
    }

}
