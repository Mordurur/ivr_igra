using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    public int DialogueID;
    public string choiceName;
    public int options;
    public DialogueLine[] lines;
    public string[] choiceText;
    public Dialogue[] nextDialogues;
}

[System.Serializable]
public struct DialogueLine
{
    public int lineId;
    public float speed;
    public int faceId;
    public string speaker;
    [TextArea(3,10)]
    public string text;

}
