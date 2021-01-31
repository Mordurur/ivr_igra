using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaberController : WeaponController
{
    public override void Hit1(bool faceRight, float dmgmod)
    {
        if (hit1Timer > 0)
        {
            return;
        }
        hit1Timer = hit1MaxTimer;
        attackTimer = 0.4f;
        weaponAnimator.SetTrigger("hit1");
        playerAnimator.SetTrigger("attack1");
        weaponCollider.size = new Vector2(1, 1);
        weaponCollider.offset = new Vector2(1, 0);

        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(weaponCollider.bounds.center, weaponCollider.bounds.size, 0f, isEnemy);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<EnemyBase>().Damage(damage * dmgmod, armPierce, faceRight? kBack: -kBack);
        }
    }
    public override void Hit2(bool faceRight, float dmgmod)
    {
        if (hit2Timer > 0)
        {
            return;
        }
        hit2Timer = hit2MaxTimer;
        attackTimer = 0.4f;
        weaponAnimator.SetTrigger("hit2");
        playerAnimator.SetTrigger("attack2");
        weaponCollider.size = new Vector2(1.5f, 2);
        weaponCollider.offset = new Vector2(1, 0);

        Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(weaponCollider.bounds.center, weaponCollider.bounds.size, 0f, isEnemy);

        foreach (Collider2D doDamage in currentCollisions)
        {
            doDamage.GetComponent<EnemyBase>().Damage(damage * 1.5f * dmgmod, armPierce, faceRight ? kBack : -kBack);
        }
    }
}
