using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletMovement : MonoBehaviour
{
    public float speed = 800f;
    public float mass = 0.005f;
    public float dragArea = 0.0005f;
    public float windAngle = 0f;
    public float windSpeed = 0f;

    public LineRenderer lr;
    private float timePassed = 0f;
    private float flightTime = 1f;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
    }

    public void Init(Vector3 start, Vector3 direction)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 velocity = direction.normalized * speed;

        Vector3 position = start;

        float g = 9.81f;
        float dt = 0.05f;
        float t = 0f;
        Vector3 wind = windSpeed * new Vector3(Mathf.Cos(windAngle * Mathf.Deg2Rad), 0, Mathf.Sin(windAngle * Mathf.Deg2Rad));

        points.Add(position);

        while (t < 30f)
        {
            Vector3 a = new Vector3(0, -g, 0) + ComputeDrag(velocity - wind);
            Vector3 k1v = a * dt;
            Vector3 k1p = velocity * dt;

            Vector3 v2 = velocity + k1v * 0.5f;
            Vector3 a2 = new Vector3(0, -g, 0) + ComputeDrag(v2 - wind);
            Vector3 k2v = a2 * dt;
            Vector3 k2p = v2 * dt;

            Vector3 v3 = velocity + k2v * 0.5f;
            Vector3 a3 = new Vector3(0, -g, 0) + ComputeDrag(v3 - wind);
            Vector3 k3v = a3 * dt;
            Vector3 k3p = v3 * dt;

            Vector3 v4 = velocity + k3v;
            Vector3 a4 = new Vector3(0, -g, 0) + ComputeDrag(v4 - wind);
            Vector3 k4v = a4 * dt;
            Vector3 k4p = v4 * dt;

            velocity += (1f / 6f) * (k1v + 2f * k2v + 2f * k3v + k4v);
            position += (1f / 6f) * (k1p + 2f * k2p + 2f * k3p + k4p);

            points.Add(position);

            if (position.y < 0f)
            {
                flightTime = t;
                Debug.Log(t);
                break;
            }

            t += dt;
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        flightTime = t; // Store for movement
    }

    Vector3 ComputeDrag(Vector3 vRel)
    {
        float cw = 0.4f;
        float rho = 1.225f;
        float drag = 0.5f * rho * cw * dragArea * vRel.sqrMagnitude / mass;
        return -drag * vRel.normalized;
    }

    void Update()
    {
        if (lr == null || lr.positionCount < 2) return;

        timePassed += Time.deltaTime;
        float progress = timePassed / flightTime;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        float point = progress * (lr.positionCount - 1);
        int index = Mathf.FloorToInt(point);
        float lerpT = point - index;

        Vector3 p1 = lr.GetPosition(index);
        Vector3 p2 = lr.GetPosition(index + 1);
        transform.position = Vector3.Lerp(p1, p2, lerpT);
        transform.forward = (p2 - p1).normalized;
    }
}
