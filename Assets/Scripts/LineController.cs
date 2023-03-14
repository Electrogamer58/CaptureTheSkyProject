using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// REMEMBER TO CREDIT BLANKdev 
/// </summary>
[RequireComponent(typeof(LineController))]
public class LineController : MonoBehaviour
{
    [SerializeField] public List<Transform> nodes;
    [SerializeField] public string Team = null;
    [SerializeField] public float Lifetime = 10f;
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = nodes.Count;
    }

    // Update is called once per frame
    void Update()
    {
  
        lr.SetPositions(nodes.ConvertAll(n => n.position - new Vector3(0, 0, 5)).ToArray());

        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0)
        {
            gameObject.SetActive(false);
        }

    }

    public Vector3[] GetPositions() {
        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);
        return positions;
    }

    public float GetWidth() {
        return lr.startWidth;
    }
}
