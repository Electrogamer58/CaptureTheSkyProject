using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    public static event Action GridGenerated;
    public static event Action TrianglesConstructed;
    
    public static float TRIWIDTH = 1.0f;
    public static float TRIOFFSET = TRIWIDTH * 0.5F;
    public static float TRIHEIGHT = Mathf.Sqrt(Mathf.Pow(TRIWIDTH, 2) - Mathf.Pow(TRIOFFSET, 2));

    [SerializeField] private Transform pointPrefab;
    [SerializeField] private float gridOffset = 0.25f;
    [SerializeField] private int gridDimensions = 10;

    [SerializeField] private bool drawLines;
    [SerializeField] private bool drawCenterPoint;
    [SerializeField] private float gridRadius = 4f;

    [SerializeField] bool generateOnStartup = false;
    [SerializeField] bool _addLines = true;
    [SerializeField] GameObject _myLineRenderer = null;

    Node[] _nodes;
    public Dictionary<Vector2, NodeObject> _nodeObjectsDictionary { get; private set; }
    public NodeObject _selectedNode { get; set; }
    public NodeObject _nextNode { get; set; }

    List<NodePair> _nodePairs;
    Vector2 _centerPoint;

    //int pairCount = 0;

    private void Start()
    {
        ClearGrid();
        

        if (generateOnStartup)
        {
            GenerateGridWithLines();
        }
    }
    public void ClearGrid()
    {
        //reset values
        transform.position = Vector2.zero;
        
        _nodes = null;
        _nodePairs = new List<NodePair>();
        //pairCount = 0;
        _centerPoint = Vector2.zero;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }
    }
    public void GenerateGridWithLines()
    {
        //clear grid
        ClearGrid();

        //get points
        _nodes = GenerateNodes(gridOffset, gridDimensions, gridDimensions);
        _centerPoint = FindCenterPoint(_nodes);

        //create gameobject points
        _nodeObjectsDictionary = GenerateNodeGameObjects(_nodes);
        GetComponent<SquareToCircle>().CircularizeGrid();

        //pairs of points
        _nodePairs = GetNodePairs(_nodes);

        //add neighbors to points
        AddNeighbors(_nodeObjectsDictionary, _nodePairs);

        //move camera
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(_centerPoint.x, _centerPoint.y, cam.transform.position.z);
        cam.orthographicSize = gridDimensions * TRIWIDTH * 0.5f;
        //cam.fieldOfView = 6 * gridDimensions * TRIWIDTH;
        transform.position = new Vector3(0.45f, 0, 0);

       //if (_addLines)
       //{
       //    if (_nodePairs != null)
       //    {
       //        
       //        foreach (var pair in _nodePairs)
       //        {
       //            Instantiate(_myLineRenderer);
       //            Transform _pair1transform = pointPrefab.transform;
       //            _pair1transform.position = new Vector3(pair.First.Xcoord, pair.First.Ycoord, _pair1transform.position.z);
       //            _myLineRenderer.GetComponent<LineController>().nodeTransforms.Add(_pair1transform);
       //            Transform _pair2transform = pointPrefab.transform;
       //            _pair1transform.position = new Vector3(pair.Second.Xcoord, pair.Second.Ycoord, _pair2transform.position.z);
       //            _myLineRenderer.GetComponent<LineController>().nodeTransforms.Add(_pair2transform);
       //        }
       //    }
       //
       //}

        GridGenerated?.Invoke();
        TrianglesConstructed?.Invoke();
    }
    private Node[] GenerateNodes(float offset, int width, int height)
    {
        Node[] nodes = new Node[width * height];

        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xOffset = Random.Range(-offset, offset);
                float yOffset = Random.Range(-offset, offset);

                if (y % 2 == 1)
                    nodes[count] = new Node(x, y, new Vector2(x * TRIWIDTH + TRIOFFSET + xOffset, y * TRIHEIGHT + yOffset));
                else if (y % 2 == 0)
                    nodes[count] = new Node(x, y, new Vector2(x * TRIWIDTH + xOffset, y * TRIHEIGHT + yOffset));

                count++;
            }
        }

        return nodes;
    }
    private Dictionary<Vector2, NodeObject> GenerateNodeGameObjects(Node[] nodes)
    {
        Dictionary<Vector2, NodeObject> nodeObjects = new Dictionary<Vector2, NodeObject>();
        foreach (var node in nodes)
        {
            //instantiate game object
            Transform prefab = Instantiate(pointPrefab, transform);

            //PointObject properties
            NodeObject nodeObject = prefab.GetComponent<NodeObject>();
            nodeObject.ResetNeighbors();
            nodeObject.SetPoint(node);
            nodeObjects.Add(new Vector2(node.Xcoord, node.Ycoord), nodeObject);

            //transform settings (text to be removed once actual sprites are added
            prefab.position = new Vector3(node.Position.x, node.Position.y, 0);
            prefab.GetComponentInChildren<TextMeshPro>().text = node.Xcoord.ToString() + "," + node.Ycoord.ToString();
        }
        return nodeObjects;
    }
    private List<NodePair> GetNodePairs(Node[] nodes)
    {
        List<NodePair> pairs = new List<NodePair>();
        Vector2[,] vector2s = new Vector2[nodes.Length, nodes.Length];

        //int loopCount = 0;
        for (int first = 0; first < nodes.Length; first++)
        {
            for (int second = 0; second < nodes.Length; second++)
            {
                //ignore current point
                if (nodes[first] != nodes[second])
                {
                    if (nodes[first].Xcoord == nodes[second].Xcoord
                        && nodes[first].Ycoord == nodes[second].Ycoord - 1)
                        pairs.Add(new NodePair(nodes[first], nodes[second]));
                    else if (nodes[first].Xcoord == nodes[second].Xcoord
                        && nodes[first].Ycoord == nodes[second].Ycoord + 1)
                        pairs.Add(new NodePair(nodes[first], nodes[second]));
                    else if (nodes[first].Xcoord == nodes[second].Xcoord - 1
                        && nodes[first].Ycoord == nodes[second].Ycoord)
                        pairs.Add(new NodePair(nodes[first], nodes[second]));
                    else if (nodes[first].Xcoord == nodes[second].Xcoord - 1
                        && nodes[first].Ycoord == nodes[second].Ycoord - 1
                        && nodes[first].Ycoord % 2 == 1)
                        pairs.Add(new NodePair(nodes[first], nodes[second]));
                    else if (nodes[first].Xcoord == nodes[second].Xcoord - 1
                        && nodes[first].Ycoord == nodes[second].Ycoord + 1
                        && nodes[first].Ycoord % 2 == 1)
                        pairs.Add(new NodePair(nodes[first], nodes[second]));
                }
                //loopCount++;
            }
        }
        //Debug.Log("Loop count: " + loopCount);

        return pairs;
    }
    private Vector2 FindCenterPoint(Node[] nodes)
    {
        float x = 0f, y = 0f;

        if (nodes.Length > 0)
        {
            foreach (var node in nodes)
            {
                x += node.Position.x;
                y += node.Position.y;
            }

            return new Vector2(x / nodes.Length, y / nodes.Length);
        }
        else
        {
            return Vector2.zero;
        }
    }
    private void AddNeighbors(Dictionary<Vector2, NodeObject> nodeObjects, List<NodePair> nodePairs)
    {
        foreach (var nGameObject in nodeObjects)
        {
            foreach (var pair in nodePairs)
            {
                Vector2 pairCoord1 = new Vector2(pair.First.Xcoord, pair.First.Ycoord);
                Vector2 pairCoord2 = new Vector2(pair.Second.Xcoord, pair.Second.Ycoord);
                Vector2 objectCoords = new Vector2(nGameObject.Value.Node.Xcoord, nGameObject.Value.Node.Ycoord);

                //if object coords == either pair one or two
                if (objectCoords == pairCoord1)
                    nGameObject.Value.AddNeighbor(nodeObjects[pairCoord2]);
                if (objectCoords == pairCoord2)
                    nGameObject.Value.AddNeighbor(nodeObjects[pairCoord1]);
            }
        }
    }

    public void UnselectNodes()
    {
        foreach (var node in _nodeObjectsDictionary)
        {
            node.Value.ChangeSelectionState(SelectedState.Unselected);
        }
    }
    private void OnDrawGizmos()
    {
        if (drawCenterPoint)
        {
            if (_centerPoint != Vector2.zero)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawSphere(_centerPoint, 0.125f);

                Gizmos.color = new Color(0, 1, 0, 0.5f);

                Gizmos.DrawSphere(_centerPoint, gridRadius);
            }
        }
        if (drawLines)
        {
            if (_nodePairs != null)
            {
                Gizmos.color = Color.black;

                foreach (var pair in _nodePairs)
                {
                    Gizmos.DrawLine(pair.First.Position, pair.Second.Position);
                }
            }

        }
    }
}
