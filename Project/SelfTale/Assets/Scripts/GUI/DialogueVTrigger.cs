using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVTrigger : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    public LayerMask playerMask;
    DialogueDisplayer displayer;

    [SerializeField] bool special;

    void Start()
    {
        displayer = FindObjectOfType<DialogueDisplayer>();
        
    }

    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(1, 40), 0f, playerMask); ;
        if (hit)
        {

            displayer.Display(dialogue);
            this.enabled = false;
        }
    }
}
