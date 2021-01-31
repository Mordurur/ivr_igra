using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : EnemyBase
{
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject prefab3;

    UnityEngine.UI.Slider bossBar;

    [SerializeField] GameObject walls;

    float pdistance;

    int prevAttack = 1;
    bool battleStart = false;

    protected override void Start()
    {
        base.Start();
        bossBar = gameObject.GetComponentInChildren<UnityEngine.UI.Slider>();
        walls.SetActive(false);
        animator.enabled = false;

    }
    void NextAttack()
    {
        int next;
        if (prevAttack != 0)
        {
            next = 0; 
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
        else if(next == 1)
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
        pdistance = Mathf.Abs(transform.position.x - playerChar.transform.position.x);
        if (!battleStart && pdistance < 5)
        {
            walls.SetActive(true);
            battleStart = true;
            animator.enabled = true;
            NextAttack();
        }

        if (battleStart && pdistance > 3 && canWalk)
        {
            velocity = new Vector2(-300 * Mathf.Sign(transform.position.x - playerChar.transform.position.x), 0) * Time.deltaTime;
        }
        else
        {
            velocity.x = 0;
        }

    }

    protected override void FixedUpdate()
    {
        velocity.x += kback;
        prevVelocity = velocity;
        Vector2 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;

        controller.Move(deltaPosition);
    }

    IEnumerator MovingLazer(float duration, Vector3 velocity, GameObject moveObject)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {

            moveObject.transform.position += velocity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator A1()
    {
        canWalk = false;
        animator.SetTrigger("a1");
        yield return new WaitForSeconds(.875f);
        canWalk = true;

        GameObject thisAttack = Instantiate(prefab1, gameObject.transform);
        thisAttack.GetComponent<Animator>().Play(0);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.15f, 0.4f, 10f);
        yield return new WaitForSeconds(.25f);

        thisAttack.GetComponent<PolygonCollider2D>().enabled = false;

        yield return new WaitForSeconds(.5f);
        Destroy(thisAttack);

        while (pdistance > 3)
        {
            yield return null;
        }

        NextAttack();

    }
    IEnumerator A2()
    {
        canWalk = false;
        animator.SetTrigger("a2");

        yield return new WaitForSeconds(.875f);

        GameObject thisAttack1 = Instantiate(prefab2, gameObject.transform.position + new Vector3(3,-1), Quaternion.identity, gameObject.transform);
        GameObject thisAttack2 = Instantiate(prefab2, gameObject.transform.position + new Vector3(-3,-1), Quaternion.identity, gameObject.transform);
        thisAttack1.GetComponent<Animator>().Play(0);
        thisAttack2.GetComponent<Animator>().Play(0);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(1f, 0.05f, 3f);
        StartCoroutine(MovingLazer(1, new Vector3(7, 0, 0), thisAttack1));
        StartCoroutine(MovingLazer(1, new Vector3(-7, 0, 0), thisAttack2));

        canWalk = true;
        yield return new WaitForSeconds(1f);

        StopCoroutine(StartCoroutine(MovingLazer(1, new Vector3(7, 0, 0), thisAttack1)));
        StopCoroutine(StartCoroutine(MovingLazer(1, new Vector3(-7, 0, 0), thisAttack2)));
        Destroy(thisAttack1);
        Destroy(thisAttack2);


        while (pdistance > 3)
        {
            yield return null;
        }

        NextAttack();
    }
    IEnumerator A3()
    {
        canWalk = false;
        animator.SetTrigger("a3");
        yield return new WaitForSeconds(.875f);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.2f, 0.1f, 4f);
        GameObject thisAttack = Instantiate(prefab3, gameObject.transform);
        thisAttack.GetComponent<Animator>().Play(0);
        yield return new WaitForSeconds(.75f);
        Destroy(thisAttack);
        canWalk = true;

        while (pdistance > 3)
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
