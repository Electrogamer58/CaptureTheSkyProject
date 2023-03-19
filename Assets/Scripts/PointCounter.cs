using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointCounter
{
    [SerializeField] static int scoreA = 0;
    [SerializeField] static int scoreB = 0;

    public static void AddScore(bool player1, int scoreToAdd)
    {
        if (player1)
        {
            scoreA += scoreToAdd;
        }
        else if (!player1)
        {
            scoreB += scoreToAdd;
        }
    }

}
