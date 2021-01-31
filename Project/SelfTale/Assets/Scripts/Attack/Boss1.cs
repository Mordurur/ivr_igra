using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : EnemyBase
{
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject prefab3;
    [SerializeField] GameObject prefab2Summon;

    [SerializeField] GameObject canvass;

    UnityEngine.UI.Slider bossBar;

    [SerializeField] GameObject walls;

    float pdistance;

    int prevAttack = 0;
    bool battleStart = false;

    protected override void Start()
    {
        Flip();
        base.Start();
        bossBar = gameObject.GetComponentInChildren<UnityEngine.UI.Slider>();
        walls.SetActive(false);
        animator.enabled = false;

    }
    void NextAttack()
    {
        int next;
        if (prevAttack != 3)
        {
            next = 3;
        }
        else
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            next = Random.Range(0, 3);
        }
        prevAttack = next;
        if (next == 0)
        {
            StartCoroutine(A1());
        }
        else if (next == 1)
        {
            StartCoroutine(A2());
        }
        else
        {
            StartCoroutine(A3());
        }
    }

    protected override void Update()
    {
        base.Update();
        if (facingRight)
            canvass.transform.localScale = new Vector3(0.0625f, 0.0625f, 0.0625f);
        else
            canvass.transform.localScale = new Vector3(-0.0625f, 0.0625f, 0.0625f);
        pdistance = Mathf.Abs(transform.position.x - playerChar.transform.position.x);

        if (!battleStart && pdistance < 2)
        {
            walls.SetActive(true);
            battleStart = true;
            animator.enabled = true;
            NextAttack();
        }

        if (battleStart && pdistance > 5 && canWalk)
        {
            velocity = new Vector2(-200 * Mathf.Sign(transform.position.x - playerChar.transform.position.x), 0) * Time.deltaTime;
        }
        else
        {
            velocity.x = 0;
        }
        animator.SetBool("walking", Mathf.Abs(velocity.x) != 0 && canWalk);
    }

    protected override void FixedUpdate()
    {
        if (transform.position.x - playerChar.transform.position.x > 0 && facingRight)
        {
            Flip();
        }
        else if (transform.position.x - playerChar.transform.position.x <= 0 && !facingRight)
        {
            Flip();
        }
        prevVelocity = velocity;

        velocity.x += kback;
        prevVelocity = velocity;
        Vector2 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;

        controller.Move(deltaPosition);
    }

    IEnumerator A4(Vector3 pos)
    {
        GameObject boom = Instantiate(prefab2Summon, new Vector3(pos.x, 2.625f, 0), Quaternion.identity);
        boom.GetComponent<Animator>().Play(0);
        yield return new WaitForSeconds(0.5f);
        Destroy(boom);
    }

    IEnumerator MovingLazer(Vector3 velocity, GameObject moveObject)
    {
        bool tilFinish = true;
        while (tilFinish)
        {

            moveObject.transform.position += velocity * Time.deltaTime;
            if (Physics2D.OverlapCircle(moveObject.transform.position, 0.3503293f, controller.cMask))
            {
                Destroy(moveObject);
                StartCoroutine(A4(moveObject.transform.position));
                tilFinish = false;
            }
                
            yield return null;
        }
    }

    IEnumerator A1()
    {
        canWalk = false;
        animator.SetTrigger("a1");
        yield return new WaitForSeconds(1.167f);
        

        GameObject thisAttack = Instantiate(prefab1, gameObject.transform);
        GameObject thisAttack1 = Instantiate(prefab1, gameObject.transform);

        Vector3 theScale = thisAttack1.transform.localScale;
        theScale.x *= -1;
        thisAttack1.transform.localScale = theScale;

        thisAttack.GetComponent<Animator>().Play(0);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.15f, 0.3f, 10f);
        yield return new WaitForSeconds(.333333333f);

        Destroy(thisAttack);
        Destroy(thisAttack1);

        canWalk = true;
        while (pdistance > 5)
        {
            yield return null;
        }

        NextAttack();

    }
    IEnumerator A2()
    {
        canWalk = false;
        animator.SetTrigger("a2");

        yield return new WaitForSeconds(1.1667f);

        GameObject thisAttack1 = Instantiate(prefab2, playerChar.transform.position + new Vector3(0.5f, 4), Quaternion.identity);
        GameObject thisAttack2 = Instantiate(prefab2, playerChar.transform.position + new Vector3(-0.5f, 4), Quaternion.identity);
        thisAttack1.GetComponent<Animator>().Play(0);
        thisAttack2.GetComponent<Animator>().Play(0);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.1f, 0.05f, 3f);
        StartCoroutine(MovingLazer(new Vector3(0, -7, 0), thisAttack1));
        StartCoroutine(MovingLazer(new Vector3(0, -7, 0), thisAttack2));

        canWalk = true;
        yield return new WaitForSeconds(1f);


        while (pdistance > 5)
        {
            yield return null;
        }

        NextAttack();
    }
    IEnumerator A3()
    {
        canWalk = false;
        animator.SetTrigger("a3");
        yield return new WaitForSeconds(1.1667f);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(1f, 0.1f, 4f);
        GameObject thisAttack = Instantiate(prefab3, gameObject.transform);
        thisAttack.GetComponent<Animator>().Play(0);
        canWalk = true;
        yield return new WaitForSeconds(1.1667f);
        Destroy(thisAttack);

        while (pdistance > 5)
        {
            yield return null;
        }

        NextAttack();
    }

    public override void Damage(float damageValue, float defPierce, float knockBack)
    {
        base.Damage(damageValue, defPierce, knockBack);
        StartCoroutine(DecreaseSlider(bossBar, 0.3f, health / maxHealth));
    }

    private IEnumerator DecreaseSlider(UnityEngine.UI.Slider slider, float duration, float goal)
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

    public override IEnumerator Die()
    {
        float elapsed = 0.0f;
        StopCoroutine(A1());
        StopCoroutine(A2());
        StopCoroutine(A3());
        this.enabled = false;
        canWalk = false;
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.1f, 1f, 3f);
        while (elapsed < 5)
        {
            spriteRenderer.color = Color.Lerp(Color.white, Color.black, elapsed / 5);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        phaseController.DialogueAction(1337, 228);
        GameMaster.GM.progress.diamond += 1;
        phaseController.kills++;
        phaseController.goldEarned += goldReward;
        Destroy(gameObject);
    }
}
