using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeMovement : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] Vector2 _startingNodeCoordinates;
    [SerializeField] SpriteRenderer _targetSprite;
    [SerializeField] Vector3 _axis = Vector3.forward;

    [Header("Movement Setting")]
    [SerializeField] float _movementSpeed = 2f;
    // [SerializeField] float _homeTurfSpeedBonus = 0.5f;

    public NodeObject CurrentNode { get; private set; }
    public bool Moving { get; private set; } = false;
    // public bool InHomeTurf = false;

    InputActionMap _playerActions;
    NodeObject _target = null;
    NodeObject _movingTo = null;
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

    void Update()
    {
        if (Moving && transform.position != _movingTo.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, _movingTo.transform.position, _currentMovementSpeed * Time.deltaTime);
            if (transform.position == _movingTo.transform.position)
            {
                CurrentNode = _movingTo;
                _movingTo = null;
                Moving = false;
            }
        }
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

    void TargetPreviousNode(InputAction.CallbackContext value)
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

    void TravelToTargetNode(InputAction.CallbackContext value)
    {
        if (_target != null && !Moving)
        {
            // NodeObject temp = CurrentNode;
            _movingTo = _target;
            Moving = true;
            // transform.position = _target.transform.position;
            _target = CurrentNode;
            _targetSprite.transform.position = _target.transform.position;
        }
    }
}
