using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RayCastController
{ 
    [SerializeField] float maxSlopeAngle = 89f;


    public CollisionInfo collisionInfo;

    public override void Start()
    {
        base.Start();
        collisionInfo.faceDir = 1;
    }



    //движение (вызывается из Player.cs)
    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        UpdateCollideCorners();

        collisionInfo.Reset();
        collisionInfo.moveAmountOld = moveAmount;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            collisionInfo.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisionInfo.below = true;
        }
    }
    //проверка столкновения со стенами
    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisionInfo.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? collideCorners.bottomLeft : collideCorners.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, cMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {

                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
                        moveAmount = collisionInfo.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisionInfo.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisionInfo.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }
            }
        }
    }
    //проверка столкновения с полом и потолком
    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? collideCorners.bottomLeft : collideCorners.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, cMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                }

                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;
            }
        }

        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? collideCorners.bottomLeft : collideCorners.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, cMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionInfo.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
                    collisionInfo.slopeNormal = hit.normal;
                }
            }
        }
    }


    //поднимание по наклонной поверхности
    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
            collisionInfo.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(collideCorners.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, cMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(collideCorners.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, cMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!collisionInfo.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? collideCorners.bottomRight : collideCorners.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, cMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisionInfo.slopeAngle = slopeAngle;
                            collisionInfo.descendingSlope = true;
                            collisionInfo.below = true;
                            collisionInfo.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    public void Crouch(bool sitDown)
    {
        if (sitDown)
        {
            collide.size -= new Vector2(0, 1f);
            collide.offset -= new Vector2(0, 0.5f);
        }
        else
        {
            collide.size += new Vector2(0, 1f);
            collide.offset += new Vector2(0, 0.5f);
        }
        RaySpace();
    }
    public bool CanStand()
    {
        float rayLength = collide.bounds.size.y + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = collideCorners.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, cMask);

            Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

            if (hit)
            {
                return false;
            }
        }
        return true;
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisionInfo.slopeAngle = slopeAngle;
                collisionInfo.slidingDownMaxSlope = true;
                collisionInfo.slopeNormal = hit.normal;
            }
        }

    }


    //хранение информации о столкновениях
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
