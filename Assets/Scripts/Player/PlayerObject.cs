using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public NodeMovement Movement { get; private set; }
    public PlayerScore Score { get; private set; }

    void Start()
    {
        Movement = GetComponent<NodeMovement>();
        Score = GetComponent<PlayerScore>();
    }
}
