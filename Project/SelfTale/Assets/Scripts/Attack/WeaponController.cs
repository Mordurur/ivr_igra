using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    protected float attackTimer = 0f;

    protected float damage;
    protected float armPierce;
    protected float flySpeed;
    protected float kBack;
    protected float bulletLife;
    protected GameObject spObject;
    protected RuntimeAnimatorController runtimeAnimatorController;

    [SerializeField] protected LayerMask isEnemy = 0;


    protected float hit1Timer = 0f;
    protected float hit1MaxTimer = 1f;
    protected float hit2Timer = 0f;
    protected float hit2MaxTimer = 2f;

    protected Animator weaponAnimator;
    protected Animator playerAnimator;

    protected BoxCollider2D weaponCollider;

    private bool prevState = false;
    private bool nowState = false;
    // Start is called before the first frame update
    void Start()
    {
        weaponAnimator = GameObject.Find("WeaponRenderer").GetComponent<Animator>();

        playerAnimator = GameObject.Find("PlayerRenderer").GetComponent<Animator>();

        weaponCollider = GameObject.Find("WeaponRenderer").GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        prevState = nowState;
        nowState = playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if (hit1Timer > 0)
        {
            hit1Timer -= Time.deltaTime;
        }
        if (hit2Timer > 0)
        {
            hit2Timer -= Time.deltaTime;
        }
        if(nowState && !prevState)
        {
            weaponAnimator.SetTrigger("idleTrigger");
        }
        //DrawHitbox();
    }
    public bool CanWalk()
    {
        if (attackTimer <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Hit1(bool faceRight, float dmgmod)
    {
        
    }
    public virtual void Hit2(bool faceRight, float dmgmod)
    {
        
    }

    public void SetWeapon(WeaponData weaponInfo)
    {
        damage = weaponInfo.weaponDamage;
        armPierce = weaponInfo.defencePierce;
        flySpeed = weaponInfo.flySpeed;
        kBack = weaponInfo.knockBack;
        spObject = weaponInfo.spObject;
        runtimeAnimatorController = weaponInfo.animatorController;
        bulletLife = weaponInfo.bulletLife;
        GetComponent<Animator>().runtimeAnimatorController = Resources.Load("WeaponAnimators/" + weaponInfo.weaponAnimatorName) as RuntimeAnimatorController;
    }


    //public void DrawHitbox()
    //{
    //    Bounds bounds = weaponCollider.bounds;

    //    Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
    //    Vector2 bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    //    Vector2 topLeft = new Vector2(bounds.min.x, bounds.max.y);
    //    Vector2 topRight = new Vector2(bounds.max.x, bounds.max.y);

    //    Debug.DrawLine(bottomRight, topRight, Color.magenta);
    //    Debug.DrawLine(bottomRight, bottomLeft, Color.magenta);
    //    Debug.DrawLine(topLeft, bottomLeft, Color.magenta);
    //    Debug.DrawLine(topLeft, topRight, Color.magenta);
    //}
}
