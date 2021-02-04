using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Lvl3 : PhaseController
{
    [SerializeField] Dialogue ph1Dialogue;
    [SerializeField] Dialogue ph2Dialogue;
    [SerializeField] Dialogue ph3Dialogue;

    [SerializeField] GameObject waveOfEnemy;
    [SerializeField] GameObject enemySpawner;

    [SerializeField] GameObject blindfold;

    [SerializeField] BoxCollider2D[] cBorder;

    [SerializeField] GameObject npcs;

    public override void Awake()
    {
        base.Awake();
        levelPhase = GameMaster.GM.progress.levelDatas[3].phase;
        completed = GameMaster.GM.progress.levelDatas[3].completed;
        for (int i = 0; i < 3; i++)
        {
            if (i != levelPhase - 1)
            {
                phases[i].SetActive(false);
            }
            else
            {
                phases[i].SetActive(true);
                Camera.main.GetComponent<CameraFollow>().cameraBounds = cBorder[i];
            }
        }
    }

    public void Start()
    {
        if (levelPhase == 1)
        {
            displayer.Display(ph1Dialogue);
        }
        if (levelPhase == 2)
        {
            displayer.Display(ph3Dialogue);
        }
        if (levelPhase == 3)
        {
            StartCoroutine(Defence());
        }
    }


    IEnumerator Defence()
    {
        npcs.SetActive(false);
        int t = 0;
        ArrayList arrayList = new ArrayList();
        while (t < 40)
        {
            GameObject i = Instantiate(waveOfEnemy, enemySpawner.transform.position, Quaternion.identity);
            i.GetComponent<EnemyBase>().Flip();
            arrayList.Add(i);
            yield return new WaitForSeconds(5f);

            t += 5;
        }
        foreach (GameObject obj in arrayList)
        {
            Destroy(obj);
        }
        if (levelPhase == 1)
            displayer.Display(ph2Dialogue);

        else {
            SaveLoad.Save();

            SetFinishLevel(3);
        }
        npcs.SetActive(true);
    }

    public override void DialogueAction(int dId, int sId)
    {
        if (dId == 22 && sId == -1)
        {
            StartCoroutine(Defence());
        }
        if (dId == 25 && sId == 0)
        {
            blindfold.SetActive(true);
        }
        if (dId == 25 && sId == 18)
        {
            blindfold.SetActive(false);
        }
        if (dId == 25 && sId == -1)
        {
            SetFinishLevel(3);
            GameMaster.GM.progress.levelDatas[3].phase = 3;
            GameMaster.GM.progress.levelDatas[5].unlocked = true;
            SaveLoad.Save();
        }
        if (dId == 23 && sId == -1)
        {
            SetFinishLevel(3);
            GameMaster.GM.progress.levelDatas[3].phase = 2;
            SaveLoad.Save();
        }

    }

}
