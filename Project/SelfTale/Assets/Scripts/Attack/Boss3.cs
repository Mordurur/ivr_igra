using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3 : EnemyBase
{
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject prefab2Summon;
    [SerializeField] GameObject prefab3;
    [SerializeField] GameObject prefab4;
    [SerializeField] GameObject npcs;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject movingPlatforms;

    UnityEngine.UI.Slider bossBar;

    [SerializeField] GameObject walls;

    int prevAttack = 1;

    float deathTimer = 0f;

    protected override void Start()
    {
        base.Start();
        bossBar = gameObject.GetComponentInChildren<UnityEngine.UI.Slider>();
        walls.SetActive(true);
        animator.enabled = true;
        NextAttack();
        npcs.SetActive(false);
    }



    void NextAttack()
    {
        int next;
        Random.InitState(System.DateTime.Now.Millisecond);
        next = Random.Range(0, 4);
        while (next == prevAttack)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            next = Random.Range(0, 4);
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
        else if (next == 2)
        {
            StartCoroutine(A3());
        }
        else
        {
            StartCoroutine(A4());
        }
    }

    protected override void FixedUpdate()
    {
        deathTimer += Time.fixedDeltaTime;
        if (deathTimer > 40)
        {
            Damage(10000, 0, 0);
        } 
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
        GameObject crossAttack = Instantiate(prefab2Summon, moveObject.transform.position, Quaternion.identity);
        Destroy(moveObject);
        yield return new WaitForSeconds(.5833f);
        Destroy(crossAttack);
    }


    IEnumerator WaitDestroy(GameObject attack, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(attack);
    }

    IEnumerator A1()
    {
        animator.SetTrigger("a1");
        yield return new WaitForSeconds(.833f);

        GameObject thisAttack = Instantiate(prefab1, gameObject.transform.position, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.15f, 0.4f, 10f);
        StartCoroutine(WaitDestroy(thisAttack, 0.583f));
        yield return new WaitForSeconds(.25f);

        for (int i = 0; i < 5; i++)
        {
            GameObject thisAttack1 = Instantiate(prefab1, gameObject.transform.position + new Vector3(3, 0, 0) * i, Quaternion.identity);
            GameObject thisAttack2 = Instantiate(prefab1, gameObject.transform.position + new Vector3(-3, 0, 0) * i, Quaternion.identity);
            StartCoroutine(WaitDestroy(thisAttack1, 0.583f));
            StartCoroutine(WaitDestroy(thisAttack2, 0.583f));

            Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.1f, 0.1f, 4f);

            yield return new WaitForSeconds(.25f);
        }

        yield return new WaitForSeconds(.25f);

        NextAttack();

    }
    IEnumerator A2()
    {
        animator.SetTrigger("a2");

        yield return new WaitForSeconds(1.333f);

        int newRInt = Random.Range(0, 60);
        for (int i = 0; i < 4; i++)
        {
            GameObject thisAttack1 = Instantiate(prefab2, gameObject.transform.position, Quaternion.identity);
            Vector2 velocity1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (i * 360 / 4 +newRInt)), Mathf.Sin(Mathf.Deg2Rad * (i * 360 / 4 + newRInt))) * 8;


            StartCoroutine(MovingLazer(.875f, velocity1, thisAttack1));
        }


        yield return new WaitForSeconds(.25f);

        NextAttack();
    }
    IEnumerator A3()
    {
        animator.SetTrigger("a3");
        yield return new WaitForSeconds(.833f);
        GameObject thisAttack = Instantiate(prefab3, gameObject.transform.position, Quaternion.identity);
        Collider2D[] collider2Ds = thisAttack.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider2D1 in collider2Ds)
        {
            collider2D1.enabled = false;
        }
        Animator[] animators2Ds = thisAttack.GetComponentsInChildren<Animator>();
        foreach (Animator collider2D1 in animators2Ds)
        {
            collider2D1.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        NextAttack();

        foreach (Collider2D collider2D1 in collider2Ds)
        {
            collider2D1.enabled = true;
        }
        foreach (Animator collider2D1 in animators2Ds)
        {
            collider2D1.enabled = true;
        }
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.1f, 0.3f, 4f);
        yield return new WaitForSeconds(.583f);
        Destroy(thisAttack);

    }

    IEnumerator A4()
    {
        animator.SetTrigger("a4");
        yield return new WaitForSeconds(1f);
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.2f, 0.1f, 4f);
        int dir = Random.Range(0, 2) == 1 ? 1 : -1;
        GameObject thisAttack = Instantiate(prefab4, gameObject.transform.position + new Vector3(10,0,0) * dir, Quaternion.identity);
        float elapsed = 0f;
        Vector3 velocity2 = new Vector3(-8, 0, 0) * dir;
        while (Mathf.Abs(gameObject.transform.position.x - thisAttack.transform.position.x) > 1)
        {

            thisAttack.transform.position += velocity2 * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;

        }
        thisAttack.GetComponent<BoxCollider2D>().enabled = false;
        thisAttack.GetComponent<Animator>().SetTrigger("fade");

        yield return new WaitForSeconds(.375f);


        Destroy(thisAttack);

        yield return new WaitForSeconds(.2f);

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
        npcs.SetActive(true);
        float elapsed = 0.0f;
        StopCoroutine(A1());
        StopCoroutine(A2());
        StopCoroutine(A3());
        StopCoroutine(A4());
        this.enabled = false;
        canWalk = false;
        while (elapsed < 1)
        {
            spriteRenderer.color = Color.Lerp(Color.white, Color.cyan, elapsed / 1);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.1f, 1f, 3f);
        phaseController.DialogueAction(1337, 228);
        GameMaster.GM.progress.diamond += 1;
        phaseController.kills++;
        phaseController.goldEarned += goldReward;
        Destroy(gameObject);
    }
}
