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
    [SerializeField] public ParticleSystem _enterParticleSystem;
    [SerializeField] public AudioSource _myAudioSource;

    [SerializeField] public Transform _player1Side;
    [SerializeField] public Transform _player2Side;

    List<NodeObject> _points = new List<NodeObject>();
    [SerializeField] private List<GameObject> _planets;
    TriangleObject _currentTri = null;
    PlayerObject _controller = null;
    float _currentCaptureProgress = 0;
    bool _collected = false;


    public static event Action<Flag> FlagCollected;

    private void Awake()
    {
        int i = Mathf.FloorToInt(Random.Range(0, 3));
        _planets[i].SetActive(true);
        _myParticleSystem = _planets[i].transform.Find("Charging").GetComponent<ParticleSystem>();
        _enterParticleSystem = _planets[i].transform.Find("Enter").GetComponent<ParticleSystem>();

        _player1Side = GameObject.FindGameObjectWithTag("PlanetsLeftSide").GetComponent<Transform>() ;
        _player2Side = GameObject.FindGameObjectWithTag("PlanetsRightSide").GetComponent<Transform>();

    }
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
        
        FlagCollected?.Invoke(this);
        //_enterParticleSystem.Play();

        if (player.Team == "Player 1" && !_collected)
        {
            transform.position = _player1Side.position;
            gameObject.transform.parent = _player1Side;
            gameObject.GetComponent<Rigidbody2D>().WakeUp();
            _myAudioSource.pitch = Random.Range(0.9f, 1.1f);
            _myAudioSource.Play();
            _collected = true;
            //gameObject.layer = 5;
        }
        if (player.Team == "Player 2" && !_collected)
        {
            transform.position = _player2Side.position;
            gameObject.transform.parent = _player2Side;
            gameObject.GetComponent<Rigidbody2D>().WakeUp();
            _myAudioSource.pitch = Random.Range(0.9f, 1.1f);
            _myAudioSource.Play();
            _collected = true;
        }

        //gameObject.layer = 5;
        
        //Destroy(gameObject);
    }

    public void EndCollect(PlayerScore player)
    {
        player.GivePoints(_pointValue);
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
        if (_currentCaptureProgress < 1)
        {
            _myParticleSystem.Play();
        }
    }

    public void SetTri(TriangleObject triangleObj)
    {
        _currentTri = triangleObj;
        _points.Add(_currentTri.Tri.Points[0]); _points.Add(_currentTri.Tri.Points[1]); _points.Add(_currentTri.Tri.Points[2]);
    }
}
