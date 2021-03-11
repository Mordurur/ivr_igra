using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu]
public class SpeakerData : ScriptableObject
{

    public FaceData[] faceDatas;

    public Sprite GetFaceFromID(int fId)
    {
        int s = fId.ToString().Length;
        int f;
        if (fId < 10000)
        {
            return faceDatas[0].face[0];
        }
        s -= 4;
        f = fId % (int)(Mathf.Pow(10,s));
        fId /= (int)(Mathf.Pow(10, s));
        fId %= 1000;
        foreach (FaceData character in faceDatas)
        {
            if (character.faceId == fId)
            {
                return character.face[f];
            }
        }
        return faceDatas[0].face[0];
    }
}


[System.Serializable]
public struct FaceData
{
    public Sprite[] face;
    public int faceId;
}