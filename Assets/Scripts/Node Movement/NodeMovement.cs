using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

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
    [SerializeField] private SpriteShapeController _spriteShape = null;
    [SerializeField] private List<SpriteShapeController> _shapeList = null;

    public NodeObject CurrentNode { get; private set; }
    public bool Moving { get; private set; } = false;
    [SerializeField] public bool inMyLine = false;
    private PlayerObject _myPlayerObject = null;
    // public bool InHomeTurf = false;

    InputActionMap _playerActions;
    NodeObject _target = null;
    NodeObject _lastNode = null;
    float _currentMovementSpeed = 0;

    void OnEnable()
    {
        GridGenerator.GridGenerated += OnGridGenerate;
        
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
    }

    void Update()
    {
        if (Moving && transform.position != CurrentNode.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, _movementSpeed * Time.deltaTime);
            if (transform.position == CurrentNode.transform.position)
            {
                RenderLine(_lastNode.transform, CurrentNode.transform);
                _playerTrail.emitting = false;
                Moving = false;
                _targetSprite.enabled = true;
                CurrentNode._player = _myPlayerObject;
                //RenderShape();
                //_lineRenderer.gameObject.SetActive(false);
                if (_playerParticles != null)
                    MakeParticles(_playerParticles, true);
                
            }
        }
        //if (_movingTo)
        //RenderLine(CurrentNode, _movingTo);
    }
    
    void OnGridGenerate()
    {
        List<NodeObject> allNodes = new List<NodeObject>(FindObjectsOfType<NodeObject>());
        NodeObject startingNode = null;
        
        foreach (NodeObject nodeObject in allNodes)
        {
            if (new Vector2(nodeObject.Node.Xcoord, nodeObject.Node.Ycoord) == _startingNodeCoordinates)
            {
                startingNode = nodeObject;
            }
        }
        
        CurrentNode = startingNode;
        transform.position = startingNode.transform.position;
        _target = CurrentNode.Neighbors[0];
        _targetSprite.transform.position = _target.transform.position;
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
            _target = _lastNode;
            _targetSprite.transform.position = _target.transform.position;
        }
    }

    private void RenderLine(Transform startPoint, Transform targetPoint)
    {
        foreach (LineRenderer renderer in _lineList)
        {
            if (renderer.gameObject.activeSelf == true)
            {
                var controller = renderer.GetComponent<LineController>();
                for (int i = 0; i < controller.nodes.Count-1; i++)
                {
                    if ((controller.nodes[i] == startPoint && controller.nodes[i+1] == targetPoint) || 
                        (controller.nodes[i] == targetPoint && controller.nodes[i+1] == startPoint))
                    {
                        Debug.Log("Line already exists, returning");
                        inMyLine = true;
                        return;
                    }
                }
            } else if (renderer.gameObject.activeSelf == false)
            {
                _lineList.Remove(renderer);
                Destroy(renderer.gameObject);
                break;
            }
            
        }

        
        var _newLineRenderer = Instantiate(_lineRenderer, lineListParent.transform, true);
        _lineList.Add(_newLineRenderer);
        inMyLine = false;
        _newLineRenderer.gameObject.SetActive(true);
        _newLineRenderer.GetComponent<LineController>().nodes.Add(startPoint);
        _newLineRenderer.GetComponent<LineController>().nodes.Add(targetPoint);
        _newLineRenderer.GetComponent<LineController>().Team = myTeam; //set names to be equal

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

   // private void RenderShape()
   // {
   //     foreach (LineRenderer renderer in _lineList)
   //     {
   //         for (int i = 0; i < _lineList.Count - 1; i++)
   //         {
   //             if ((_lineList[i].GetComponent<LineController>().nodes[0] == _lineList[i + 1].GetComponent<LineController>().nodes[0]) || (_lineList[i].GetComponent<LineController>().nodes[0] == _lineList[i + 1].GetComponent<LineController>().nodes[1]) || (_lineList[i].GetComponent<LineController>().nodes[1] == _lineList[i + 1].[0]) || (_lineList[i].GetComponent<LineController>().nodes[1] == _lineList[i + 1].GetComponent<LineController>().nodes[1]))
   //             {
   //                 if ((_lineList[i].GetComponent<LineController>().nodes[0] == _lineList[i - 1].GetComponent<LineController>().nodes[0]) || (_lineList[i].GetComponent<LineController>().nodes[0] == _lineList[i - 1].GetComponent<LineController>().nodes[1]) || (_lineList[i].GetComponent<LineController>().nodes[1] == _lineList[i - 1].GetComponent<LineController>().nodes[0]) || (_lineList[i].GetComponent<LineController>().nodes[1] == _lineList[i - 1].GetComponent<LineController>().nodes[1]))
   //                     Debug.Log("Create Shape");
   //             }
   //         }
   //         //if (point1 && point2 && point3) //if three lines share 2 points
   //         //{
   //         //    //create new sprite shape
   //         //    //add to list
   //         //    //set points
   //         //}
   //     }
   // }

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
