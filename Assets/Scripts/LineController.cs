using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// REMEMBER TO CREDIT BLANKdev 
/// </summary>
[RequireComponent(typeof(LineController))]
public class LineController : MonoBehaviour
{
    [SerializeField] public List<NodeObject> nodes;
    [SerializeField] public List<Transform> nodeTransforms;
    [SerializeField] public string Team = null;
    //[SerializeField] public float Lifetime = 10f;
    [SerializeField] public PolygonCollider2D _pc;
    LineRenderer lr;

    public NodeMovement _myParent;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = nodeTransforms.Count;
        _pc = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPositions(nodeTransforms.ConvertAll(n => n.position - new Vector3(0, 0, 5)).ToArray());

        //Lifetime -= Time.deltaTime;
        //if (Lifetime <= 0)
        //{
        //    gameObject.SetActive(false);
        //}

    }

    public Vector3[] GetPositions() {
        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);
        return positions;
    }

    public float GetWidth() {
        return lr.startWidth;
    }

    public void AddNodes(NodeObject node1, NodeObject node2)
    {
        nodes.Add(node1);
        nodes.Add(node2);
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    //if (collision.gameObject.GetComponent<LineController>().Team != null)
    //    //{
    //    if (collision.gameObject.GetComponent<LineController>()._myParent != _myParent)
    //        {
    //            Debug.Log("overlappingLines");
    //            collision.gameObject.SetActive(false);
    //        }
    //    else
    //    {
    //        return;
    //    }
    //    //}
    //    
    //}
}
