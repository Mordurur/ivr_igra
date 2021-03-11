using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOneMelee : EnemyBase
{
    private float gravity = -10;


    [SerializeField] private float moveSpeed = 0f;

    private bool onGround = false;

    private float aggroTimer = 0f;

    public float maxAttackTimer = 0f;
    private float attackTimer = 0f;

    [SerializeField] Vector2 attackCenter1;
    [SerializeField] Vector2 attackSize1;

    Vector2 attackCenter;
    Vector2 attackSize;


    protected override void Start()
    {
        base.Start();
        
        attackSize = attackSize1;
    }

    protected override void Update()
    {
        base.Update();
        float dist = playerChar.transform.position.x - transform.position.x;

        attackCenter = controller.collide.bounds.center + new Vector3(attackCenter1.x * ((facingRight) ? 1 : -1), attackCenter1.y);

        if (aggroTimer > 0)
        {
            aggroTimer -= Time.deltaTime;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        //враг видит игрока
        Vector2 eyesPos = controller.collide.bounds.center + new Vector3(0, controller.collide.bounds.size.y / 4, 0);

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

        if (Mathf.Abs(dist) < attackSize1.x / 2 + 0.1f)
        {
            velocity.x = 0;
            if (attackTimer <= 0 && canWalk)
            {
                if (Physics2D.OverlapBox(attackCenter, attackSize, 0f, isDamagable))
                {
                    StartCoroutine(AttackFast(attackDamage));
                }
            }
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
        prevVelocity = velocity;


        velocity.y += gravity * 1.5f * Time.fixedDeltaTime;
        velocity.x += kback;

        if (!canWalk)
        {
            velocity.x = 0;
            prevVelocity.x = 0;
        }
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

    IEnumerator AttackFast(float dmg)
    {
        attackTimer = maxAttackTimer;
        animator.SetTrigger("attack1");
        canWalk = false;
        yield return new WaitForSeconds(0.3f);
        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0f, isDamagable);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<Player>().Damage(dmg);
        }
        yield return new WaitForSeconds(0.075f);
        yield return new WaitForSeconds(0.7f - 0.075f);
        canWalk = true;
    }
}
