using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class RayCastController : MonoBehaviour
{
    public LayerMask cMask;
    public const float skinWidth = .0015f;
    const float dstBetweenRays = .1f;

    [HideInInspector] public int horizontalRayCount;
    [HideInInspector] public int verticalRayCount;

    [HideInInspector] public float horizontalRaySpacing;
    [HideInInspector] public float verticalRaySpacing;

    [HideInInspector] public BoxCollider2D collide;
    [HideInInspector] public CollideCorners collideCorners;

    private void Awake()
    {
        collide = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        RaySpace();
    }

    //расстояние между векторами
    public void RaySpace()
    {
        Bounds bounds = collide.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    //углы BoxCollider2D
    public void UpdateCollideCorners()
    {
        Bounds bounds = collide.bounds;
        bounds.Expand(skinWidth * -2);

        collideCorners.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        collideCorners.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        collideCorners.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        collideCorners.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // структура для углов BoxCollider2D
    public struct CollideCorners
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
