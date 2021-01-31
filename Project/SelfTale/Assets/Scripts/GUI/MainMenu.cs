using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI diamond;

    [SerializeField] GameObject toggleScreenControls;

    [SerializeField] TextAsset OGDataChar;
    [SerializeField] TextAsset OGDataItem;
    [SerializeField] TextAsset OGDataProgress;

    [SerializeField] private GameObject renameButton;
    [SerializeField] private TextMeshProUGUI infoHead;
    [SerializeField] private TextMeshProUGUI infoMain;

    [SerializeField] private TextMeshProUGUI upgradeMain;
    [SerializeField] private TextMeshProUGUI upgradeValues;
    [SerializeField] private TextMeshProUGUI upgradePrice;

    [SerializeField] private GameObject charMenu;
    [SerializeField] private GameObject skillMenu;
    [SerializeField] private GameObject weaponMenu;
    [SerializeField] private GameObject playMenu;


    [SerializeField] private GameObject charSelectButtonList;
    [SerializeField] private GameObject weaponSelectButtonList;
    [SerializeField] private GameObject levelsSelectButtonList;

    [SerializeField] private TextMeshProUGUI LevelHead;
    [SerializeField] private TextMeshProUGUI LevelPhase;

    [SerializeField] private UnityEngine.UI.Button upgradeButton;

    private string charName = " ";
    private string weaponName = " ";
    private string skillName = " ";
    private int sceneName;

    private long st;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        SaveLoad.Load();
        renameButton.GetComponent<TextMeshProUGUI>().SetText(GameMaster.GM.characters.playerName);
        DisableButtons();
        upgradeButton.enabled = false;
        gold.text = GameMaster.GM.progress.gold.ToString();
        diamond.text = GameMaster.GM.progress.diamond.ToString();
        toggleScreenControls.GetComponent<UnityEngine.UI.Image>().color = GameMaster.GM.progress.enabledButtons ? Color.white : Color.grey;
    }

    public void ResetProgress()
    {
        GameMaster.GM.characters = JsonUtility.FromJson<CharacterData>(OGDataChar.ToString());
        GameMaster.GM.items = JsonUtility.FromJson<ItemData>(OGDataItem.ToString());
        GameMaster.GM.progress = JsonUtility.FromJson<ProgressData>(OGDataProgress.ToString());
        gold.text = GameMaster.GM.progress.gold.ToString();
        diamond.text = GameMaster.GM.progress.diamond.ToString();
        DisableButtons();
        upgradeButton.enabled = false;
        SaveLoad.Save();
    }

    public void SetButtons()
    {
        GameMaster.GM.progress.enabledButtons = !GameMaster.GM.progress.enabledButtons;
        toggleScreenControls.GetComponent<UnityEngine.UI.Image>().color = GameMaster.GM.progress.enabledButtons ? Color.white : Color.grey;
        SaveLoad.Save();
    }

    private void DisableButtons()
    {
        foreach (Transform child in levelsSelectButtonList.transform)
        {
            UnityEngine.UI.Button buttonController = child.GetComponent<UnityEngine.UI.Button>();
            if (buttonController)
            {
                if (GameMaster.GM.progress.levelDatas[GameMaster.GM.progress.GetLevelID(child.gameObject.name)].completed)
                {
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.green;
                    buttonController.enabled = true;
                }
                else if (GameMaster.GM.progress.levelDatas[GameMaster.GM.progress.GetLevelID(child.gameObject.name)].unlocked)
                {
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.blue;
                    buttonController.enabled = true;
                }
                else
                {
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.black;
                    buttonController.enabled = false;
                }
            }
        }
        foreach (Transform child in weaponSelectButtonList.transform)
        {
            UnityEngine.UI.Button buttonController = child.GetComponent<UnityEngine.UI.Button>();
            if (buttonController)
            {
                if (GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(child.gameObject.name)].unlocked)
                {
                    buttonController.enabled = true;
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.white;
                }
                else
                {
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.black;
                    buttonController.enabled = false;
                }
            }
        }
        foreach (Transform child in charSelectButtonList.transform)
        {
            UnityEngine.UI.Button buttonController = child.GetComponent<UnityEngine.UI.Button>();
            if (buttonController)
            {
                if (GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(child.gameObject.name)].unlocked)
                {
                    buttonController.enabled = true;
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.white;
                }
                else
                {
                    var colr = buttonController.GetComponent<UnityEngine.UI.Image>();
                    colr.color = Color.black;
                    buttonController.enabled = false;
                }
            }
        }

    }

    public void SetPlayType(int playType)
    {
        sceneName = playType;
        LevelHead.text = GameMaster.GM.progress.levelDatas[playType].levelNameGame;
        if (!GameMaster.GM.progress.levelDatas[playType].completed)
        {
            LevelPhase.text =$"Фаза {GameMaster.GM.progress.levelDatas[playType].phase} \n из {GameMaster.GM.progress.levelDatas[playType].maxPhase}";

        }
        else
        {
            LevelPhase.text = "Пройдено";
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Upgrade()
    {
        if (charMenu.activeSelf)
        {
            GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(charName)].level++;
            GameMaster.GM.progress.gold -= st;
            gold.text = GameMaster.GM.progress.gold.ToString();
            LoadUpgrade();
        }
        else if (weaponMenu.activeSelf)
        {
            GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(weaponName)].level++;
            GameMaster.GM.progress.gold -= st;
            gold.text = GameMaster.GM.progress.gold.ToString();
            LoadUpgrade();
        }
        else if (skillMenu.activeSelf)
        {
            if (skillName == "Skill1")
            {
                GameMaster.GM.progress.skillData.skill1Level++;
                GameMaster.GM.progress.gold -= st;

            }
            else if (skillName == "Skill2")
            {
                GameMaster.GM.progress.skillData.skill2Level++;
                GameMaster.GM.progress.gold -= st;
            }
            else if (skillName == "Skill3")
            {
                GameMaster.GM.progress.skillData.skill3Level++;
                GameMaster.GM.progress.gold -= st;
            }
            gold.text = GameMaster.GM.progress.gold.ToString();
            LoadUpgrade();
        }
        SaveLoad.Save();
    }


    public void LoadInfo()
    {
        if (charMenu.activeSelf)
        {
            if (charName != " ")
            {
            infoHead.text = GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(charName)].infoName;
            infoMain.text = GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(charName)].infoLore;
            }
            else
            {
                infoHead.text = " ";
                infoMain.text = " ";
            }

        }
        else
        {
            if (weaponName != " ")
            {
                infoHead.text = GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(weaponName)].infoName;
                infoMain.text = GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(weaponName)].infoLore;
            }
            else
            {
                infoHead.text = " ";
                infoMain.text = " ";
            }

        }
    }

    public void LoadUpgrade()
    {
        if (charMenu.activeSelf)
        {
            if (charName != " ")
            {
                upgradeMain.text = GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(charName)].infoName;
                int lvll = GameMaster.GM.characters.charDatas[GameMaster.GM.characters.GetCharacterID(charName)].level;
                if(lvll < 20)
                {
                    upgradeValues.text =$"{lvll} -> {lvll + 1}";
                }
                else
                {
                    upgradeValues.text = "Достигнут максимальный уровень 20";
                }
                st = 5 * (long)Mathf.Pow(1.5f, lvll - 1);
                upgradePrice.text = st.ToString();
                if (GameMaster.GM.progress.gold < st || lvll > 19)
                {
                    upgradePrice.color = Color.red;
                    upgradeButton.enabled = false;
                }
                else
                {
                    upgradePrice.color = Color.white;
                    upgradeButton.enabled = true;
                }
            }
            else
            {
                upgradeMain.text = " ";
                upgradeValues.text = " ";
                upgradeButton.enabled = false;
            }

        }
        else if (weaponMenu.activeSelf)
        {
            if (weaponName != " ")
            {
                upgradeMain.text = GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(weaponName)].infoName;
                int lvll = GameMaster.GM.items.weaponDatas[GameMaster.GM.items.GetWeaponID(weaponName)].level;
                if (lvll < 40)
                {
                    upgradeValues.text = $"{lvll} -> {lvll + 1}";
                }
                else
                {
                    upgradeValues.text = "Достигнут максимальный уровень 40";
                }
                st = 3 * (long)Mathf.Pow(1.5f, lvll - 1);
                upgradePrice.text = st.ToString();
                if (GameMaster.GM.progress.gold < st || lvll > 39)
                {
                    upgradePrice.color = Color.red;
                    upgradeButton.enabled = false;
                }
                else
                {
                    upgradePrice.color = Color.white;
                    upgradeButton.enabled = true;
                }
            }
            else
            {
                upgradeMain.text = " ";
                upgradeValues.text = " ";
                upgradeButton.enabled = false;
            }

        }
        else if (skillMenu.activeSelf)
        {
            int lvll;
            if (skillName == "Skill1")
            {
                upgradeMain.text = "Быстрая атака";
                lvll = GameMaster.GM.progress.skillData.skill1Level;

            }
            else if (skillName == "Skill2")
            {
                upgradeMain.text = "Сильная атака";
                lvll = GameMaster.GM.progress.skillData.skill2Level;
            }
            else if (skillName == "Skill3")
            {
                upgradeMain.text = "Специальная атака";
                lvll = GameMaster.GM.progress.skillData.skill3Level;
            }
            else
            {
                upgradeMain.text = " ";
                lvll = 10;
            }

            if (lvll < 10)
            {
                upgradeValues.text = $"{lvll} -> {lvll + 1}";
            }
            else
            {
                upgradeValues.text = "Достигнут максимальный уровень 10";
            }
            st = 7 * (long)Mathf.Pow(1.5f, lvll - 1);
            upgradePrice.text = st.ToString();
            if (GameMaster.GM.progress.gold < st || lvll > 9)
            {
                upgradePrice.color = Color.red;
                upgradeButton.enabled = false;
            }
            else
            {
                upgradePrice.color = Color.white;
                upgradeButton.enabled = true;
            }

        }
    }


    public void SelectButton(GameObject gameObject)
    {
        foreach(Transform child in gameObject.transform.parent)
        {
            if(child.gameObject != gameObject)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }


    public void SetCharString(string setString)
    {
        charName = setString;
    }

    public void SetWeapString(string setString)
    {
        weaponName = setString;
    }

    public void SetSkillString(string setString)
    {
        skillName = setString;
    }

    public void SetChar()
    {
        GameMaster.GM.characters.selectedChar = GameMaster.GM.characters.GetCharacterID(charName);
        SaveLoad.Save();
    }
    public void SetWeapon()
    {
        GameMaster.GM.items.selectedWeapon = GameMaster.GM.items.GetWeaponID(weaponName);
        SaveLoad.Save();
    }
}
