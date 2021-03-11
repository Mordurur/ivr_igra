using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour
{
    [HideInInspector] public float maxHealth;
    private float health;
    // переменные для инспектора юнити
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float floatSpeed = 0f;
    [SerializeField] private float dashSpeed = 0f;
    [SerializeField] private float crouchSpeed = 0f;
    [SerializeField] private float maxDashTime = 0f;
    [SerializeField] private float timeBetweenDashes = 0f;

    // для физики прыжков
    [SerializeField] private float maxJumpHeight = 0f;
    [SerializeField] private float timeToJumpApex = 0f;

    [SerializeField] LayerMask enemyAttack;

    //оружие
    private WeaponController weapon;
    private Renderer weaponRenderer;
    private UltController ult;
    Collider2D collider2;

    //переменные для анимации
    private bool facingRight = true;

    // контроллер2д
    public Controller2D controller;

    float stamina = 10;

    public Slider sliderHP;
    public Slider sliderSTA;
    public float skill1modifier;
    public float skill2modifier;
    public float skill3modifier;

    //аниматор
    private Animator animator;

    // физические велечины
    private float jumpForce;
    private float gravity;

    //переменные для прыжков и рывка
    private bool canDoubleJump = false;
    private float doubleJumpTimer = 0;
    private bool floatGravity = false;

    private bool isDashing = false;
    private float dashTime = 0;
    private float sinceLastDash = 0;

    private bool onGround = false;
    private float timeInAir = 0;

    private float jumpTimer = 0f;
    private bool isJumping = false;

    private bool isCrouching = false;

    // для более точной физики прыжков
    Vector2 velocity;
    Vector2 prevVelocity;

    private bool canBeDamaged = true;

    private SpriteRenderer spriteRenderer1;

    void Start()
    {
        sinceLastDash = timeBetweenDashes;
        controller = GetComponent<Controller2D>();
        animator = GameObject.Find("PlayerRenderer").GetComponent<Animator>();
        spriteRenderer1 = GameObject.Find("PlayerRenderer").GetComponent<SpriteRenderer>();

        weaponRenderer = GameObject.Find("WeaponRenderer").GetComponent<Renderer>();
        weapon = weaponRenderer.GetComponent<WeaponController>();
        collider2 = GetComponent<Collider2D>();

        ult = GetComponent<UltController>();
        //физика кинематика
        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);

        jumpForce = 2 * maxJumpHeight / timeToJumpApex;
        health = maxHealth;
        StartCoroutine(RestoreStamina());

    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        bool onGround2 = timeInAir < 0.02f;
        if (!onGround)
        {
            timeInAir += Time.deltaTime;
        }

        if (GameMaster.enabledMovement && (Input.GetKey(KeyCode.DownArrow) || InputMixer.CrouchS))
        {
            if (!isCrouching && !isJumping)
            {
                isCrouching = true;
                controller.Crouch(true);
            }
        }
        else if (isCrouching && controller.CanStand())
        {
            isCrouching = false;
            controller.Crouch(false);
        }
        
        if (GameMaster.enabledMovement && (Input.GetButtonDown("Jump") || InputMixer.JumpDownS) && !isCrouching)
        {
            if (onGround)
            {
                doubleJumpTimer = 0;
                Jump();
            }
            else if (canDoubleJump)
            {
                velocity.y = 0;
                canDoubleJump = false;
                Jump();
            }
        }
        if (GameMaster.enabledMovement && (Input.GetButtonUp("Jump") || InputMixer.JumpUpS) && isJumping)
        {
            velocity.y *= 0.4f;
            isJumping = false;
        }
        if (jumpTimer < timeToJumpApex)
        {
            jumpTimer += Time.deltaTime;
        }
        if (jumpTimer >= timeToJumpApex)
        {
            isJumping = false;
        }
        if (GameMaster.enabledMovement && (Input.GetButton("Jump") || InputMixer.JumpS) && !onGround && !isDashing && !isJumping && !isCrouching)
        {
            doubleJumpTimer += Time.deltaTime;
            if (doubleJumpTimer > 0.1f)
            {
                floatGravity = true;
            }
        }
        else
        {
            doubleJumpTimer = 0;
            floatGravity = false;
        }
        Vector2 input;
        if (GameMaster.enabledMovement)
        {
            input = new Vector2(Mathf.Clamp(Input.GetAxisRaw("Horizontal") + InputMixer.HorizontalS, -1, 1), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            input = new Vector2(0, 0);
        }

        //совршить рывок
        if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.LeftShift) || InputMixer.DashDownS) && sinceLastDash >= timeBetweenDashes && !isCrouching && stamina > 0)
        {
            isDashing = true;
            stamina -= 1;
            StartCoroutine(DecreaseSlider(sliderSTA, .2f, stamina / 10));
        }
        if (!isDashing)
        {
            sinceLastDash += Time.deltaTime;
        }
        else
        {
            sinceLastDash = 0;
        }
        if (isCrouching)
        {
            velocity.x = input.x * crouchSpeed;
        }
        else if (isDashing && dashTime <= maxDashTime)
        {
            dashTime += Time.deltaTime;
            velocity.x = (input.x != 0)?(input.x * dashSpeed):((facingRight)?(dashSpeed):(-dashSpeed));
        }
        else
        {
            velocity.x = input.x * moveSpeed;
            isDashing = false;
            dashTime = 0;
        }
        if (!isCrouching && (Input.GetKeyDown(KeyCode.C) || InputMixer.Skill3DownS))
        {
            ult.Ultimate(facingRight, skill3modifier);
        }

        if (onGround && !isCrouching && !isDashing && weapon.CanWalk())
        {
            if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Z) || InputMixer.Skill1DownS) && stamina > 0)
            {
                weapon.Hit1(facingRight, skill1modifier);
                stamina -= 1;
                StartCoroutine( DecreaseSlider(sliderSTA, .2f, stamina / 10));
            }
            else if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.X) || InputMixer.Skill2DownS) && stamina > 0)
            {
                weapon.Hit2(facingRight, skill2modifier);
                stamina -= 2;
                StartCoroutine(DecreaseSlider(sliderSTA, .2f, stamina / 10));
            }
        }

        if (!weapon.CanWalk())
        {
            velocity.x = 0;
        }

        if (velocity.x != 0 || isCrouching || isDashing || !onGround)
        {
            weaponRenderer.enabled = false;
        }
        else
        {
            weaponRenderer.enabled = true;
        }



        animator.SetBool("isMoving", velocity.x != 0);
        animator.SetBool("floatGravity", floatGravity);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("onGround", onGround2);
    }

    void FixedUpdate()
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
        //плавное падение
        if (floatGravity)
        {
            velocity.y = -100 * floatSpeed * Time.fixedDeltaTime;
        }
        else if (isDashing)
        {
            velocity.y = 0;
        }
        else
        {
            if (isJumping)
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }
            else
            {
                velocity.y += gravity * 1.5f * Time.fixedDeltaTime;

            }
        }
        


        Vector2 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        
        if (!GameMaster.stagnate)
        {
            controller.Move(deltaPosition);
        }
        onGround = controller.collisionInfo.below;
        // Убирает накопление гравитации и столкновений
        if (onGround)
        {
            canDoubleJump = true;
            velocity.y = 0;
            doubleJumpTimer = 0;
            isJumping = false;
            timeInAir = 0;
        }
        else if (controller.collisionInfo.above)
        {
            velocity.y = 0;
        }
        if (controller.collisionInfo.left || controller.collisionInfo.right)
        {
            velocity.x = 0;
        }

        if (Physics2D.OverlapBox(collider2.bounds.center, collider2.bounds.size, 0f, enemyAttack))
        {
            Damage(1);
        }
    }

    IEnumerator RestoreStamina()
    {
        while (health > 0)
        {
            if (stamina < 10)
            {
                stamina += 1;
                StartCoroutine(DecreaseSlider(sliderSTA, .1f, stamina / 10));
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void Damage(float dmg)
    {
        if (dmg < 0)
        {
            if (health - dmg <= maxHealth)
            {
                health -= dmg;
            }
            StartCoroutine(DDDamaged(true));
            return;
        }
        if (!canBeDamaged)
        {
            return;
        }
        health -= dmg;
        StartCoroutine(CanBeDamaged(1f));
        StartCoroutine(DDDamaged());
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        
    }

    private IEnumerator DecreaseSlider(Slider slider, float duration, float goal)
    {
        float elapsed = 0f;
        float startValue = slider.value;
        while (elapsed < 0.5f)
        {
            slider.value = Mathf.Lerp(startValue, goal, elapsed / duration);
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator DDDamaged(bool heal = false)
    {
        StartCoroutine(DecreaseSlider(sliderHP, .2f, health / maxHealth));
        if (heal)
        {
            spriteRenderer1.color = Color.green;
        }
        else
        {
            spriteRenderer1.color = Color.red;
        }
        yield return new WaitForSeconds(.2f);
        if (health > 0)
            spriteRenderer1.color = Color.white;
    }

    public IEnumerator CanBeDamaged(float invTime)
    {
        canBeDamaged = false;

        yield return new WaitForSeconds(invTime);

        canBeDamaged = true;
    }

    public IEnumerator Die()
    {
        foreach(Animator spriteRenderer in GetComponentsInChildren<Animator>())
        {
            spriteRenderer.enabled = false;
        }
        this.enabled = false;
        StopCoroutine(DDDamaged());
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = Color.black;
        }
        yield return new WaitForSeconds(1f);
        GameMaster.enabledMovement = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Jump()
    {
        velocity.y = jumpForce;
        jumpTimer = 0;
        isJumping = true;
    }
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    public void GiveInvince(float tim)
    {
        StartCoroutine(CanBeDamaged(tim));
    }
}