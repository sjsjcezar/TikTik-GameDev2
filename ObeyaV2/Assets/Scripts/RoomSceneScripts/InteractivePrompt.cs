using UnityEngine;
using TMPro;

public class InteractivePrompt : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public string interactionText;
    public string isDeadInteractionText;

    private bool isInteracting = false;
    private NPC currentNPC;

    private void Start()
    {
        promptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentNPC = other.GetComponent<NPC>();
            StartInteracting();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopInteracting();
            currentNPC = null;
        }
    }

    public void StartInteracting()
    {
        isInteracting = true;
        if (currentNPC != null && currentNPC.IsDead())
        {
            promptText.text = isDeadInteractionText;
        }
        else
        {
            promptText.text = interactionText;
        }
        promptText.gameObject.SetActive(true);
    }

    public void StopInteracting()
    {
        isInteracting = false;
        promptText.gameObject.SetActive(false);
    }
}