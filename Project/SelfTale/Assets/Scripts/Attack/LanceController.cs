using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanceController : WeaponController
{
    public override void Hit1(bool faceRight, float dmgmod)
    {
        if (hit1Timer > 0)
        {
            return;
        }
        hit1Timer = hit1MaxTimer;
        attackTimer = 0.55f;
        weaponAnimator.SetTrigger("hit1");
        playerAnimator.SetTrigger("attack5");
        weaponCollider.size = new Vector2(3.5f, 1);
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
        StopAllCoroutines();
        StartCoroutine(HitSpin(faceRight,dmgmod));
    }

    private IEnumerator HitSpin(bool faceRight, float dmgmod)
    {
        hit2Timer = hit2MaxTimer;
        attackTimer = 1f;
        weaponAnimator.SetTrigger("hit2");
        playerAnimator.SetTrigger("attack5");
        weaponCollider.size = new Vector2(1.5f, 2);
        weaponCollider.offset = new Vector2(1, 0);

        for (int i = 0; i < 3; i++)
        {
            Collider2D[] currentCollisions = Physics2D.OverlapBoxAll(weaponCollider.bounds.center, weaponCollider.bounds.size, 0f, isEnemy);
            foreach (Collider2D doDamage in currentCollisions)
            {
                doDamage.GetComponent<EnemyBase>().Damage(damage * 1.5f *dmgmod, armPierce, (faceRight ? kBack : -kBack) * 2);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
