using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePlayer : MonoBehaviour
{
    CharData characterData1;
    WeaponData weaponData1;
    [SerializeField] UnityEngine.UI.Slider slider1;
    [SerializeField] UnityEngine.UI.Slider slider2;

    private void Awake()
    {
        characterData1 = GameMaster.GM.characters.charDatas[GameMaster.GM.characters.selectedChar];
        weaponData1 = GameMaster.GM.items.weaponDatas[GameMaster.GM.items.selectedWeapon];

        Object playerPrefab = Resources.Load(characterData1.prefabName);
        Object WeaponRenderer;
        switch (weaponData1.weaponType)
        {
            case 1:
                WeaponRenderer = Resources.Load("SaberRenderer");
                break;
            case 2:
                WeaponRenderer = Resources.Load("ThrowableRenderer");
                break;
            case 3:
                WeaponRenderer = Resources.Load("LanceRenderer");
                break;
            case 4:
                WeaponRenderer = Resources.Load("SwordRenderer");
                break;
            default:
                WeaponRenderer = Resources.Load("SaberRenderer");
                break;
        }

        GameObject player = Instantiate(playerPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        player.name = "Player";
        GameObject weapon = Instantiate(WeaponRenderer, gameObject.transform.position, Quaternion.identity, player.transform) as GameObject;
        weapon.name = "WeaponRenderer";
        player.GetComponent<Player>().maxHealth = characterData1.health + characterData1.level;
        player.GetComponent<Player>().skill1modifier = 1 + GameMaster.GM.progress.skillData.skill1Level * 0.1f;
        player.GetComponent<Player>().skill2modifier = 1 + GameMaster.GM.progress.skillData.skill2Level * 0.1f;
        player.GetComponent<Player>().skill3modifier = 1 + GameMaster.GM.progress.skillData.skill3Level * 0.1f;
        player.GetComponent<Player>().sliderHP = slider1;
        player.GetComponent<Player>().sliderSTA = slider2;

        weaponData1.weaponDamage += weaponData1.weaponDamage / 10 * weaponData1.level;
        weapon.GetComponent<WeaponController>().SetWeapon(weaponData1);

    }
}
