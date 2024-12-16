using System;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCDialogueManager dialogueManager;
    public NPCDialogue npcDialogue;
    private bool isInRange = false;
    public NPC currentNPC;
    private EnergyManager energyManager;
    private InteractivePrompt interactivePrompt;

    private void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
        interactivePrompt = FindObjectOfType<InteractivePrompt>();
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Q) && energyManager.tempEnergy != 0 && !currentNPC.IsDead())
        {
            dialogueManager.StartDialogue(npcDialogue);
            interactivePrompt.promptText.text = interactivePrompt.interactionText;
            
        }

        if (isInRange && Input.GetKeyDown(KeyCode.Q) && energyManager.tempEnergy == 0 && !currentNPC.IsDead())
        {
            dialogueManager.ShowCannotTalkMessage();
            interactivePrompt.promptText.text = interactivePrompt.interactionText;
        }
        if(currentNPC.IsDead())
        {
            interactivePrompt.promptText.text = interactivePrompt.isDeadInteractionText;
            Debug.Log("Cannot interact with a dead NPC.");
        }
    }

    public void StartDialogueOnSpawn()
    {
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(npcDialogue);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            currentNPC = GetComponent<NPC>();
            interactivePrompt.StartInteracting();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            currentNPC = null;
            interactivePrompt.StopInteracting();
        }
    }

    public void EnableInteraction()
    {
        this.enabled = true;
    }
}