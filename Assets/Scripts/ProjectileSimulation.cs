using UnityEngine;
using System.Collections.Generic;

public class ProjectileSimulation : MonoBehaviour
{
    public float v = 20f;             // Startgeschwindigkeit (m/s)
    public float angXY = 45f;      // Wurfwinkel (vertikal)
    public float angXZ = 0f;            // Winkel in XZ-Ebene
    public float m = 1f;              // Masse (kg)
    public float ag = 0.05f;          // Querschnittsfläche (m²)
    public float degWind = 0f;        // Windrichtung (Grad)
    public float vw = 0f;             // Windgeschwindigkeit (m/s)
    public float h = 1f;              // Start-Höhe (m)
    public LineRenderer lineRenderer;
    public static float s;

    void Start()
    {
        SimuliereWurf();
    }

    void SimuliereWurf()
    {
        List<Vector3> points = new List<Vector3>();

        float windRad = Mathf.Deg2Rad * degWind;
        float angRad = Mathf.Deg2Rad * angXZ;
        float angleRad = Mathf.Deg2Rad * angXY;

        float vxz = v * Mathf.Cos(angleRad);
        float vy = v * Mathf.Sin(angleRad);
        float vx = vxz * Mathf.Cos(angRad);
        float vz = vxz * Mathf.Sin(angRad);

        float wax = Mathf.Cos(windRad);
        float waz = Mathf.Sin(windRad);

        float g = 9.81f;
        float dt = 0.01f;

        Vector3 pos = new Vector3(0f, h, 0f);
        float t = 0f;
        bool maxHeightRecorded = false;
        float maxHeight = 0f;

        while (t < 30f)
        {
            float vrelX = vx - vw * wax;
            float vrelZ = vz - vw * waz;

            float ay = Luftwiderstand(vy, ag, m) + g;
            float ax = Luftwiderstand(vrelX, ag, m);
            float az = Luftwiderstand(vrelZ, ag, m);

            vx -= ax * dt;
            vy -= ay * dt;
            vz -= az * dt;

            pos += new Vector3(vx * dt, vy * dt, vz * dt);
            points.Add(pos);

            if (!maxHeightRecorded && Mathf.Abs(vy) < dt * 3)
            {
                maxHeight = pos.y;
                Debug.Log($"Höchster Punkt: {maxHeight:F2} m");
                maxHeightRecorded = true;
            }

            if (pos.y < 0)
            {
                float range = new Vector2(pos.x, pos.z).magnitude;
                Debug.Log($"Reichweite: {range:F2} m");
                Debug.Log($"Flugzeit: {t:F2} s");
                s = t;
                break;
            }

            t += dt;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    float Luftwiderstand(float vrel, float flaeche, float masse)
    {
        float f = 0.45f * flaeche * 1.225f * vrel * vrel / (2 * masse);
        return vrel > 0 ? f : -f;
    }
}
