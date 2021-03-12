using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnifer : EnemyBase
{
    [SerializeField] int enemyType = 0;

    private float gravity = -10;

    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float attackSpeed = 0f;


    private bool onGround = false;

    private float aggroTimer = 0f;

    public float maxAttackTimer = 0f;
    private float attackTimer = 0f;

    [SerializeField] float hitRangeMin;
    [SerializeField] float hitRangeMax;

    [SerializeField] GameObject attackPrefab;

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

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        //враг видит игрока
        Vector2 eyesPos = controller.collide.bounds.center + new Vector3(0, controller.collide.bounds.size.y / 4, 0);

        RaycastHit2D hit = Physics2D.Raycast(eyesPos, Vector2.right * ((facingRight) ? 1 : -1), 10, isDamagable);

        if (hit || (playerChar.transform.position - transform.position).magnitude <= 5)
        {
            aggroTimer = 2f;

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


        if (Mathf.Abs(dist) < hitRangeMax && attackTimer <= 0 && canWalk && aggroTimer > 0)
        {
            bool doFlip = false;
            if(!facingRight && Mathf.Sign(dist) > 0)
            {
                doFlip = true;
            }
            else if (facingRight && Mathf.Sign(dist) < 0)
            {
                doFlip = true;
            }
            StartCoroutine(AttackFast(attackDamage, doFlip));
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

    IEnumerator AttackFast(float dmg, bool doFlip)
    {
        attackTimer = maxAttackTimer;
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
            GameObject k1 = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            GameObject k2 = Instantiate(attackPrefab, transform.position, Quaternion.Euler(0, 0, 15));
            GameObject k3 = Instantiate(attackPrefab, transform.position, Quaternion.Euler(0, 0, -15));
            allKnifes.Add(k1);
            allKnifes.Add(k2);
            allKnifes.Add(k3);

            if (!facingRight)
            {
                foreach (GameObject k in new GameObject[] {k1,k2,k3 })
                {
                    Vector3 theScale = k.transform.localScale;
                    theScale.x *= -1;
                    k.transform.localScale = theScale;
                    k.transform.rotation = Quaternion.Euler(0, 0, k.transform.rotation.eulerAngles.z * -1);
                }
            }
            Vector2 velocity1 = new Vector2((facingRight)?1:-1 , 0) * attackSpeed;
            Vector2 velocity2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 15) * ((facingRight) ? 1 : -1), Mathf.Sin(Mathf.Deg2Rad * 15)) * attackSpeed;
            Vector2 velocity3 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * -15) * ((facingRight) ? 1 : -1), Mathf.Sin(Mathf.Deg2Rad * -15)) * attackSpeed;
            StartCoroutine(MovingKnife(5, velocity1, k1));
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(MovingKnife(5, velocity2, k2));
            StartCoroutine(MovingKnife(5, velocity3, k3));
            yield return new WaitForSeconds(0.3f);

        }
        else if (enemyType == 1)
        {
            GameObject k1 = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            GameObject k2 = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            GameObject k3 = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            GameObject k4 = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            allKnifes.Add(k1);
            allKnifes.Add(k2);
            allKnifes.Add(k3);
            allKnifes.Add(k4);
            foreach (GameObject k in new GameObject[] { k3, k4})
            {
                Vector3 theScale = k.transform.localScale;
                theScale.x *= -1;
                k.transform.localScale = theScale;
            }
            Vector2 velocity1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 20), Mathf.Sin(Mathf.Deg2Rad * 20)) * attackSpeed;
            Vector2 velocity2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * -20), Mathf.Sin(Mathf.Deg2Rad * -20)) * attackSpeed;
            Vector2 velocity3 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * -20) * -1, Mathf.Sin(Mathf.Deg2Rad * -20)) * attackSpeed;
            Vector2 velocity4 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 20) * -1, Mathf.Sin(Mathf.Deg2Rad * 20)) * attackSpeed;
            StartCoroutine(MovingKnife(5, velocity1, k1));
            StartCoroutine(MovingKnife(5, velocity2, k2));
            StartCoroutine(MovingKnife(5, velocity3, k3));
            StartCoroutine(MovingKnife(5, velocity4, k4));
            yield return new WaitForSeconds(0.6f);
        }

        canWalk = true;
    }


    IEnumerator MovingKnife(float duration, Vector3 velocity, GameObject moveObject)
    {
        
        if (moveObject == null)
        {
            yield break;
        }
        BoxCollider2D collider2 = moveObject.GetComponent<BoxCollider2D>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (moveObject == null)
            {
                yield break;
            }
            moveObject.transform.position += velocity * Time.deltaTime;
            if (Physics2D.OverlapBox(collider2.bounds.center, collider2.bounds.size, 0f, controller.cMask))
            {
                Destroy(moveObject);
                allKnifes.Remove(moveObject);
                
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(moveObject);
        allKnifes.Remove(moveObject);
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
