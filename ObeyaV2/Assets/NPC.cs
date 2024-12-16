using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCDialogue npcDialogue; // Reference to the NPCDialogue ScriptableObject

    public SpriteRenderer spriteRenderer; // Reference to the sprite renderer to change sprite
    public Sprite deadBodySprite;

    private Animator animator; // Reference to the Animator component

    public string uniqueID;
    private bool isDead = false; 

    private void Awake()
    {
        // Get the Animator component when the NPC is instantiated
        animator = GetComponent<Animator>();
    }

    public void RespondToCheck(string response)
    {
        // Display the response in the game's dialogue UI
        Debug.Log(response);
    }

    public void SwapToDeadSprite()
    {
        if (animator != null)
        {
            animator.enabled = false;
            Debug.Log("Animator disabled for NPC: " + gameObject.name);
        }

        if (gameObject.CompareTag("Human"))
        {
            spriteRenderer.sprite = deadBodySprite;
            Debug.Log("Swapped to garbage bag sprite for " + gameObject.name);
        }
        else if (gameObject.CompareTag("Aswang") && deadBodySprite != null)
        {
            spriteRenderer.sprite = deadBodySprite;
            Debug.Log("Swapped to dead sprite for " + gameObject.name);
        }
        else
        {
            Debug.LogError("Dead body sprite is not assigned for " + gameObject.name);
        }
         
        isDead = true;
    }

    public bool IsDead()
    {
        return isDead;
    }


    // Check if the NPC has a specific feature based on the ScriptableObject
    public bool HasFeature(int featureIndex)
    {
        // Return true if the feature index is valid, otherwise false
        return featureIndex >= 0 && featureIndex < npcDialogue.featureDialoguesList.Count;
    }
}
