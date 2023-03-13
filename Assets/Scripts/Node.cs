using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int Xcoord { get; set; }
    public int Ycoord { get; set; }
    public Vector2 Position { get; set; }

    public Node(int x, int y, Vector2 pos)
    {
        Xcoord = x;
        Ycoord = y;
        Position = pos;
    }
}
