using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl5 : PhaseController
{
    [SerializeField] Dialogue dial1;
    [SerializeField] Dialogue dial2;

    [SerializeField] Boss3 boss3;

    public override void Awake()
    {
        base.Awake();
        boss3.enabled = false;
        boss3.gameObject.SetActive(false);
        completed = GameMaster.GM.progress.levelDatas[5].completed;
        levelPhase = GameMaster.GM.progress.levelDatas[5].phase;
    }
    private void Start()
    {
        if (!completed)
        {
            displayer.Display(dial1);
        }
    }

    public override void DialogueAction(int dId, int sId)
    {

        if (dId == 26 && sId == -1)
        {
            boss3.enabled = true;
        }

        if (dId == 26 && sId == 9)
        {
            boss3.gameObject.SetActive(true);
        }

        if (dId == 1337 && sId == 228)
        {
            if (!completed)
            {
                displayer.Display(dial2);
            }
            else
            {
                GameMaster.GM.progress.levelDatas[5].phase = 2;
                SetFinishLevel(5);

            }
            
        }
        if (dId == 27 && sId == -1)
        {
            GameMaster.GM.progress.levelDatas[5].completed = true;
            SetFinishLevel(5);
            SaveLoad.Save();
        }
    }
}
