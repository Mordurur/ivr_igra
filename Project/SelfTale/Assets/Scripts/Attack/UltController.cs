using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltController : MonoBehaviour
{
    int charID;
    int weaponID;
    bool faceRight = true;
    bool canAttack = true;

    [SerializeField] Renderer rendererA;

    [SerializeField] GameObject spawnObject;
    [SerializeField] RuntimeAnimatorController runtimeAnimatorController;
    [SerializeField] LayerMask isEnemy;

    Player player;

    private float ultModifier;


    private void Awake()
    {
        
        if (GameMaster.GM)
        {
            charID = GameMaster.GM.characters.selectedChar;
            weaponID = GameMaster.GM.items.selectedWeapon;
        }
        else
        {
            charID = 0;
            weaponID = 0;
        }
    }

    private void Start()
    {
        player = GetComponent<Player>();
    }
    public void Ultimate(bool faceSide, float ultt)
    {
        if (!canAttack)
        {
            return;
        }
        ultModifier = ultt;
        faceRight = faceSide;
        switch (charID)
        {
            case 0:
                StartCoroutine(Ult0());
                return;
            case 1:
                StartCoroutine(Ult1());
                return;
            case 2:
                StartCoroutine(Ult2());
                return;
            case 3:
                StartCoroutine(Ult3());
                return;
        }
    }

    IEnumerator CanAttack(float tim)
    {
        canAttack = false;
        yield return new WaitForSeconds(tim);
        canAttack = true;
    }


    IEnumerator WaitDestroy(GameObject attack, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(attack);
    }

    IEnumerator Ult0()
    {
        StartCoroutine(CanAttack(3f));
        GameObject shield = Instantiate(spawnObject, gameObject.transform);
        player.GiveInvince(1f);
        yield return new WaitForSeconds(1);
        Destroy(shield);

    }

    IEnumerator Ult1()
    {
        print(GameMaster.GM.items.weaponDatas[weaponID].weaponID);
        print(GameMaster.GM.items.weaponDatas[weaponID].defencePierce);
        print(GameMaster.GM.items.weaponDatas[weaponID].knockBack);

        StartCoroutine(CanAttack(4f));
        float elapsed = 0f;
        float sinceLastSpawn = 0f;
        while(elapsed < 0.2f)
        {
            if (sinceLastSpawn > 0.02f)
            {
                GameObject columnOfFire = Instantiate(spawnObject, player.transform.position, Quaternion.identity);
                StartCoroutine(WaitDestroy(columnOfFire, .417f));
                foreach (Collider2D enemy in Physics2D.OverlapBoxAll(columnOfFire.GetComponent<BoxCollider2D>().bounds.center, columnOfFire.GetComponent<BoxCollider2D>().bounds.size, 0f, isEnemy))
                {
                    enemy.GetComponent<EnemyBase>().Damage(GameMaster.GM.items.weaponDatas[weaponID].weaponDamage * 4 * ultModifier, GameMaster.GM.items.weaponDatas[weaponID].defencePierce, GameMaster.GM.items.weaponDatas[weaponID].knockBack);
                }
                sinceLastSpawn = 0f;
            }
            player.controller.Move(new Vector2(40,0) * Time.deltaTime * ((faceRight)?-1:1));
            yield return null;
            elapsed += Time.deltaTime;
            sinceLastSpawn += Time.deltaTime;
        }
    }


    IEnumerator Ult2()
    {
        StartCoroutine(CanAttack(4f));
        rendererA.enabled = true;
        SpawnFlyingObject(Vector3.zero, 30, GameMaster.GM.items.weaponDatas[weaponID].weaponDamage * 4 * ultModifier, 1, new float[] { 0, 0, 0.3f, 2 });
        yield return new WaitForSeconds(0.1f);
        SpawnFlyingObject(Vector3.zero, 30, GameMaster.GM.items.weaponDatas[weaponID].weaponDamage * 4 * ultModifier, 1, new float[] { 0, 0, 0.3f, 2 });
        yield return new WaitForSeconds(2.9f);
        rendererA.enabled = false;
    }


    IEnumerator Ult3()
    {
        return null;
    }
    private void SpawnFlyingObject(Vector3 offset1, float flySpeed1, float damage, float bulletLife, float[] colliderProperties)
    {
        GameObject bullet = Instantiate(spawnObject, gameObject.transform.position + offset1, gameObject.transform.rotation);
        if (!faceRight)
        {
            Vector2 theScale = bullet.transform.localScale;
            theScale.x *= -1;
            bullet.transform.localScale = theScale;
            bullet.GetComponent<MovingAttack>().SetMovingAttack(damage, flySpeed1, bulletLife, runtimeAnimatorController, colliderProperties, isEnemy, -1);
        }
        else
        {
            bullet.GetComponent<MovingAttack>().SetMovingAttack(damage, flySpeed1, bulletLife, runtimeAnimatorController, colliderProperties, isEnemy, 1);
        }
    }
}
