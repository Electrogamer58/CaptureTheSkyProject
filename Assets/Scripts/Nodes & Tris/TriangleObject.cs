using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleObject : MonoBehaviour, IEquatable<TriangleObject>
{
    public Triangle Tri { get; private set; }
    public PlayerObject Owner = null;
    
    public bool Equals(TriangleObject other)
    {
        return Tri.Equals(other.Tri);
    }

    public void Initialize(NodeObject point1, NodeObject point2, NodeObject point3)
    {
        Tri = new Triangle(point1, point2, point3);
    }

    public void AddToNodes()
    {
        foreach (NodeObject node in Tri.Points)
        {
            node.Triangles.Add(Tri);
            node.TriangleObjs.Add(this);
        }
    }

    public bool CheckPlayerControl(PlayerObject player, List<NodePair> playerEdges)
    {
        bool hasAB = PlayerHasEdge(playerEdges, Tri.Points[0], Tri.Points[1]);
        bool hasAC = PlayerHasEdge(playerEdges, Tri.Points[0], Tri.Points[2]);
        bool hasBC = PlayerHasEdge(playerEdges, Tri.Points[1], Tri.Points[2]);

        if (hasAB && hasAC && hasBC)
        {
            Owner = player;
        }
        else
        {
            if (Owner != null && (hasAB || hasAC || hasBC))
            {
                Owner = null;
            }
        }

        return Owner == player;
    }

    bool PlayerHasEdge(List<NodePair> playerEdges, NodeObject point1, NodeObject point2)
    {
        return playerEdges.Contains(new NodePair(point1, point2));
    }
}
