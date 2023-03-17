using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] public string Team { get; private set; } = null;
    [SerializeField] UnityEvent<float> OnScorePoints;

    float _score = 0;

    public void GivePoints(float amount)
    {
        _score += amount;
        Debug.Log(gameObject.name + " Scored: " + amount + "\t Total: " + _score);
        OnScorePoints?.Invoke(amount);
    }
}
