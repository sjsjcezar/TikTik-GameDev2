using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioSystem : MonoBehaviour
{
    public TextMeshProUGUI radioUIText;
    public TextMeshProUGUI radioText;
    public TextMeshProUGUI mumblingGrowlText;
    public NightManager nightManager;
    public NPCDialogueManager dialogueManager;

    public GameObject journal;
    public GameObject journalPanel;
    public TextMeshProUGUI journalText;

    private int currentLineIndex = 0;
    private bool isInteracting = false;
    private bool hasFinishedDialogue = false; // Track if all lines have been traversed
    private bool hasLearnedCurrentNightFeature = false;
    public int currentNightIndex; // Track the current night index
    private bool playerInRange = false;

    public GameObject radioUIPanel;
    public GameObject leaveButton;
    public DialogueData[] nightDialogues; // Array of ScriptableObjects for each night

    // Features learned flags
    public bool hasLearnedFeature1 = false;
    public bool hasLearnedFeature2 = false;
    public bool hasLearnedFeature3 = false;
    public bool hasLearnedFeature4 = false;

    private AudioSource audioSource;

    private void Start()
    {
        radioUIPanel.SetActive(false);
        leaveButton.SetActive(false);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            if (!isInteracting)
            {
                StartInteracting();
            }
            else
            {
                StopInteracting();
            }
        }

        if (isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextLine();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            radioUIText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            radioUIText.gameObject.SetActive(false);
            StopInteracting();
        }
    }

    private void StartInteracting()
    {
        isInteracting = true;
        currentLineIndex = 0;
        radioUIPanel.SetActive(true);
        ShowNextLine();
    }

    private void StopInteracting()
    {
        isInteracting = false;
        radioUIPanel.SetActive(false);
        leaveButton.SetActive(false);
        audioSource.Stop();
    }

    private void ShowNextLine()
    {
        currentNightIndex = nightManager.currentNight - 1;
        if (currentNightIndex < nightDialogues.Length && currentLineIndex < nightDialogues[currentNightIndex].dialogueEntries.Count && hasLearnedCurrentNightFeature == false)
        {
            var entry = nightDialogues[currentNightIndex].dialogueEntries[currentLineIndex];
            radioText.text = entry.lines;
            PlayVoiceLine(entry.voiceLine);
            HighlightText(radioText.text, entry.highlights);
            currentLineIndex++;
        }
        else
        {
            
            radioText.text = "*bzzzzz*";
            hasFinishedDialogue = true;
            LearnFeature(currentNightIndex);
            leaveButton.SetActive(true); 
            currentLineIndex = 0;
            hasLearnedCurrentNightFeature = true;
            // currentNightIndex++;
        }
        

    }

    private void PlayVoiceLine(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void HighlightText(string text, DialogueData.HighlightInfo[] highlights)
    {
        foreach (var highlight in highlights)
        {
            if (!string.IsNullOrEmpty(highlight.phrase) && text.Contains(highlight.phrase))
            {
                text = text.Replace(highlight.phrase, $"<color=#{ColorUtility.ToHtmlStringRGB(highlight.highlightColor)}>{highlight.phrase}</color>");
            }
        }
        radioText.text = text;
    }

    private void LearnFeature(int nightIndex)
    {
        // Update the feature flag based on the current night
        switch (nightIndex)
        {
            case 0:
                if (hasLearnedFeature1 == false)
                {
                    journalText.text += "\n" + "Look at their Arms";
                }
                hasLearnedFeature1 = true; // Learned elongated limbs on Night 1
                Debug.Log("Learned feature 1.");
                break;
            case 1:
                if (hasLearnedFeature2 == false)
                {
                    journalText.text += "\n" + "Look at their Eyes";
                }
                hasLearnedFeature2 = true; // Learned about eyes on Night 2
                Debug.Log("Learned feature 2.");
                break;
            case 2:
                if (hasLearnedFeature3 == false)
                {
                    journalText.text += "\n" + "Look at their Teeth";
                }
                hasLearnedFeature3 = true; // Learned about sharp teeth on Night 3
                Debug.Log("Learned feature 3.");
                break;
            case 3:
                if (hasLearnedFeature4 == false)
                {
                    journalText.text += "\n" + "Look at their Smiles";
                }
                hasLearnedFeature4 = true; // Learned about false smiles on Night 4
                Debug.Log("Learned feature 4.");
                break;
            default:
                break;
        }
    }

    public void Leave()
    {
        radioUIPanel.SetActive(false);
        leaveButton.SetActive(false);
        audioSource.Stop();
        isInteracting = false;
    }

    public bool HasLearnedFeature(int featureIndex)
    {
        switch (featureIndex)
        {
            case 1: return hasLearnedFeature1;
            case 2: return hasLearnedFeature2;
            case 3: return hasLearnedFeature3;
            case 4: return hasLearnedFeature4;
            default: return false;
        }
    }

    public void ResetCurrentNightFeature()
    {
        hasLearnedCurrentNightFeature = false;
    }

    public void DisableJournal()
    {
        journal.SetActive(false);
    }

    public void EnableJournal()
    {
        journal.SetActive(true);
        
    }
}
