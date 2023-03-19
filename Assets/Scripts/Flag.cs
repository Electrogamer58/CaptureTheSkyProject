using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flag : MonoBehaviour
{
    // Change PlayerHasControl to work with new triangle class
    
    [SerializeField] public float _pointValue = 10f;
    [SerializeField] float _captureSpeed = 0.25f;

    [Header("Feedback")]
    [SerializeField] public ParticleSystem _myParticleSystem;
    //[SerializeField] public AudioSource _myAudioSource;

    List<NodeObject> _points = new List<NodeObject>();
    TriangleObject _currentTri = null;
    PlayerObject _controller = null;
    float _currentCaptureProgress = 0;

    public static event Action<Flag> FlagCollected;

    void Update()
    {
        if (_controller != null)
        {
            ProgressCapture(_captureSpeed * Time.deltaTime, _controller);
        }
        else if (_currentTri.Owner != null)
        {
            ProgressCapture(_captureSpeed * Time.deltaTime, _currentTri.Owner);
        }
    }

    public void Collect(PlayerScore player)
    {
        player.GivePoints(_pointValue);
        FlagCollected?.Invoke(this);
        Destroy(gameObject);
    }

    public void ProgressCapture(float amount, PlayerObject player)
    {
        if (player != _currentTri.Owner)
        {
            _currentCaptureProgress = Mathf.Clamp(_currentCaptureProgress - amount, 0f, 1f);
            if (_currentCaptureProgress == 0)
            {
                _currentTri.Owner = player;
            }
        }
        else
        {
            _currentCaptureProgress = Mathf.Clamp(_currentCaptureProgress + amount, 0f, 1f);
            if (_currentCaptureProgress == 1)
            {
                Collect(player.Score);
            }
        }
    }

    public void SetTri(TriangleObject triangleObj)
    {
        _currentTri = triangleObj;
        _points.Add(_currentTri.Tri.Points[0]); _points.Add(_currentTri.Tri.Points[1]); _points.Add(_currentTri.Tri.Points[2]);
    }
}
