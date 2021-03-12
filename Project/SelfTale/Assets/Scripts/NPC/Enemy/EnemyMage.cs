using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMage : EnemyBase
{
    [SerializeField] int enemyType = 0;

    private float gravity = -10;

    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float attackSpeed = 0f;


    private bool onGround = false;

    private float aggroTimer = 0f;

    public float maxAttackTimer1 = 0f;
    public float maxAttackTimer2 = 0f;
    private float attackTimer1 = 0f;
    private float attackTimer2 = 0f;

    [SerializeField] float hitRangeMin;
    [SerializeField] float hitRangeMax;

    [SerializeField] GameObject attackPrefab1;
    [SerializeField] GameObject attackPrefab2;

    private List<GameObject> allKnifes = new List<GameObject>();

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

        if (attackTimer1 > 0)
        {
            attackTimer1 -= Time.deltaTime;
        }

        if (attackTimer2 > 0)
        {
            attackTimer2 -= Time.deltaTime;
        }

        //враг видит игрока
        Vector2 eyesPos = controller.collide.bounds.center + new Vector3(0, controller.collide.bounds.size.y / 4, 0);

        RaycastHit2D hit = Physics2D.Raycast(eyesPos, Vector2.right * ((facingRight) ? 1 : -1), 10, isDamagable);

        if (hit || (playerChar.transform.position - transform.position).magnitude <= 5)
        {
            aggroTimer = 3f;
        }

        if (aggroTimer > 0)
        {

            if (Mathf.Abs(dist) > hitRangeMax)
            {
                velocity.x = moveSpeed * Mathf.Sign(dist);
            }
            else if (Mathf.Abs(dist) < hitRangeMin)
            {
                velocity.x = -moveSpeed * Mathf.Sign(dist);
            }
            else
            {
                velocity.x = 0;
            }
        }
        else
        {
            velocity.x = 0;
        }

        bool doFlip = false;
        if (!facingRight && Mathf.Sign(dist) > 0)
        {
            doFlip = true;
        }
        else if (facingRight && Mathf.Sign(dist) < 0)
        {
            doFlip = true;
        }

        if (Mathf.Abs(dist) < hitRangeMax && attackTimer2 <= 0 && canWalk && aggroTimer > 0)
        {
            StartCoroutine(Attack2(attackDamage, doFlip));
        }
        else if (Mathf.Abs(dist) < hitRangeMax && attackTimer1 <= 0 && canWalk && aggroTimer > 0)
        {
            StartCoroutine(Attack1(attackDamage, doFlip));
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

    IEnumerator Attack1(float dmg, bool doFlip)
    {
        attackTimer1 = maxAttackTimer1;
        canWalk = false;
        if (doFlip)
        {
            yield return new WaitForSeconds(flipWait);
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        animator.SetTrigger("attack1");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enemyType == 0)
        {
            for (int i = -2; i <= 2; i++)
            {
                GameObject k = Instantiate(attackPrefab2, transform.position, Quaternion.identity);
                if (!facingRight)
                {
                    Vector3 theScale = k.transform.localScale;
                    theScale.x *= -1;
                    k.transform.localScale = theScale;
                }
                Vector2 velocity1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 10*i) * ((facingRight) ? 1 : -1), Mathf.Sin(Mathf.Deg2Rad * 10*i)) * attackSpeed;
                StartCoroutine(MovingKnife(5, velocity1, k));
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.6f);

        }
        

        canWalk = true;
    }

    IEnumerator Attack2(float dmg, bool doFlip)
    {
        attackTimer2 = maxAttackTimer2;
        canWalk = false;
        if (doFlip)
        {
            yield return new WaitForSeconds(flipWait);
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        animator.SetTrigger("attack2");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enemyType == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject k = Instantiate(attackPrefab2, transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * 45 * i) * ((facingRight) ? 1 : -1),
                    Mathf.Sin(Mathf.Deg2Rad * 45 * i)) * 4, Quaternion.identity);
                k.transform.parent = transform;
                StartCoroutine(RotateAroundKnife(5, attackSpeed, k));
            }
            yield return new WaitForSeconds(0.6f);

        }


        canWalk = true;
    }

    IEnumerator RotateAroundKnife(float duration, float speed, GameObject moveObject)
    {
        allKnifes.Add(moveObject);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (moveObject == null)
            {
                yield break;
            }
            moveObject.transform.RotateAround(transform.position, new Vector3(0, 0, 1), speed * Time.deltaTime * 10);
            moveObject.transform.rotation = Quaternion.identity;
            //moveObject.transform.position += velocity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        allKnifes.Remove(moveObject);
        Destroy(moveObject);
    }

    IEnumerator MovingKnife(float duration, Vector3 velocity, GameObject moveObject)
    {
        allKnifes.Add(moveObject);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (moveObject == null)
            {
                yield break;
            }
            moveObject.transform.position += velocity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        allKnifes.Remove(moveObject);
        Destroy(moveObject);
    }

    public override IEnumerator Die()
    {
        foreach (GameObject knife in allKnifes)
        {
            if (knife != null)
            {
                Destroy(knife);
            }
        }
        return base.Die();
    }
}
