using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ItemData
{
    public int selectedWeapon;

    public WeaponData[] weaponDatas;

    public int GetWeaponID(string weaponName)
    {
        foreach(WeaponData weapon in weaponDatas)
        {
            if (weapon.weaponName == weaponName)
            {
                return weapon.weaponID;
            }
        }
        return selectedWeapon;
    }
}

