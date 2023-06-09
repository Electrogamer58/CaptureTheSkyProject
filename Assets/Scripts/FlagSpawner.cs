using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSpawner : MonoBehaviour
{
    [SerializeField] Flag _flagPrefab;
    [SerializeField] int _maxFlags = 3;
    [SerializeField] float _minSpawnDelay = 5f;
    [SerializeField] float _maxSpawnDelay = 10f;
    [Tooltip("The interval at which the spawner should attempt to instantiate a flag -- should be a multiple of 0.05")]
    [SerializeField] float _checkStep = 0.5f;

    List<TriangleObject> _trisInScene = new List<TriangleObject>();
    List<Flag> _flagsInScene = new List<Flag>();
    float _spawnTimer = 0f;
    bool _needsTris = false;
    
    void OnEnable()
    {
        GridGenerator.TrianglesConstructed += GetAllTris;
        Flag.FlagCollected += RemoveFlag;
    }

    void OnDisable()
    {
        GridGenerator.TrianglesConstructed -= GetAllTris;
        Flag.FlagCollected -= RemoveFlag;
    }

    void FixedUpdate()
    {
        if (_trisInScene.Count != 0 && _flagsInScene.Count < _maxFlags)
        {
            _spawnTimer += Time.deltaTime;

            if (Time.time % _checkStep == 0 && _spawnTimer >= _minSpawnDelay && Random.Range(0f, 1f) <= (_spawnTimer - _minSpawnDelay) / (_maxSpawnDelay - _minSpawnDelay))
            {
                SpawnFlag();
            }
        }

        if (_needsTris && _trisInScene.Count == 0)
        {
            _trisInScene = new List<TriangleObject>(FindObjectsOfType<TriangleObject>());
        }
    }

    void GetAllTris()
    {
        RemoveAllFlags();
        _trisInScene.Clear();
        _trisInScene.TrimExcess();
        _needsTris = true;
    }

    public void SpawnFlag()
    {
        TriangleObject triangleObj = _trisInScene[Random.Range(0, _trisInScene.Count)];
        
        Flag spawned = Instantiate(_flagPrefab, triangleObj.Tri.Circumcenter, Quaternion.identity);
        spawned.SetTri(triangleObj);
        _flagsInScene.Add(spawned);
        _spawnTimer = 0;
        spawned._enterParticleSystem.Play();
    }

    void RemoveFlag(Flag flagToRemove)
    {
        _flagsInScene.Remove(flagToRemove);
    }

    void RemoveAllFlags()
    {
        foreach (Flag flag in _flagsInScene)
        {
            Destroy(flag.gameObject);
        }

        _flagsInScene.Clear();
        _flagsInScene.TrimExcess();
    }
}
