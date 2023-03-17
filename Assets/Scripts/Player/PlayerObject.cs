using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public PlayerScore Score { get; private set; }

    void Start()
    {
        Score = GetComponent<PlayerScore>();
    }
}
