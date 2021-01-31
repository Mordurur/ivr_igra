using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : WeaponController
{
    public override void Hit1(bool faceRight, float dmgmod)
    {
        if (hit1Timer > 0)
        {
            return;
        }
        hit1Timer = hit1MaxTimer;
        attackTimer = 1f;
        weaponAnimator.SetTrigger("hit1");
        playerAnimator.SetTrigger("attack5");
        weaponCollider.size = new Vector2(2, 2);
        weaponCollider.offset = new Vector2(0, 0);

        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(weaponCollider.bounds.center, weaponCollider.bounds.size, 0f, isEnemy);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<EnemyBase>().Damage(damage * dmgmod, armPierce, kBack);
        }
    }
    public override void Hit2(bool faceRight, float dmgmod)
    {
        if (hit2Timer > 0)
        {
            return;
        }
        hit2Timer = hit2MaxTimer;
        attackTimer = 1;
        weaponAnimator.SetTrigger("hit2");
        playerAnimator.SetTrigger("attack5");
        weaponCollider.size = new Vector2(3, 2);
        weaponCollider.offset = new Vector2(0, 0);

        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(weaponCollider.bounds.center, weaponCollider.bounds.size, 0f, isEnemy);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<EnemyBase>().Damage(damage * dmgmod, armPierce, kBack * 2);
        }
    }
}
