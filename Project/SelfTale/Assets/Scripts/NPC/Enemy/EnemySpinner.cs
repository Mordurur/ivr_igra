using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpinner : EnemyBase
{
    private float gravity = -10;

    private Vector2 velocityOld = Vector2.zero;

    [SerializeField] private float moveSpeed = 0f;

    private bool onGround = false;

    private float aggroTimer = 0f;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        float dist = playerChar.transform.position.x - transform.position.x;


        if (aggroTimer > 0)
        {
            aggroTimer -= Time.deltaTime;
        }

        //враг видит игрока
        Vector2 eyesPos = controller.collide.bounds.center - new Vector3(0, controller.collide.bounds.size.y / 4, 0);

        RaycastHit2D hit = Physics2D.Raycast(eyesPos, Vector2.right * ((facingRight) ? 1 : -1), 10, isDamagable);

        if (hit || (playerChar.transform.position - transform.position).magnitude <= 5)
        {
            aggroTimer = 1.5f;
        }


        if (aggroTimer > 0)
        {
            velocity.x = moveSpeed * Mathf.Sign(dist);
        }
        else
        {
            velocity.x = 0;
        }

        animator.SetBool("walking", Mathf.Abs(velocity.x) == moveSpeed && canWalk);

    }

    protected override void FixedUpdate()
    {
        if (facingRight && velocity.x < 0 && canWalk)
        {
            Flip();
        }
        else if (!facingRight && velocity.x > 0 && canWalk)
        {
            Flip();
        }
        if (canWalk)
        {
            velocityOld = velocity;
        }
        else
        {
            velocity.x = velocityOld.x;
        }
        prevVelocity = velocity;


        velocity.y += gravity * 1.5f * Time.fixedDeltaTime;
        velocity.x += kback;

        Vector2 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        kback = 0;
        controller.Move(deltaPosition);
        onGround = controller.collisionInfo.below;
        // Убирает накопление гравитации и столкновений
        if (onGround)
        {
            velocity.y = 0;
        }
        else if (controller.collisionInfo.above)
        {
            velocity.y = 0;
        }
        if (controller.collisionInfo.left || controller.collisionInfo.right)
        {
            velocity.x = 0;
        }
    }
}
