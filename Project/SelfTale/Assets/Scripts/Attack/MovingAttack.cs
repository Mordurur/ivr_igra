using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAttack : MonoBehaviour
{
    public int directRight = 1;
    public float damage;
    public float flySpeed;
    public float lifeTime;

    private BoxCollider2D weaponCollider;

    private float destroyTimer = 0;

    [SerializeField] LayerMask isEnemy;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        destroyTimer += Time.fixedDeltaTime;
        if(destroyTimer > lifeTime)
        {
            Destroy(gameObject);
        }
        transform.Translate(flySpeed * transform.right * directRight * Time.fixedDeltaTime);
        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(weaponCollider.bounds.center, weaponCollider.bounds.size, 0f, isEnemy);

        foreach (Collider2D doDamage in currentCollisions)
        {
            EnemyBase enemy = doDamage.GetComponent<EnemyBase>();
            if (enemy)
            {
                enemy.Damage(damage, 0, 0);
            }
            Destroy(gameObject);

        }
    }

    public void SetMovingAttack(float damage, float flySpeed, float lifeTime, RuntimeAnimatorController runtimeAnimator, float[] colliderProperties, LayerMask layer, int directRight = 1)
    {
        this.directRight = directRight;
        this.damage = damage;
        this.flySpeed = flySpeed;
        this.lifeTime = lifeTime;
        isEnemy = layer;
        GetComponent<Animator>().runtimeAnimatorController = runtimeAnimator;
        weaponCollider.offset = new Vector2(colliderProperties[0], colliderProperties[1]);
        weaponCollider.size = new Vector2(colliderProperties[2], colliderProperties[3]);
    }
}
