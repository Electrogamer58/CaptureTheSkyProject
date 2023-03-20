using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] public string Team = null; //{ get; private set; }
    [SerializeField] UnityEvent<float> OnScorePoints;
    [SerializeField] public List<Flag> _flagList;

    [SerializeField] TimerCountdown _timer;

    public float _score = 0;
    private void Awake()
    {
        if (!_timer)
            _timer = FindObjectOfType<TimerCountdown>();
    }

    public void GivePoints(float amount)
    {
        if (_timer._gameStart)
        {
            _score += amount;
            OnScorePoints?.Invoke(amount);
        }
        
    }
}
