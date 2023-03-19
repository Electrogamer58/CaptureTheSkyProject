using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] public string Team = null; //{ get; private set; }
    [SerializeField] UnityEvent<float> OnScorePoints;
    [SerializeField] public List<Flag> _flagList;

    public float _score = 0;

    public void GivePoints(float amount)
    {
        _score += amount;
        OnScorePoints?.Invoke(amount);
    }
}
