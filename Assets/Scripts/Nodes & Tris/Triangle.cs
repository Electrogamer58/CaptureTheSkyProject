using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : IEquatable<Triangle>
{
    NodeObject point1;
    NodeObject point2;
    NodeObject point3;

    public List<NodeObject> Points { get; private set; } = new List<NodeObject>();

    public Vector3 Circumcenter { get; private set; }
    public float Area { get; private set; }

    public Triangle(NodeObject p1, NodeObject p2, NodeObject p3)
    {
        point1 = p1;
        point2 = p2;
        point3 = p3;

        Points.Add(point1);
        Points.Add(point2);
        Points.Add(point3);

        Circumcenter = TriangleMath.Circumcenter(point1.transform.position, point2.transform.position, point3.transform.position);
        Area = TriangleMath.AreaFromPoints(point1.transform.position, point2.transform.position, point3.transform.position);
    }
    
    public bool Equals(Triangle other)
    {
        if (other.Points.Contains(point1) && other.Points.Contains(point2) && other.Points.Contains(point3))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
