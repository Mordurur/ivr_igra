using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster GM;

    [HideInInspector] public static bool enabledMovement = true;

    [HideInInspector] public static bool stagnate = false;

    public CharacterData characters;

    public ItemData items;

    public ProgressData progress;



    void Awake()
    {
        if (GM != null)
            Destroy(gameObject);
        else
            GM = this;

        DontDestroyOnLoad(this);
    }
}

[System.Serializable]
public struct WeaponData 
{
    public bool unlocked;
    public int level;
    public int weaponID;
    public string weaponName;
    public int weaponType;
    public string weaponAnimatorName;
    public int weaponDamage;
    public int defencePierce;
    public int flySpeed;
    public int knockBack;
    public GameObject spObject;
    public RuntimeAnimatorController animatorController;
    public float bulletLife;

    public string infoName;
    public string infoLore;
}

[System.Serializable]
public struct CharData
{
    public bool unlocked;
    public int level;
    public int charID;
    public string charName;
    public string prefabName;
    public int health;

    public string infoName;
    public string infoLore;
}
