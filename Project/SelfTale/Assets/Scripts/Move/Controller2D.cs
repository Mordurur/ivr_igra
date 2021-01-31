using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RayCastController
{ 
    [SerializeField] float maxSlopeAngle = 89f;
    [SerializeField] float maxSlopeAngleDown = 60f;

    public CollisionInfo collisionInfo;

    public override void Start()
    {
        base.Start();
    }

    //движение (вызывается из Player.cs)
    public void Move(Vector2 velocity, bool standingOnPlatform = false)
    {
        UpdateCollideCorners();
        collisionInfo.Reset();
        collisionInfo.velocityOld = velocity;

        if(velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            collisionInfo.below = true;
        }
    }
    //проверка столкновения со стенами
    public void HorizontalCollisions(ref Vector2 velocity)
    {
        //направление игрока
        float directionX = Mathf.Sign(velocity.x);

        //длина луча
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        //обрабатывает все горизонтальные лучи
        for (int i = 0; i < horizontalRayCount; i++)
        {

            //создает рейкаст
            Vector2 rayOrigin = (directionX == -1) ? collideCorners.bottomLeft : collideCorners.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, cMask);

            //ПОТОМ УДАЛИТЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! (рисует лучи)
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            //если есть столкновение
            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }
                //угол наклона препятствия
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //спуск по наклонной
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
                        velocity = collisionInfo.velocityOld;
                    }
                }
                //если по препятствию можно ходить
                if (i == 0  && slopeAngle <= maxSlopeAngle)
                {


                    //двигает игрока вплотную к наклонной поверхности
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    Climb(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }
                if (!collisionInfo.onSlope || slopeAngle > maxSlopeAngle)
                {
                    //двигает игрока
                    velocity.x = (hit.distance - skinWidth) * directionX;

                    //изменение длинны луча чтобы не пройти через неровную поверхность
                    rayLength = hit.distance;

                    //убирает застревание в объектах стоящих чуть выше наклонных поверхностей на наклонных поверхностях
                    if (collisionInfo.onSlope)
                    {
                        velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    //изменяет данные о столкновениях
                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }
            }
        }
    }
    //проверка столкновения с полом и потолком
    public void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i<verticalRayCount; i++)
        {
            Vector2 rayOrigin = ( directionY == -1 )? collideCorners.bottomLeft:collideCorners.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, cMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;

                //изменение длинны луча чтобы не пройти через неровную поверхность
                rayLength = hit.distance;

                //убирает застревание в потолке на наклонных поверхностях
                if (collisionInfo.onSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }


                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;
            }
        }

        if (collisionInfo.onSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? collideCorners.bottomLeft : collideCorners.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, cMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }


    //поднимание по наклонной поверхности
    void Climb(ref Vector2 velocity, float slopeAngle)
    {
        //создает скорость по y
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

        //если прыгаем
        if(velocity.y <= climbVelocityY) 
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * velocity.x;
            collisionInfo.below = true;
            collisionInfo.onSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }    
    }
    
    void DescendSlope(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? collideCorners.bottomRight : collideCorners.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, cMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxSlopeAngleDown)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float downVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * velocity.x;
                        velocity.y -= downVelocityY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.below = true;
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

    //хранение информации о столкновениях
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool onSlope;
        public bool descendingSlope;

        public float slopeAngle, slopeAngleOld;

        public Vector2 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            onSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
