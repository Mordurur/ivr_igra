using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Lvl4 : PhaseController
{
    [SerializeField] Dialogue ph1Dialogue;
    [SerializeField] Dialogue ph2Dialogue;
    [SerializeField] Dialogue ph3Dialogue;

    [SerializeField] GameObject npcs;


    [SerializeField] BoxCollider2D[] cBorder;
    [SerializeField] LayerMask playerMask;

    [SerializeField] BoxCollider2D cEntr;
    [SerializeField] GameObject cE;

    [SerializeField] BoxCollider2D cEntr1;
    [SerializeField] GameObject cE1;

    public override void Awake()
    {
        base.Awake();
        levelPhase = GameMaster.GM.progress.levelDatas[4].phase;
        completed = GameMaster.GM.progress.levelDatas[4].completed;
        for (int i = 0; i < 4; i++)
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

    private void Start()
    {
        if (levelPhase == 1)
        {
            displayer.Display(ph1Dialogue);
        }
        if (levelPhase == 3)
        {
            GameMaster.stagnate = true;
            displayer.Display(ph3Dialogue);
        }
    }

    private void Update()
    {
        if (levelPhase == 1)
        {
            Collider2D collider2D = Physics2D.OverlapBox(cEntr.bounds.center, cEntr.bounds.size, 0f, playerMask);
            if (collider2D)
            {
                cE.SetActive(true);
                if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Space) || InputMixer.EnDownS))
                {
                    SetFinishLevel(4);
                    GameMaster.GM.progress.levelDatas[4].phase += 1;
                    GameMaster.enabledMovement = false;
                    SaveLoad.Save();
                }
            }
            else
            {
                cE.SetActive(false);
            }
        }
        if (levelPhase == 4)
        {
            Collider2D collider2D = Physics2D.OverlapBox(cEntr1.bounds.center, cEntr1.bounds.size, 0f, playerMask);
            if (collider2D)
            {
                cE1.SetActive(true);
                if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Space) || InputMixer.EnDownS))
                {
                    SetFinishLevel(4);
                    GameMaster.enabledMovement = false;
                    SaveLoad.Save();
                }
            }
            else
            {
                cE1.SetActive(false);
            }
        }

    }



    public override void DialogueAction(int dId, int sId)
    {
        if (dId == 1337 && sId == 228)
        {
            displayer.Display(ph2Dialogue);
            npcs.SetActive(true);
        }

        if (dId == 12 && sId == -1)
        {
            npcs.SetActive(false);
        }

        if (dId == 13 && sId == -1)
        {
            SetFinishLevel(4);
            GameMaster.GM.progress.levelDatas[4].phase = 3;
            GameMaster.stagnate = false;
            SaveLoad.Save();

        }
        if (dId == 19 && sId == -1)
        {
            GameMaster.GM.progress.levelDatas[3].unlocked = true;
            GameMaster.GM.progress.levelDatas[4].phase += 1;
            GameMaster.GM.progress.levelDatas[4].completed = true;
            SaveLoad.Save();
            SceneManager.LoadScene(0);
        }
    }
}
