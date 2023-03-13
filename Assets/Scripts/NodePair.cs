using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePair
{
    public Node First { get; set; }
    public Node Second { get; set; }

    private List<Node> Nodes;

    public NodePair(Node first, Node second)
    {
        First = first;
        Second = second;

        Nodes = new List<Node>();
        Nodes.Add(first);
        Nodes.Add(second);
    }

    /*public bool ContainsBothPoints(Point point1, Point point2)
    {
        //Debug.Log("ContainsPoints()");
        if (Points.Contains(point1) && Points.Contains(point2))
            return true;
        else
            return false;
    }*/
}
