using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : SelectableNode
{
    public Node Node { get; private set; }
    public int XInt { get; private set; }
    public int YInt { get; private set; }
    public List<NodeObject> Neighbors { get; private set; }
    [SerializeField] public PlayerObject _player = null;

    private GridGenerator GridGenerator;

    private void Start()
    {
        //ResetNeighbors();
        GridGenerator = GetComponentInParent<GridGenerator>();
    }
    public void ResetNeighbors()
    {
        Neighbors = new List<NodeObject>();
    }
    public void SetPoint(Node node)
    {
        Node = node;
        XInt = Node.Xcoord;
        YInt = Node.Ycoord;
    }
    public void AddNeighbor(NodeObject nodeObject)
    {
        if (!Neighbors.Contains(nodeObject))
        {
            Neighbors.Add(nodeObject);
        }
    }

    public void SelectThisNode()
    {
        GridGenerator.UnselectNodes();

        ChangeSelectionState(SelectedState.Selected);

        foreach (var neighbor in Neighbors)
        {
            neighbor.ChangeSelectionState(SelectedState.Neighbor);
        }

        GridGenerator._selectedNode = this;

    }
    public void UnselectThisNode()
    {
        ChangeSelectionState(SelectedState.Unselected);

        if (GridGenerator._selectedNode == this)
            GridGenerator._selectedNode = null;
    }

    #region Attempt at Dot Product stuff (doesnt work)
    /*public void ChangeNeighborToNextNode(NodeObject nextNode)
    {
        foreach (var neighbor in Neighbors)
        {
            neighbor.UnselectThisNode();
        }

        ChangeSelectionState(SelectedState.Selected);

        nextNode.ChangeSelectionState(SelectedState.Neighbor);
    }
    public NodeObject GetNextNeighbor(Vector2 inputDirection)
    {
        Debug.LogWarning("GetNextNeighbor()");
        Dictionary<Vector2, NodeObject> neighborDirections = GetDirectionsOfNeighbors();
        SortedDictionary<float, Vector2> dotDirections = new SortedDictionary<float, Vector2>();

        float bestDot = -100f;

        foreach (var nDir in neighborDirections)
        {
            float nDot = Vector2.Dot(nDir.Key.normalized, inputDirection.normalized);
            dotDirections.Add(nDot, nDir.Key);

            if (nDot >= bestDot)
                bestDot = nDot;

            //Debug.Log("Neighbor, Dot: (" + nDir.Value.Point.Xcoord + "," + nDir.Value.Point.Ycoord + "); " + nDot);
        }

        *//*if (bestDot != -100f)
            Debug.Log("Next: (" + neighborDirections[dotDirections[bestDot]].Point.Xcoord + ","
            + neighborDirections[dotDirections[bestDot]].Point.Xcoord + ") " + bestDot);*//*

        if (bestDot == -100f)
            return null;
        else
            return neighborDirections[dotDirections[bestDot]];
    }
    private Dictionary<Vector2, NodeObject> GetDirectionsOfNeighbors()
    {
        Dictionary<Vector2, NodeObject> dirNeighbors = new Dictionary<Vector2, NodeObject>();

        foreach (var neighbor in Neighbors)
        {
            Vector2 dir = Node.Position - neighbor.Node.Position;
            dirNeighbors.Add(dir, neighbor);
        }

        return dirNeighbors;
    }*/
    #endregion
}
