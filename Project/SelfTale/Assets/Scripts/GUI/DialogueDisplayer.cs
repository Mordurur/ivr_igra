using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueDisplayer : MonoBehaviour
{
    static bool playingDialogue = false;

    private Queue<DialogueLine> sentences;
    DialogueLine nextLine;

    private bool playingSentence;
    private float blinkTimer;
    private Dialogue dialogue1;

    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI nameBox;
    [SerializeField] TextMeshProUGUI mainBox;
    [SerializeField] GameObject nextBox;
    [SerializeField] GameObject buttons;
    [SerializeField] public GameObject healthBars;

    [SerializeField] GameObject choiceBox;
    [SerializeField] TextMeshProUGUI textChoice;
    [SerializeField] GameObject choice1;
    [SerializeField] GameObject choice2;
    [SerializeField] GameObject choice3;

    private GameObject[] choicesList;
    private PhaseController phaseController;

    

    private void Start()
    {
        choicesList = new GameObject[3];
        choicesList[0] = choice1;
        choicesList[1] = choice2;
        choicesList[2] = choice3;
        sentences = new Queue<DialogueLine>();
        phaseController = FindObjectOfType<PhaseController>().GetComponent<PhaseController>();
    }

    private void Update()
    {
        if(blinkTimer <= 0)
        {
            blinkTimer = 1;
        }
        else
        {
            blinkTimer -= Time.deltaTime;
        }

        if(blinkTimer > 0.5f && !playingSentence)
        {
            nextBox.SetActive(true);
        }
        else
        {
            nextBox.SetActive(false);
        }


        if (dialogueBox.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (playingSentence)
            {
                playingSentence = false;
                StopAllCoroutines();
                mainBox.text = nextLine.text;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void Display(Dialogue dialogue)
    {
        if (playingDialogue)
        {
            return;
        }
        healthBars.SetActive(false);
        
        foreach(Transform transform in buttons.transform)
        {
            transform.GetComponent<UnityEngine.UI.RawImage>().enabled = false;
        }
        dialogue1 = dialogue;
        mainBox.text = "";
        GameMaster.enabledMovement = false;
        dialogueBox.SetActive(true);
        sentences.Clear();
        choiceBox.SetActive(false);
        foreach (DialogueLine line in dialogue1.lines)
        {
            sentences.Enqueue(line);
        }
        DisplayNextSentence();

    }
    public void DisplayNextSentence()
    {
        if(sentences.Count > 0)
        {
            nextLine = sentences.Dequeue();
            nextLine.text = nextLine.text.Replace("(игрок)", GameMaster.GM.characters.playerName);
            if (nextLine.speaker == "Игрок")
            {
                nextLine.speaker = GameMaster.GM.characters.playerName;
            }
            phaseController.DialogueAction(dialogue1.DialogueID, nextLine.lineId);
            nameBox.text = nextLine.speaker;
            StartCoroutine(TextDisplayer(nextLine.text, nextLine.speed));
            choiceBox.SetActive(false);
        }
        else if (dialogue1.options == 1)
        {
            StopAllCoroutines();
            playingDialogue = false;
            Display(dialogue1.nextDialogues[0]);
        }
        else if (dialogue1.options > 0)
        {
            choiceBox.SetActive(true);
            textChoice.text = dialogue1.choiceName;
            foreach (GameObject cho in choicesList)
            {
                cho.SetActive(false);
            }
            for (int i = 0; i < dialogue1.options; i++)
            {
                choicesList[i].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners(); 
                choicesList[i].SetActive(true);
                choicesList[i].GetComponentInChildren<TextMeshProUGUI>().text = dialogue1.choiceText[i];
                int tempInt = i;
                choicesList[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { Display(dialogue1.nextDialogues[tempInt]); }); 
            }
            StopAllCoroutines();
            playingDialogue = false;
        }
        else
        {
            phaseController.DialogueAction(dialogue1.DialogueID, -1);
            StopAllCoroutines();
            choiceBox.SetActive(false);
            playingDialogue = false;
            dialogueBox.SetActive(false);
            healthBars.SetActive(true);
            foreach (Transform transform in buttons.transform)
            {
                transform.GetComponent<UnityEngine.UI.RawImage>().enabled = GameMaster.GM.progress.enabledButtons;
            }
            GameMaster.enabledMovement = true;
        }
           
    }

    public IEnumerator TextDisplayer(string text, float speed)
    {
        playingSentence = true;
        mainBox.text = "";
        foreach(char letter in text.ToCharArray())
        {
            mainBox.text += letter;
            yield return new WaitForSeconds(speed / 40);
        }
        playingSentence = false;
    }
}
