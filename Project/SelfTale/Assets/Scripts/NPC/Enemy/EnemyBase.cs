using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 0f;
    [SerializeField] protected float attackDamage = 0f;
    [SerializeField] protected LayerMask isDamagable;
    [SerializeField] protected int goldReward;
    [SerializeField] protected int arm;
    [SerializeField] protected bool kbackres;
    [SerializeField] protected float flipWait;

   protected float kback;
    public float health;
    protected SpriteRenderer spriteRenderer;

    protected Controller2D controller;
    protected Animator animator;

    protected GameObject playerChar;

    protected PhaseController phaseController;

    protected bool facingRight = true;

    protected bool canWalk = true;

    protected Vector2 velocity;
    protected Vector2 prevVelocity;

    protected float hitTimer;

    

    protected virtual void Start()
    {
        health = maxHealth;
        arm = Mathf.Clamp(arm, 0, 100);
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        phaseController = FindObjectOfType<PhaseController>().GetComponent<PhaseController>();

        playerChar = GameObject.Find("Player");
        if (transform.localScale.x < 0 && facingRight)
        {
            facingRight = false;
        }
    }

    protected virtual void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    protected virtual void FixedUpdate()
    {
        velocity.x += kback;
        prevVelocity = velocity;
        Vector2 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        velocity.x = 0;
        controller.Move(deltaPosition);
    }

    public virtual void Damage(float damageValue, float defPierce, float knockBack)
    {
        health -= damageValue * (100 - arm + defPierce) / 100;
        if (!kbackres)
            StartCoroutine(ChangeSpeed(knockBack, 0, 0.2f));
        spriteRenderer.color = Color.red;
        hitTimer = 0.2f;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public virtual IEnumerator Die()
    {
        spriteRenderer.color = Color.black;
        canWalk = false;
        yield return new WaitForSeconds(0.3f);
        phaseController.kills++;
        phaseController.goldEarned += goldReward;
        Destroy(gameObject);
    }

    public virtual void Flip()
    {
        StartCoroutine(FlipWait());
    }

    public virtual IEnumerator FlipWait()
    {
        canWalk = false;
        yield return new WaitForSeconds(flipWait);
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        canWalk = true;
    }

    IEnumerator ChangeSpeed(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            kback = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        kback = v_end;
    }
}


