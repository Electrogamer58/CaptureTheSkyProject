using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareToCircle : MonoBehaviour
{
    [Tooltip("Should be equal to at least 1/2 the base of the original rectangle")]
    [SerializeField] float horizontal = 1f;
    [Tooltip("Should be equal to at least 1/2 the height of the original rectangle")]
    [SerializeField] float vertical = 1f;
    [SerializeField] float finalRadius = 1f;
    [SerializeField] Vector3 center = Vector3.zero;
    
    public void CircularizeGrid()
    {
        List<NodeObject> nodesInScene = new List<NodeObject>(FindObjectsOfType<NodeObject>());

        foreach (NodeObject node in nodesInScene)
        {
            Vector3 dir = (node.transform.position - center).normalized;
            float radius = Mathf.Sqrt(Mathf.Pow(vertical * Mathf.Sin(Mathf.Deg2Rad * Vector3.Angle(Vector3.right, dir)), 2) + Mathf.Pow(horizontal * Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(Vector3.right, dir)), 2));
            float correctedRadius = radius * (1 + ((Mathf.Sqrt(2) - 1) * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * 2 * Vector3.Angle(Vector3.up, dir)))));
            float percentDist = Vector3.Distance(node.transform.position, center) / correctedRadius;

            node.transform.position = center + (dir * finalRadius * percentDist);
            node.Node.Position = new Vector2(node.transform.position.x, node.transform.position.y);
        }
    }
}
