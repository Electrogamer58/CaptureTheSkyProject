using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flag : MonoBehaviour
{
    [SerializeField] float _pointValue = 10f;

    List<NodeObject> _points = new List<NodeObject>();
    float _currentCaptureProgress = 0;
    string _team = null;

    public static event Action<Flag> FlagCollected;

    public void Collect(PlayerScore player)
    {
        player.GivePoints(_pointValue);
        FlagCollected?.Invoke(this);
    }

    public void ProgressCapture(float amount, PlayerScore player)
    {
        if (player.Team != _team)
        {
            _currentCaptureProgress = Mathf.Clamp(_currentCaptureProgress - amount, 0f, 1f);
            if (_currentCaptureProgress == 0)
            {
                _team = player.Team;
            }
        }
        else
        {
            _currentCaptureProgress = Mathf.Clamp(_currentCaptureProgress + amount, 0f, 1f);
            if (_currentCaptureProgress == 1)
            {
                Collect(player);
            }
        }
    }

    public void SetNodes(NodeObject point1, NodeObject point2, NodeObject point3)
    {
        _points.Add(point1); _points.Add(point2); _points.Add(point3);
    }

    public bool PlayerHasControl(List<LineRenderer> playerEdges)
    {
        return PlayerHasEdge(playerEdges, _points[0], _points[1]) && PlayerHasEdge(playerEdges, _points[0], _points[2]) && PlayerHasEdge(playerEdges, _points[1], _points[2]);
    }

    bool PlayerHasEdge(List<LineRenderer> playerEdges, NodeObject point1, NodeObject point2)
    {
        foreach (LineRenderer edge in playerEdges)
        {
            Vector3[] pointArray = new Vector3[edge.positionCount];
            edge.GetPositions(pointArray);
            List<Vector3> points = new List<Vector3>(pointArray);
            if (points.Contains(point1.transform.position) && points.Contains(point2.transform.position))
            {
                return true;
            }
        }
        return false;
    }
}
