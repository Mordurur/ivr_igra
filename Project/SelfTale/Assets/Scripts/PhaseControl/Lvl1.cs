using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lvl1 : PhaseController
{
    [SerializeField] Dialogue ph1Dialogue;
    [SerializeField] Dialogue sionDialoguefinal;
    [SerializeField] BoxCollider2D[] cBorder;
    [SerializeField] LayerMask playerMask;

    [SerializeField] BoxCollider2D cEntr;
    [SerializeField] GameObject cE;

    [SerializeField] BoxCollider2D cEntr1;
    [SerializeField] GameObject cE1;

    [SerializeField] BoxCollider2D cEntr2;
    [SerializeField] GameObject cE2;

    [SerializeField] EnemyBase sion;
    [SerializeField] GameObject shopMenu;

    [SerializeField] TextMeshProUGUI headtxt;
    [SerializeField] TextMeshProUGUI pricetxt;
    [SerializeField] TextMeshProUGUI infotxt;

    [SerializeField] GameObject shopObject;

    [SerializeField] GameObject cs1;
    [SerializeField] GameObject cs2;

    public bool hsSpoken = false;
    public bool fSpoken = false;

    int chh;

    int idd;

    public override void Awake()
    {
        base.Awake();
        levelPhase = GameMaster.GM.progress.levelDatas[1].phase;
        completed = GameMaster.GM.progress.levelDatas[1].completed;
        for (int i = 0; i < 6; i++)
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
            GameMaster.stagnate = true;
            displayer.Display(ph1Dialogue);
        }
    }

    private void Update()
    {
        if (shopMenu.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (levelPhase == 2)
        {
            Collider2D collider2D = Physics2D.OverlapBox(cEntr.bounds.center, cEntr.bounds.size, 0f, playerMask);
            if (collider2D)
            {
                cE.SetActive(true);
                if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Space) || InputMixer.EnDownS))
                {
                    GameMaster.GM.progress.levelDatas[1].phase += 1;
                    SaveLoad.Save();
                    SceneManager.LoadScene(1);
                }
            }
            else
            {
                cE.SetActive(false);
            }
        }
        else if (levelPhase == 3 && hsSpoken)
        {
            Collider2D collider2D = Physics2D.OverlapBox(cEntr1.bounds.center, cEntr1.bounds.size, 0f, playerMask);
            if (collider2D)
            {
                cE1.SetActive(true);
                if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Space) || InputMixer.EnDownS))
                {
                    GameMaster.GM.progress.levelDatas[1].phase += 1;
                    SaveLoad.Save();
                    SceneManager.LoadScene(1);
                }
            }
            else
            {
                cE1.SetActive(false);
            }
        }
        else if (levelPhase == 4)
        {
            Collider2D collider2D = Physics2D.OverlapBox(cEntr2.bounds.center, cEntr2.bounds.size, 0f, playerMask);
            if (collider2D)
            {
                cE2.SetActive(true);
                if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Space) || InputMixer.EnDownS))
                {
                    GameMaster.GM.progress.levelDatas[1].phase += 1;
                    SaveLoad.Save();
                    SceneManager.LoadScene(1);
                }
            }
            else
            {
                cE2.SetActive(false);
            }
        }
        else if (levelPhase == 5)
        {
            if (sion.health <= 200 && !fSpoken)
            {
                fSpoken = true;
                displayer.Display(sionDialoguefinal);
            }
        }
    }
    public void SelectButton()
    {
        foreach (Transform child in shopObject.transform.parent)
        {
            if (!GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(child.name)].unlocked 
                || !GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(child.name)].unlocked)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void Unlock()
    {
        
        if (chh == 1 && !GameMaster.GM.characters.charDatas[idd].unlocked && GameMaster.GM.progress.diamond >= 1)
        {
            GameMaster.GM.characters.charDatas[idd].unlocked = true;
            GameMaster.GM.progress.diamond--;
            SelectButton();
            SaveLoad.Save();

        }
        else if (chh == 2 && !GameMaster.GM.items.weaponDatas[idd].unlocked && GameMaster.GM.progress.gold >= 1000)
        {
            GameMaster.GM.items.weaponDatas[idd].unlocked = true;
            GameMaster.GM.progress.gold -= 1000;
            SelectButton();

            SaveLoad.Save();

        }
    }

    public int Chh
    {
        set { chh = value; }
    }

    public int Idd
    {
        set { idd = value; }
    }

    public void Shop()
    {
        if (chh == 1)
        {
            headtxt.text = GameMaster.GM.characters.charDatas[idd].infoName;
            infotxt.text = GameMaster.GM.characters.charDatas[idd].infoLore;
            pricetxt.text = "1 алмаз";

        }
        else if (chh == 2)
        {
            headtxt.text = GameMaster.GM.items.weaponDatas[idd].infoName;
            infotxt.text = GameMaster.GM.items.weaponDatas[idd].infoLore;
            pricetxt.text = "1000 золота";
        }
    }


    public override void DialogueAction(int dId, int sId)
    {
        if(dId == 28 && sId == -1)
        {
            headtxt.text = "";
            infotxt.text = "";
            pricetxt.text = "";
            shopMenu.SetActive(true);
            SelectButton();
        }
        if (dId == 1 && sId == -1)
        {
            GameMaster.GM.progress.levelDatas[1].phase += 1;
            GameMaster.stagnate = false;
            SaveLoad.Save();
            SceneManager.LoadScene(1);
        }
        if (dId == 1 && sId == 3)
        {
            cs2.SetActive(true);
            cs1.SetActive(false);
        }
        if (dId == 1 && sId == 5)
        {
            cs2.SetActive(false);
            cs1.SetActive(true);
        }
        if (dId == 5 && sId == -1)
        {
            hsSpoken = true;
        }
        if (dId == 8 && sId == -1)
        {
            GameMaster.GM.progress.levelDatas[4].unlocked = true;
            SetFinishLevel(1);
            GameMaster.GM.progress.levelDatas[1].phase += 1;
            GameMaster.GM.progress.levelDatas[1].completed = true;
            SaveLoad.Save();
        }
        if (dId == 9 && sId == -1)
        {
            GameMaster.GM.progress.levelDatas[2].unlocked = true;
            SetFinishLevel(1);
            GameMaster.GM.progress.levelDatas[1].phase += 1;
            GameMaster.GM.progress.levelDatas[1].completed = true;
            SaveLoad.Save();
        }
    }
}
