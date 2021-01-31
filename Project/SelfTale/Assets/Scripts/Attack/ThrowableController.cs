using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : WeaponController
{

    public override void Hit1(bool faceRight,float dmgmod)
    {
        if (hit1Timer > 0)
        {
            return;
        }
        hit1Timer = hit1MaxTimer;
        attackTimer = 0.250f;
        weaponAnimator.SetTrigger("hit1");
        playerAnimator.SetTrigger("attack3");
        StartCoroutine(WaitBeforeSpawn1(0.25f, faceRight, dmgmod));
    }
    private void Hit22(bool faceRight, Vector3 offset1, float flySpeed1, float dmgmod)
    {
        GameObject bullet = Instantiate(spObject, gameObject.transform.position + offset1, gameObject.transform.rotation);
        if (!faceRight)
        {
            Vector2 theScale = bullet.transform.localScale;
            theScale.x *= -1;
            bullet.transform.localScale = theScale;
            bullet.GetComponent<MovingAttack>().SetMovingAttack(damage * dmgmod,flySpeed1,bulletLife,runtimeAnimatorController,new float[] { 1, 0, 1, 0.25f }, isEnemy,-1);
        }
        else
        {
            bullet.GetComponent<MovingAttack>().SetMovingAttack(damage * dmgmod,flySpeed1, bulletLife, runtimeAnimatorController, new float[] { 1, 0, 1, 0.25f }, isEnemy, 1);
        }
    }


    public override void Hit2(bool faceRight,float dmgmod)
    {
        if (hit2Timer > 0)
        {
            return;
        }
        hit2Timer = hit2MaxTimer;
        attackTimer = 0.250f;
        weaponAnimator.SetTrigger("hit2");
        playerAnimator.SetTrigger("attack4");
        StartCoroutine(WaitBeforeSpawn2(0.1f, faceRight, dmgmod));
    }

    IEnumerator WaitBeforeSpawn1(float delaySpawn, bool faceRight, float dmgmod)
    {
        yield return new WaitForSeconds(delaySpawn);
        Hit22(faceRight, new Vector2(0, 0), flySpeed * 1.5f, dmgmod);
    }
    IEnumerator WaitBeforeSpawn2(float delaySpawn, bool faceRight, float dmgmod)
    {
        yield return new WaitForSeconds(delaySpawn);
        Hit22(faceRight, new Vector2((faceRight) ? 0.5f : -0.5f, 0), flySpeed, dmgmod);
        Hit22(faceRight, new Vector2(0, -0.5f), flySpeed, dmgmod);
        Hit22(faceRight, new Vector2(0, 0.5f), flySpeed, dmgmod);
    }
}
