using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float v = 1f;
    public LineRenderer lr;
    public float dt = 0.01f;
    float timePassed = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        lr = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    void Movement()
    {
        float t = ProjectileSimulation.s;
        float totalPoints = lr.positionCount - 1;

        if (lr == null || lr.positionCount < 2) return;

        timePassed += Time.deltaTime;
        float progress = timePassed / t;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        totalPoints = lr.positionCount - 1;
        float punkt = progress * totalPoints;
        int index = Mathf.FloorToInt(punkt);
        float lerpT = punkt - index;

        if (index < totalPoints)
        {
            Vector3 p1 = lr.GetPosition(index);
            Vector3 p2 = lr.GetPosition(index + 1);
            transform.position = Vector3.Lerp(p1, p2, lerpT);
        }
    }
}
