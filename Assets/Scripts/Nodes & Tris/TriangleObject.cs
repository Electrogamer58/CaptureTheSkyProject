using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TriangleObject : MonoBehaviour, IEquatable<TriangleObject>
{
    [SerializeField] float _scorePerSquareUnit = 2.5f;
    [SerializeField] float _colorAlpha = .99f;

    public Triangle Tri { get; private set; }
    public PlayerObject Owner = null;
    public SpriteShapeController spriteShapeController;
    public SpriteShapeRenderer spriteShapeRenderer;

    bool playedAudio = false;
    
    public bool Equals(TriangleObject other)
    {
        return Tri.Equals(other.Tri);
    }

    void OnEnable()
    {
        NodeMovement.CaptureEdge += OnEdgeCaptured;
    }

    void OnDisable()
    {
        NodeMovement.CaptureEdge -= OnEdgeCaptured;
    }
    
    private void Awake()
    {
        spriteShapeController = GetComponent<SpriteShapeController>();
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (Owner != null)
        {

            Owner.Score.GivePoints(Tri.Area * _scorePerSquareUnit * Time.deltaTime);

            //enable sprite renderer component
            spriteShapeRenderer.enabled = true;
            //change color to that of Owner
            spriteShapeRenderer.color = Owner.GetComponent<SpriteRenderer>().color;
            
            // Get the current color of the SpriteRenderer
            Color spriteColor = spriteShapeRenderer.color;

            // Set the alpha value to 0.5
            spriteColor.a = _colorAlpha;

            // Assign the new color to the SpriteRenderer
            GetComponent<SpriteShapeRenderer>().color = spriteColor;

            if (!playedAudio)
            {
                var ownerMovement = Owner.GetComponent<NodeMovement>();
                ownerMovement._myAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
                ownerMovement._myAudioSource.PlayOneShot(ownerMovement._triangleCreationClip);
                playedAudio = true;
            }

        }
        if (Owner == null)
        {
            spriteShapeRenderer.enabled = false;
        }
    }

    public void Initialize(NodeObject point1, NodeObject point2, NodeObject point3)
    {
        Tri = new Triangle(point1, point2, point3);
        spriteShapeController.spline.Clear();
        spriteShapeController.spline.InsertPointAt(0, point1.transform.position);
        spriteShapeController.spline.InsertPointAt(1, point2.transform.position);
        spriteShapeController.spline.InsertPointAt(2, point3.transform.position);
        spriteShapeRenderer.enabled = false;
    }

    public void AddToNodes()
    {
        foreach (NodeObject node in Tri.Points)
        {
            node.Triangles.Add(Tri);
            node.TriangleObjs.Add(this);
        }
    }

    void OnEdgeCaptured(PlayerObject player, NodePair edge)
    {
        if ((Tri.Points[0].Node == edge.First || Tri.Points[1].Node == edge.First || Tri.Points[2].Node == edge.First) &&
            (Tri.Points[0].Node == edge.Second || Tri.Points[1].Node == edge.Second || Tri.Points[2].Node == edge.Second))
        {
            CheckPlayerControl(player);
        }
    }

    public bool CheckPlayerControl(PlayerObject player)
    {
        List<NodePair> playerEdges = player.Movement.Edges;
        
        bool hasAB = PlayerHasEdge(playerEdges, Tri.Points[0], Tri.Points[1]);
        bool hasAC = PlayerHasEdge(playerEdges, Tri.Points[0], Tri.Points[2]);
        bool hasBC = PlayerHasEdge(playerEdges, Tri.Points[1], Tri.Points[2]);

        if (hasAB && hasAC && hasBC)
        {
            Owner = player;
            Debug.Log(Owner.gameObject.name);
        }
        else if (Owner != player && Owner != null && (hasAB || hasAC || hasBC))
        {
            Owner = null;
        }

        return Owner == player;
    }

    bool PlayerHasEdge(List<NodePair> playerEdges, NodeObject point1, NodeObject point2)
    {
        return playerEdges.Contains(new NodePair(point1, point2));
    }
}
