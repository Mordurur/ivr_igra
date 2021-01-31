using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ProgressData
{ 

    public LevelData[] levelDatas;

    public SkillData skillData;

    public long gold;

    public long diamond;

    public bool enabledButtons;


    public int GetLevelID(string levelName)
    {
        foreach (LevelData character in levelDatas)
        {
            if (character.levelName == levelName)
            {
                return character.levelID;
            }
        }
        return 0;
    }

}

[System.Serializable]
public struct LevelData
{
    public int levelID;
    public string levelName;
    public string levelNameGame;
    public bool completed;
    public bool unlocked;
    public int phase;
    public int maxPhase;
}

[System.Serializable]
public struct SkillData
{
    public int skill1Level;
    public int skill2Level;
    public int skill3Level;
}