using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleMath
{
    public static float AreaFromPoints(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Vector3 AB = point2 - point1;
        Vector3 AC = point3 - point1;

        float angle = Vector3.Angle(AB, AC) * Mathf.Deg2Rad;

        return 0.5f * AB.magnitude * AC.magnitude * Mathf.Sin(angle);
    }

    public static float AreaFromPoints(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        return 0.5f * ((point1.x * (point2.y - point3.y)) + (point2.x * (point3.y - point1.y)) + (point3.x * (point1.y - point2.y)));
    }

    public static Vector3 CenterFromPoints(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        return new Vector3((point1.x + point2.x + point3.x) / 3, (point1.y + point2.y + point3.y) / 3, (point1.z + point2.z + point3.z) / 3);
    }
}
