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
    private float timePassed;
    private float flightTime = 1f;
    private float impactSpeed;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
    }

    public void Init(Vector3 start, Vector3 direction)
    {
        timePassed = 0f;
        List<Vector3> points = new List<Vector3>();
        Vector3 velocity = direction.normalized * speed;

        Vector3 position = start;

        float g = 9.81f;
        float dt = 0.2f;
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

                if (flightTime < 0.2f)
                {
                    flightTime = 0.2f;
                    points.Clear();
                    for (int i = 0; i <= 10; i++)
                    {
                        points.Add(start + direction.normalized * speed * dt * i / 100);
                    }
                }
                break;
            }


            t += dt;
            dt += 0.01f;
        }
        List<Vector3> interpolated = new List<Vector3>();

        if (points.Count >= 2)
        {
            // Spiegelung am Anfang
            Vector3 preStart = points[0] + (points[0] - points[1]);

            // Spiegelung am Ende
            Vector3 postEnd = points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]);

            // Neue Punktliste mit Padding
            List<Vector3> padded = new List<Vector3>();
            padded.Add(preStart);
            padded.AddRange(points);
            padded.Add(postEnd);

            // Interpolation
            for (int i = 1; i < padded.Count - 2; i++)
            {
                for (float ti = 0f; ti < 1f; ti += 0.2f)
                {
                    Vector3 p = CatmullRom(padded[i - 1], padded[i], padded[i + 1], padded[i + 2], ti);
                    interpolated.Add(p);
                }
            }

            // Optional: letzten echten Punkt hinzuf�gen
            interpolated.Add(points[points.Count - 1]);
        }
        else
        {
            interpolated.AddRange(points); // Fallback
        }
        lr.positionCount = interpolated.Count;
        lr.SetPositions(interpolated.ToArray());
    }
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    Vector3 ComputeDrag(Vector3 vRel)
    {
        float cw = 0.4f;
        float rho = 1.225f;
        float drag = 0.5f * rho * cw * dragArea * vRel.sqrMagnitude / mass;
        return -drag * vRel.normalized;
    }

    void DeleteBullet()
    {
        BulletPool.Instance.ReturnBullet(this);
    }

    void Update()
    {
        if (lr == null || lr.positionCount < 2) return;

        timePassed += Time.deltaTime;
        float progress = timePassed / flightTime;

        if (progress >= 1f)
        {
            DeleteBullet(); 
            return;
        }

        float point = progress * (lr.positionCount - 1);
        int index = Mathf.FloorToInt(point);
        float lerpT = point - index;

        Vector3 p1 = lr.GetPosition(index);
        Vector3 p2 = lr.GetPosition(index + 1);
        
        float segmentLength = Vector3.Distance(p1, p2);
        
        Vector3 origin = Vector3.Lerp(p1, p2, lerpT);
        Vector3 direction = (p2 - p1).normalized;
        
        transform.position = origin;
        transform.forward = direction;
        
        Debug.DrawRay(origin, direction * segmentLength, Color.green, 1f);
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, segmentLength))
        {
            float segmentTime = flightTime / (lr.positionCount - 1);
            float currentSpeed = segmentLength / segmentTime;
            float damage = Mathf.Min(100, (currentSpeed / speed) * 1 * 100);
            
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            Debug.DrawRay(origin, direction * segmentLength, Color.red, 1f);
            DeleteBullet();

            // Debug.Log($"Ray Hit: {hit.collider.name}"); 
            // Debug.Log($"Damage {damage}"); 
            // Debug.Log($"Current Speed at Impact: {currentSpeed} m/s");
            
            if (damageable == null)
            {
                return;
            }

            damageable.Damage(damage);
        }
    }
}
