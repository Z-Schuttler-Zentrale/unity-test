using UnityEngine;
using System.Collections.Generic;

public class ProjectileSimulation : MonoBehaviour
{
    public float v = 900f;             // Startgeschwindigkeit (m/s)
    public static float angXY;         // Vertikaler Winkel
    public static float angXZ;          // Horizontaler Winkel
    public float m = 1f;              // Masse (kg)
    public float ag = 0.05f;          // Querschnittsfl�che (m�)
    public float degWind = 0f;        // Windrichtung (Grad)
    public float vw = 0f;             // Windgeschwindigkeit (m/s)
    public float h = 0f;              // Starth�he (m)
    public LineRenderer lineRenderer;
    public static float s;

    public static Vector3 startPoint;

    void Start()
    {
        SimuliereWurfRK4();
    }

    void SimuliereWurfRK4()
    {
        List<Vector3> points = new List<Vector3>();

        float windRad = Mathf.Deg2Rad * degWind;
        float angRad = Mathf.Deg2Rad * angXZ;
        float angleRad = Mathf.Deg2Rad * angXY;

        float vxz = v * Mathf.Cos(angleRad);
        float vy = v * Mathf.Sin(angleRad);
        float vx = vxz * Mathf.Cos(angRad);
        float vz = vxz * Mathf.Sin(angRad);

        Vector3 velocity = new Vector3(vx, vy, vz);
        Vector3 position = startPoint;

        Vector3 wind = vw * new Vector3(Mathf.Cos(windRad), 0f, Mathf.Sin(windRad));

        float g = 9.81f;
        float dt = 0.01f;
        float t = 0f;

        points.Add(position);

        while (t < 30f)
        {
            Vector3 aGravity = new Vector3(0f, -g, 0f);
            Vector3 vRel = velocity - wind;
            Vector3 aDrag = Luftwiderstand(vRel, ag, m);
            Vector3 a = aGravity + aDrag;

            // RK4 Schritte f�r Position und Geschwindigkeit
            Vector3 k1v = a * dt;
            Vector3 k1p = velocity * dt;

            Vector3 v2 = velocity + k1v * 0.5f;
            Vector3 p2 = position + k1p * 0.5f;
            Vector3 a2 = new Vector3(0f, -g, 0f) + Luftwiderstand(v2 - wind, ag, m);
            Vector3 k2v = a2 * dt;
            Vector3 k2p = v2 * dt;

            Vector3 v3 = velocity + k2v * 0.5f;
            Vector3 p3 = position + k2p * 0.5f;
            Vector3 a3 = new Vector3(0f, -g, 0f) + Luftwiderstand(v3 - wind, ag, m);
            Vector3 k3v = a3 * dt;
            Vector3 k3p = v3 * dt;

            Vector3 v4 = velocity + k3v;
            Vector3 p4 = position + k3p;
            Vector3 a4 = new Vector3(0f, -g, 0f) + Luftwiderstand(v4 - wind, ag, m);
            Vector3 k4v = a4 * dt;
            Vector3 k4p = v4 * dt;

            // Kombiniere alle Schritte
            velocity += (1f / 6f) * (k1v + 2f * k2v + 2f * k3v + k4v);
            position += (1f / 6f) * (k1p + 2f * k2p + 2f * k3p + k4p);

            points.Add(position);

            if (position.y < 0)
            {
                float range = new Vector2(position.x, position.z).magnitude;
                // Debug.Log($"Reichweite: {range:F2} m");
                // Debug.Log($"Flugzeit: {t:F2} s");
                s = t;
                break;
            }

            t += dt;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    Vector3 Luftwiderstand(Vector3 vRel, float flaeche, float masse)
    {
        float cw = 0.3f; // f�r spitzes Projektil
        float luftdichte = 1.225f;
        float f = 0.5f * luftdichte * cw * flaeche * vRel.sqrMagnitude / masse;
        return -f * vRel.normalized;
    }
}

