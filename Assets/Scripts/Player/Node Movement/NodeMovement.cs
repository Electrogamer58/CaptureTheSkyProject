using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class NodeMovement : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] Vector2 _startingNodeCoordinates;
    [SerializeField] SpriteRenderer _targetSprite;
    [SerializeField] Vector3 _axis = Vector3.forward;
    [SerializeField] string myTeam = null;

    [Header("Movement Setting")]
    [SerializeField] float _movementSpeed = 2f;
    [SerializeField] float _movementSpeedMultiplier = 2f;
    private float _originalMovementSpeed;
    private float _increasedMovementSpeed;
    private float _decreasedMovementSpeed;

    [Header("Visual Settings")]
    [SerializeField] private TrailRenderer _playerTrail;
    [SerializeField] private ParticleSystem _playerParticles;
    [SerializeField] private LineRenderer _lineRenderer = null;
    [SerializeField] private List<LineRenderer> _lineList = null;
    [SerializeField] private GameObject lineListParent;
    [SerializeField] private List<SpriteShapeController> _shapeList = null;

    [Header("Sound Settings")]
    [SerializeField] public AudioSource _myAudioSource;
    [SerializeField] public AudioClip _nodeMovementClip;
    [SerializeField] public AudioClip _triangleCreationClip;
    [SerializeField] public AudioClip _flyingClip;

    [SerializeField] public bool inMyLine = false;
    public NodeObject CurrentNode { get; private set; }
    public bool Moving { get; private set; } = false;
    private PlayerObject _myPlayerObject = null;

    public List<NodePair> Edges { get; private set; } = new List<NodePair>();
    InputActionMap _playerActions;
    NodeObject _target = null;
    NodeObject _lastNode = null;
    float _currentMovementSpeed = 0;

    public static event Action<PlayerObject, NodePair> CaptureEdge;

    void OnEnable()
    {
        GridGenerator.GridGenerated += OnGridGenerate;
        CaptureEdge += OnEdgeCaptured;
        
        _playerActions = GetComponent<PlayerInput>().currentActionMap;
        
        _playerActions["Previous"].performed += TargetPreviousNode;
        _playerActions["Next"].performed += TargetNextNode;
        _playerActions["Confirm"].performed += TravelToTargetNode;
        _playerActions.Enable();

        _currentMovementSpeed = _movementSpeed;
    }

    void OnDisable()
    {
        GridGenerator.GridGenerated -= OnGridGenerate;
        CaptureEdge -= OnEdgeCaptured;
        
        _playerActions["Previous"].performed -= TargetPreviousNode;
        _playerActions["Next"].performed -= TargetNextNode;
        _playerActions["Confirm"].performed -= TravelToTargetNode;
        _playerActions.Disable();
    }

    private void Awake()
    {
        _originalMovementSpeed = _movementSpeed;
        _increasedMovementSpeed = _movementSpeed *= _movementSpeedMultiplier;
        _decreasedMovementSpeed = _movementSpeed /= 2;
        _myPlayerObject = gameObject.GetComponent<PlayerObject>();

        _shapeList.AddRange(FindObjectsOfType<SpriteShapeController>());
    }

    void Update()
    {
        if (Moving && transform.position != CurrentNode.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, _movementSpeed * Time.deltaTime);
            _targetSprite.transform.position = _target.transform.position;
            if (transform.position == CurrentNode.transform.position)
            {
                RenderLine(_lastNode.transform, CurrentNode.transform);
                _playerTrail.emitting = false;
                Moving = false;
                _targetSprite.enabled = true;
                CurrentNode._player = _myPlayerObject;
                if (_playerParticles != null)
                {
                    MakeParticles(_playerParticles, true);
                }
                _myAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
                _myAudioSource.PlayOneShot(_nodeMovementClip);

                NodePair edge = new NodePair(_lastNode, CurrentNode);
                if (!Edges.Contains(edge))
                {
                    Edges.Add(edge);
                    CaptureEdge?.Invoke(_myPlayerObject, edge);
                }
            }
        }
    }
    
    void OnGridGenerate()
    {
        GridGenerator grid = FindObjectOfType<GridGenerator>();
        NodeObject startingNode = grid._nodeObjectsDictionary[_startingNodeCoordinates];
        CurrentNode = startingNode;
        transform.position = CurrentNode.transform.position;
        _target = CurrentNode.Neighbors[0];
        _targetSprite.transform.position = _target.transform.position;

        foreach (LineRenderer line in _lineList)
        {
            Destroy(line.gameObject);
        }
        _lineList.Clear();
        _lineList.TrimExcess();
    }

    void OnEdgeCaptured(PlayerObject player, NodePair edge)
    {
        if (_myPlayerObject != player)
        {
            Edges.Remove(edge);
        }
    }

    void TargetNextNode(InputAction.CallbackContext value)
    {
        if (!Moving)
        {
            if (_target != null)
            {
                Vector3 dirToTarget = _target.transform.position - transform.position;

                NodeObject next = null;
                float angleToNext = 360f;

                foreach (NodeObject node in CurrentNode.Neighbors)
                {
                    if (node != _target)
                    {
                        Vector3 dirToNode = node.transform.position - transform.position;
                        float angleToNode = Vector3.SignedAngle(dirToTarget, dirToNode, _axis);
                        if (angleToNode < 0)
                        {
                            angleToNode += 360f;
                        }

                        if (angleToNode < angleToNext)
                        {
                            next = node;
                            angleToNext = angleToNode;
                        }
                    }
                }

                _target = next;
                _targetSprite.transform.position = _target.transform.position;
            }
            else
            {
                _target = CurrentNode.Neighbors[0];
                _targetSprite.transform.position = _target.transform.position;
            }
        }
    }

    void TargetPreviousNode(InputAction.CallbackContext value)
    {
        if (!Moving)
        {
            if (_target != null)
            {
                Vector3 dirToTarget = _target.transform.position - transform.position;

                NodeObject previous = null;
                float angleToPrevious = 0f;

                foreach (NodeObject node in CurrentNode.Neighbors)
                {
                    if (node != _target)
                    {
                        Vector3 dirToNode = node.transform.position - transform.position;
                        float angleToNode = Vector3.SignedAngle(dirToTarget, dirToNode, _axis);
                        if (angleToNode < 0)
                        {
                            angleToNode += 360f;
                        }

                        if (angleToNode > angleToPrevious)
                        {
                            previous = node;
                            angleToPrevious = angleToNode;
                        }
                    }
                }

                _target = previous;
                _targetSprite.transform.position = _target.transform.position;
            }
            else
            {
                _target = CurrentNode.Neighbors[0];
                _targetSprite.transform.position = _target.transform.position;
            }
        }
    }

    void TravelToTargetNode(InputAction.CallbackContext value)
    {
        if (_target != null && !Moving)
        {
            NodeObject temp = CurrentNode;
            CurrentNode = _target;
            _movementSpeed = _originalMovementSpeed;
            Moving = true;
            if (_playerTrail)
                _playerTrail.emitting = true;
            _targetSprite.enabled = false;


            _lastNode = temp;
            _target = GetNextNode();
            _targetSprite.transform.position = _target.transform.position;
        }
    }

    NodeObject GetNextNode()
    {
        Vector3 dir = CurrentNode.transform.position - _lastNode.transform.position;
        NodeObject next = null;
        float angleToNext = 180f;

        foreach (NodeObject node in CurrentNode.Neighbors)
        {
            if (next == null || Vector3.Angle(dir, next.transform.position - CurrentNode.transform.position) < angleToNext)
            {
                next = node;
                angleToNext = Vector3.Angle(dir, next.transform.position - CurrentNode.transform.position);
            }
        }

        return next;
    }

    private void RenderLine(Transform startPoint, Transform targetPoint)
    {
        foreach (LineRenderer renderer in _lineList)
        {
            if (renderer.gameObject.activeSelf == true)
            {
                var controller = renderer.GetComponent<LineController>();
                for (int i = 0; i < controller.nodeTransforms.Count-1; i++)
                {
                    if ((controller.nodeTransforms[i] == startPoint && controller.nodeTransforms[i+1] == targetPoint) || 
                        (controller.nodeTransforms[i] == targetPoint && controller.nodeTransforms[i+1] == startPoint))
                    {
                        Debug.Log("Line already exists, returning");
                        inMyLine = true;
                        return;
                    }
                }
            }
            else if (renderer.gameObject.activeSelf == false)
            {
                var controller = renderer.GetComponent<LineController>();
                _lineList.Remove(renderer);
                NodePair edge = new NodePair(controller.nodes[0], controller.nodes[1]);
                Edges.Remove(edge);
                foreach (TriangleObject triangleObj in CurrentNode.TriangleObjs)
                {
                   triangleObj.CheckPlayerControl(_myPlayerObject);
                }

                Destroy(renderer.gameObject);
                break;
            }
            
        }

        
        var _newLineRenderer = Instantiate(_lineRenderer, lineListParent.transform, true);
        _lineList.Add(_newLineRenderer);
        inMyLine = false;
        _newLineRenderer.gameObject.SetActive(true);
        var _newLineController = _newLineRenderer.GetComponent<LineController>();
        _newLineController.nodeTransforms.Add(startPoint);
        _newLineController.nodeTransforms.Add(targetPoint);
        _newLineController.Team = myTeam; //set names to be equal
        _newLineController.AddNodes(_lastNode, CurrentNode);


        #region Old RenderLine
        //OLD CODE//
        //_newLineRenderer.positionCount = 2;
        //Vector3 _startPoint = startPoint.transform.position;
        //Vector3 _endPoint = targetPoint.transform.position;
        //
        //_newLineRenderer.SetPosition(0, _startPoint);
        //_newLineRenderer.SetPosition(1, _endPoint);

        //foreach (LineRenderer renderer in _lineList)
        //{
        //    renderer.gameObject.SetActive(true);
        //    renderer.positionCount = 2;
        //    Vector3 _startPoint = startPoint.transform.position;
        //    Vector3 _endPoint = targetPoint.transform.position;
        //
        //    renderer.SetPosition(0, _startPoint);
        //    renderer.SetPosition(1, _endPoint);
        //}
        #endregion
    }

    private void MakeParticles(ParticleSystem _particleSystem, bool play)
    {
        ParticleSystem newParticles = Instantiate(_particleSystem, CurrentNode.transform, false);
        if (newParticles && play)
        {
            newParticles.Play();
        }

        var particlesLifetime = newParticles.main.duration;
        particlesLifetime -= Time.deltaTime;
        if (particlesLifetime <= 0)
        {
            Destroy(newParticles);
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log(collision);
        if (collision.GetComponent<LineController>())
        {
            //Debug.Log("entered collider line");
            if (collision.GetComponent<LineController>().Team == myTeam)
            {
                //Debug.Log("increased speed");
                _movementSpeed = _increasedMovementSpeed;
                
            }
            else
            {
                _movementSpeed = _decreasedMovementSpeed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<LineController>())
        {
            {
                //Debug.Log("decreased speed");
                _movementSpeed = _originalMovementSpeed;
            }
            
        }
    }
}
