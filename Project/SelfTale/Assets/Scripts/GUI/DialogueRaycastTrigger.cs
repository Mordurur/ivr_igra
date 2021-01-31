using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class DialogueRaycastTrigger : MonoBehaviour
{
    [SerializeField] GameObject dialogueIndicator;
    [SerializeField] Dialogue dialogue;

    private BoxCollider2D dialogueArea;

    public LayerMask playerMask;
    DialogueDisplayer displayer;

    bool pressed;

    void Start()
    {
        dialogueArea = GetComponent<BoxCollider2D>();
        displayer = FindObjectOfType<DialogueDisplayer>();
        
    }

    private void Update()
    {
        if (GameMaster.enabledMovement && (Input.GetKeyDown(KeyCode.Space) || InputMixer.EnDownS))
        {
            pressed = true;
        }
    }

    void FixedUpdate()
    {
        Collider2D collider2D = Physics2D.OverlapBox(dialogueArea.bounds.center, dialogueArea.bounds.size, 0f, playerMask);
        if (collider2D)
        {
            dialogueIndicator.SetActive(true);
            if (pressed) 
            {
                pressed = false;
                displayer.Display(dialogue);
            }
        }
        else
        {
            dialogueIndicator.SetActive(false);
        }
        pressed = false;
    }
}
