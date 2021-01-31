using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EnemyBase
{ 

    private float jumpForce;
    private float gravity;


    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float maxJumpHeight = 0f;
    [SerializeField] private float timeToJumpApex = 0f;

    private bool onGround = false;
    private float jumpTimer = 0f;
    private bool isJumping = false;

    private float aggroTimer = 0f;

    public float maxAttackTimer = 0f;
    public float maxAttackTimer2 = 0f;
    private float attackTimer = 0f;
    private float attackTimer2 = 0f;

    [SerializeField] Vector2 attackCenter1;
    [SerializeField] Vector2 attackSize1;

    [SerializeField] Vector2 attackCenter21;
    [SerializeField] Vector2 attackSize21;

    Vector2 attackCenter;
    Vector2 attackSize;

    Vector2 attackCenter2;
    Vector2 attackSize2;


    protected override void Start()
    {
        base.Start();

        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);
        jumpForce = 2 * maxJumpHeight / timeToJumpApex;
        attackSize = attackSize1;
        attackSize2 = attackSize21;
    }

    protected override void Update()
    {
        base.Update();


        attackCenter = controller.collide.bounds.center + new Vector3(attackCenter1.x * ((facingRight)?1:-1), attackCenter1.y);
        attackCenter2 = controller.collide.bounds.center + new Vector3(attackCenter21.x * ((facingRight) ? 1 : -1), attackCenter21.y);

        if (aggroTimer > 0)
        {
            aggroTimer -= Time.deltaTime;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if (attackTimer2 > 0)
        {
            attackTimer2 -= Time.deltaTime;
        }


        if (jumpTimer < timeToJumpApex)
        {
            jumpTimer += Time.deltaTime;
        }
        if (jumpTimer >= timeToJumpApex)
        {
            isJumping = false;
        }




        //враг видит игрока
        Vector2 eyesPos = controller.collide.bounds.center + new Vector3 (0, controller.collide.bounds.size.y / 4, 0);

        RaycastHit2D hit = Physics2D.Raycast(eyesPos, Vector2.right * ((facingRight) ? 1 : -1), 30, isDamagable);

        if (hit)
        {
            aggroTimer = 5;
        }
        if (aggroTimer > 0 && playerChar.transform.position.x > transform.position.x)
        {
            velocity.x = moveSpeed;
        }
        else if (aggroTimer > 0 && playerChar.transform.position.x < transform.position.x)
        {
            velocity.x = -moveSpeed;
        }
        else
        {
            velocity.x = 0;
        }

        if (Mathf.Abs(playerChar.transform.position.x - attackCenter2.x) < attackSize2.x / 2 + 0.15f)
        {
            velocity.x = 0;
            if (attackTimer2 <= 0)
            {
                if (Physics2D.OverlapBox(attackCenter2, attackSize2, 0f, isDamagable) && canWalk)
                {
                    StartCoroutine(AttackSlow(attackDamage * 2));
                }
            }
        }


        if (Mathf.Abs(playerChar.transform.position.x - attackCenter.x) < attackSize1.x / 2 + 0.15f)
        {
            if (attackTimer <= 0 && canWalk)
            {
                if (Physics2D.OverlapBox(attackCenter, attackSize, 0f, isDamagable))
                {
                    StartCoroutine(AttackFast(attackDamage));
                }
            }
        }

        animator.SetBool("walking", Mathf.Abs(velocity.x) == 1 && canWalk);

    }

    protected override void FixedUpdate()
    {
        if (facingRight && velocity.x < 0)
        {
            Flip();
        }
        else if (!facingRight && velocity.x > 0)
        {
            Flip();
        }

        prevVelocity = velocity;

        if (isJumping)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y += gravity * 1.5f * Time.fixedDeltaTime;

        }
        if (!canWalk)
        {
            velocity.x = 0;
            prevVelocity.x = 0;
        }
        Vector2 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;

        controller.Move(deltaPosition);
        onGround = controller.collisionInfo.below;
        // Убирает накопление гравитации и столкновений
        if (onGround)
        {
            velocity.y = 0;
            isJumping = false;
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

    private void Jump()
    {
        velocity.y = jumpForce;
        jumpTimer = 0;
        isJumping = true;
    }

    IEnumerator AttackFast(float dmg)
    {
        attackTimer = maxAttackTimer;
        attackTimer2 = maxAttackTimer;
        animator.SetTrigger("attack1");
        canWalk = false;
        yield return new WaitForSeconds(0.3f);
        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0f, isDamagable);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<Player>().Damage(dmg);
        }
        yield return new WaitForSeconds(0.075f);
        yield return new WaitForSeconds(0.5f - 0.075f);
        canWalk = true;
    }

    IEnumerator AttackSlow(float dmg)
    {
        attackTimer = maxAttackTimer;
        attackTimer2 = maxAttackTimer2;
        animator.SetTrigger("attack2");
        canWalk = false;
        yield return new WaitForSeconds(0.4f);
        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(attackCenter2, attackSize2, 0f, isDamagable);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<Player>().Damage(dmg);
        }
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(1f - 0.1f);
        canWalk = true;
    }
}
