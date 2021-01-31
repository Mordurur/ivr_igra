using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterData
{
    public string playerName;

    public int selectedChar;

    public CharData[] charDatas;

    public int GetCharacterID(string charName)
    {
        foreach (CharData character in charDatas)
        {
            if (character.charName == charName)
            {
                return character.charID;
            }
        }
        return selectedChar;
    }

}