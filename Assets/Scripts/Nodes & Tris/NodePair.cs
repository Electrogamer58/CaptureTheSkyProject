using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodePair : IEquatable<NodePair>
{
    public Node First { get; set; }
    public Node Second { get; set; }

    private List<Node> Nodes;

    public NodePair(Node first, Node second)
    {
        First = first;
        Second = second;

        Nodes = new List<Node>();
        Nodes.Add(First);
        Nodes.Add(Second);
    }

    public NodePair(NodeObject first, NodeObject second)
    {
        First = first.Node;
        Second = second.Node;

        Nodes = new List<Node>();
        Nodes.Add(First);
        Nodes.Add(Second);
    }

    public bool Equals(NodePair other)
    {
        if (other.Nodes.Contains(First) && other.Nodes.Contains(Second))
            return true;
        else
            return false;
    }
}
