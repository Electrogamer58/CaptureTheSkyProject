using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] public string Team = null; //{ get; private set; }
    [SerializeField] UnityEvent<float> OnScorePoints;
    [SerializeField] public List<Flag> _flagList;
    [SerializeField] public TMP_Text _scoreText;
 
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
            if (_scoreText)
                 _scoreText.text = Mathf.FloorToInt(_score).ToString();
            OnScorePoints?.Invoke(amount);
        }
        
    }
}
