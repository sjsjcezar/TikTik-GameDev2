using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using System.Diagnostics;

public class NPCDialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class NPCDeadBodyPair
    {
        public string npcName;
        public GameObject deadBodyPrefab;
    }
    [SerializeField] private List<NPCDeadBodyPair> npcDeadBodyPairs;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private GameObject messagePanelBG;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button exitButton;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;

    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public Button option4Button;
    public Button killButton;
    public Button hearOutButton;
    public Button elongateArmsButton;
    public Button darkEyesButton;
    public Button sharpTeethButton;
    public Button falseSmileButton;

    public GameObject dialoguePanel;
    public GameObject buttonsPanel;

    private NPCDialogue currentNPCDialogue;
    private int dialogueIndex = 0; // Index to track current dialogue

    public bool isInDialogue = false;
    private int aswangResponseIndex = 0;
    private int eclipseResponseIndex = 0;
    private int killResponseIndex = 0;

    // Variables for hear out dialogues
    private int hearOutDialogues1Index = 0;
    private int hearOutDialogues2Index = 0;
    private int hearOutDialogues3Index = 0;
    private int hearOutDialogues4Index = 0;

    private string activeResponseType = "";
    private NPCInteraction npcInteraction;
    private RadioSystem radioSystem;
    private NightManager nightManager;
    private Sprite originalNPCSprite;
    public Image npcImage;
    public Image featureImage;
    public Image featureImageEyes;
    public Image featureImageTeeth;
    public Image featureImageFalseSmile;

    public PlayerMovement playerMovement;

    private EnergyManager energyManager;
    public GameObject cannotTalkPanel; // Panel to display the cannot sleep message
    public TextMeshProUGUI cannotTalkText; // TextMeshProUGUI component for displaying the cannot sleep message
    public float cannotTalkMessageDuration = 5f;

    private string lastClickedFeatureName;

    private void Start()
    {
        npcInteraction = FindObjectOfType<NPCInteraction>();
        option1Button.onClick.AddListener(Option1Selected);
        option2Button.onClick.AddListener(Option2Selected);
        option3Button.onClick.AddListener(Option3Selected);
        option4Button.onClick.AddListener(EndDialogue);

        if (npcImage != null)
        {
            npcImage.gameObject.SetActive(false);
        }

        nightManager = FindObjectOfType<NightManager>();
        energyManager = FindObjectOfType<EnergyManager>();
        InitializeFeatureButtons();

        // Make sure the kill button is enabled
        killButton.gameObject.SetActive(false);
        hearOutButton.gameObject.SetActive(false);
        exitButton.onClick.AddListener(ExitLearnedNothingMessage);
        
    }

    private void Awake()
    {
        radioSystem = FindObjectOfType<RadioSystem>();
    }

    private void Update()
    {
        if (isInDialogue && Input.GetKeyDown(KeyCode.E))  // Press 'E' to go to next dialogue
        {
            DisplayNextDialogue();
        }
    }

    public void StartDialogue(NPCDialogue npcDialogue)
    {
        currentNPCDialogue = npcDialogue;
        dialogueIndex = 0; // Reset index when starting a new dialogue
        aswangResponseIndex = 0;  // Reset the response indexes
        eclipseResponseIndex = 0;
        hearOutDialogues1Index = 0;
        hearOutDialogues2Index = 0;
        hearOutDialogues3Index = 0;
        hearOutDialogues4Index = 0;
        activeResponseType = "";

        dialoguePanel.SetActive(true);
        buttonsPanel.SetActive(false);

        if (npcImage != null)
        {
            npcImage.sprite = currentNPCDialogue.npcSprite; // Assuming you have a sprite field in NPCDialogue
            npcImage.gameObject.SetActive(true); // Show the NPC picture
        }

        Debug.Log("Starting Dialogue for NPC: " + npcDialogue.npcName);
        isInDialogue = true;
        DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (activeResponseType != "")
        {
            DisplaySequentialResponse();
            return;
        }

        // Get dialogues for the current night
        var dialoguesForCurrentNight = GetDialoguesForCurrentNight();

        // Display the next dialogue if there is one
        if (dialogueIndex < dialoguesForCurrentNight.Count)
        {
            dialogueText.text = dialoguesForCurrentNight[dialogueIndex];
            dialogueIndex++; // Increment the index for the next call
        }
        else
        {
            Debug.Log("General dialogue ended, showing response buttons.");
            buttonsPanel.SetActive(true); // Show the buttons after general dialogue ends
               // Stop sequential dialogue handling after all lines
            // Debug.Log("Line 153 called");
        }
    }

    private List<string> GetDialoguesForCurrentNight()
    {
        switch (nightManager.currentNight)
        {
            case 1: return currentNPCDialogue.dialoguesNight1;
            case 2: return currentNPCDialogue.dialoguesNight2;
            case 3: return currentNPCDialogue.dialoguesNight3;
            case 4: return currentNPCDialogue.dialoguesNight4;
            case 5: return currentNPCDialogue.dialoguesNight5;
            default: return new List<string>();
        }
    }

    private void DisplaySequentialResponse()
    {
        switch (activeResponseType)
        {
            case "Aswang":
                if (aswangResponseIndex < currentNPCDialogue.aswangResponses.Length)
                {
                    dialogueText.text = currentNPCDialogue.aswangResponses[aswangResponseIndex];
                    aswangResponseIndex++;
                }
                else
                {
                    EndSequentialDialogue();
                }
                break;

            case "Eclipse":
                if (eclipseResponseIndex < currentNPCDialogue.eclipseResponses.Length)
                {
                    dialogueText.text = currentNPCDialogue.eclipseResponses[eclipseResponseIndex];
                    eclipseResponseIndex++;
                }
                else
                {
                    EndSequentialDialogue();
                }
                break;

            case "Kill":
                if (killResponseIndex < currentNPCDialogue.killDialogues.Length)
                {
                    dialogueText.text = currentNPCDialogue.killDialogues[killResponseIndex];
                    killResponseIndex++;
                }
                else
                {
                    EndDialogue();
                }
                break;

            case "HearOutFeature1":
                if (hearOutDialogues1Index < currentNPCDialogue.hearOutDialogues1.Length)
                {
                    dialogueText.text = currentNPCDialogue.hearOutDialogues1[hearOutDialogues1Index];
                    hearOutDialogues1Index++;
                }
                else
                {
                    EndDialogue();
                }
                break;

            case "HearOutFeature2":
                if (hearOutDialogues2Index < currentNPCDialogue.hearOutDialogues2.Length)
                {
                    dialogueText.text = currentNPCDialogue.hearOutDialogues2[hearOutDialogues2Index];
                    hearOutDialogues2Index++;
                }
                else
                {
                    EndDialogue();
                }
                break;

            case "HearOutFeature3":
                if (hearOutDialogues3Index < currentNPCDialogue.hearOutDialogues3.Length)
                {
                    dialogueText.text = currentNPCDialogue.hearOutDialogues3[hearOutDialogues3Index];
                    hearOutDialogues3Index++;
                }
                else
                {
                    EndDialogue();
                }
                break;

            case "HearOutFeature4":
                if (hearOutDialogues4Index < currentNPCDialogue.hearOutDialogues4.Length)
                {
                    dialogueText.text = currentNPCDialogue.hearOutDialogues4[hearOutDialogues4Index];
                    hearOutDialogues4Index++;
                }
                else
                {
                    EndDialogue();
                }
                break;
        }
    }

    private void EndSequentialDialogue()
    {
        Debug.Log("Sequential dialogue ended.");
        activeResponseType = "";
        
        // Debug.Log("Line 265 called");
        buttonsPanel.SetActive(true);
        killButton.gameObject.SetActive(false);
        hearOutButton.gameObject.SetActive(false);
    }

    public void Option1Selected()
    {
        activeResponseType = "Aswang";
        aswangResponseIndex = 0; // Reset the index for sequential display
        buttonsPanel.SetActive(false);
        isInDialogue = true;
        DisplaySequentialResponse();
    }

    public void Option2Selected()
    {
        activeResponseType = "Eclipse";
        eclipseResponseIndex = 0;
        buttonsPanel.SetActive(false);
        isInDialogue = true;
        DisplaySequentialResponse();
    }

    public void Option3Selected()
    {
        if (!radioSystem.HasLearnedFeature(1) &&
            !radioSystem.HasLearnedFeature(2) &&
            !radioSystem.HasLearnedFeature(3) &&
            !radioSystem.HasLearnedFeature(4))
        {
            ShowLearnedNothingMessage();
            return;
        }

        // Hide main option buttons
        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
        option3Button.gameObject.SetActive(false);
        option4Button.gameObject.SetActive(false);

        // Display feature buttons based on learned traits
        elongateArmsButton.gameObject.SetActive(radioSystem.HasLearnedFeature(1));
        darkEyesButton.gameObject.SetActive(radioSystem.HasLearnedFeature(2));
        sharpTeethButton.gameObject.SetActive(radioSystem.HasLearnedFeature(3));
        falseSmileButton.gameObject.SetActive(radioSystem.HasLearnedFeature(4));

        SetupFeatureButtonListeners();
    }

    private void SetupFeatureButtonListeners()
    {
        ClearFeatureButtonListeners();
        elongateArmsButton.onClick.AddListener(() => OnFeatureButtonClick("Elongated Limbs"));
        darkEyesButton.onClick.AddListener(() => OnFeatureButtonClick("Dark Eyes"));
        sharpTeethButton.onClick.AddListener(() => OnFeatureButtonClick("Sharp Teeth"));
        falseSmileButton.onClick.AddListener(() => OnFeatureButtonClick("False Smile"));
    }

    private void ClearFeatureButtonListeners()
    {
        elongateArmsButton.onClick.RemoveAllListeners();
        darkEyesButton.onClick.RemoveAllListeners();
        sharpTeethButton.onClick.RemoveAllListeners();
        falseSmileButton.onClick.RemoveAllListeners();
    }

    private void OnFeatureButtonClick(string featureName)
    {
        lastClickedFeatureName = featureName; // Store the clicked feature name
        string featureDialogue = currentNPCDialogue.GetDialogueForFeature(featureName);
        dialogueText.text = featureDialogue;

        // Store the original NPC sprite and disable the NPC image
        originalNPCSprite = npcImage.sprite;
        npcImage.gameObject.SetActive(false); // Hide the NPC image

        // Display the feature image in the separate UI component
        FeatureDialogue selectedFeature = currentNPCDialogue.featureDialoguesList.Find(f => f.featureName == featureName);
        if (selectedFeature != null && selectedFeature.featureImage != null)
        {
            if (featureName == "Dark Eyes")
            {
                featureImageEyes.sprite = selectedFeature.featureImage;
                featureImageEyes.gameObject.SetActive(true); // Show the feature image
            }
            else if (featureName == "Sharp Teeth")
            {
                featureImageTeeth.sprite = selectedFeature.featureImage;
                featureImageTeeth.gameObject.SetActive(true); // Show the feature image
            }
            else if (featureName == "False Smile")
            {
                featureImageFalseSmile.sprite = selectedFeature.featureImage;
                featureImageFalseSmile.gameObject.SetActive(true); // Show the feature image
            }
            else
            {
                featureImage.sprite = selectedFeature.featureImage;
                featureImage.gameObject.SetActive(true); // Show the feature image
            }
        }
        // Hide feature buttons
        HideFeatureButtons();

        // Enable the Hear Out button and Kill button after selecting a feature
        Debug.Log("Crucial Choice");
        //ReduceEnergy();
        energyManager.ReduceEnergy();
        EnableHearButton();
        EnableKillButton(); // Ensure the Kill button is visible
    }

    private void EnableKillButton()
    {
        killButton.gameObject.SetActive(true);
        killButton.onClick.RemoveAllListeners();
        killButton.onClick.AddListener(() => {
            RestoreImagesOnKillHearOut(); // Restore images when kill button is clicked
            KillNPC();
        });
    }

    private void EnableHearButton()
    {
        hearOutButton.gameObject.SetActive(true);
        hearOutButton.onClick.RemoveAllListeners();

        hearOutButton.onClick.AddListener(() =>
        {
            RestoreImagesOnKillHearOut(); // Restore images when hear out button is clicked
            HearOutNPC();
        });
    }
    
    private void RestoreImagesOnKillHearOut()
    {
        // Re-enable the NPC image and disable the feature image
        if (npcImage != null)
        {
            npcImage.gameObject.SetActive(true);
        }

        if (featureImage != null)
        {
            featureImage.gameObject.SetActive(false); // Hide the feature image
        }
        if (featureImageEyes != null)
        {
            featureImageEyes.gameObject.SetActive(false); // Hide the feature image
        }
        if (featureImageTeeth != null)
        {
            featureImageTeeth.gameObject.SetActive(false); // Hide the feature image
        }
        if (featureImageFalseSmile != null)
        {
            featureImageFalseSmile.gameObject.SetActive(false); // Hide the feature image
        }
    }


    private void RestoreOriginalNPCImage()
    {
        if (originalNPCSprite != null)
        {
            npcImage.sprite = originalNPCSprite;
        }
    }

    public void KillNPC()
    {
        playerMovement.killCount = (playerMovement.killCount + 1);
        Debug.Log("Kill Count: " + playerMovement.killCount);
        currentNPCDialogue.isDead = true;
        activeResponseType = "Kill";
        killResponseIndex = 0;
        buttonsPanel.SetActive(false);
        isInDialogue = true;
        StartCoroutine(ShowKillDialogueAndEliminate());
    }

    private IEnumerator ShowKillDialogueAndEliminate()
    {
        DisplaySequentialResponse();

        GameObject npcObject = FindNPCGameObject(currentNPCDialogue);
        if (npcObject != null)
        {
            NPC npcComponent = npcObject.GetComponent<NPC>();
            if (npcComponent != null)
            {
                npcComponent.PlayGunCockSound();
                yield return new WaitForSeconds(2.0f);
                // Remove the NPC from the night manager's active list
                nightManager.RemoveGuestFromList(npcComponent);

                // Remove the NPC from the guestList if it's there
                if (nightManager.guestList.Contains(npcObject))
                {
                    nightManager.guestList.Remove(npcObject);
                }

                npcComponent.SwapToDeadSprite();
                // Destroy(npcObject);
            }
            else
            {
                Debug.LogWarning($"NPC component not found on GameObject for NPC: {currentNPCDialogue.npcName}");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find GameObject for NPC: {currentNPCDialogue.npcName}");
        }
        EndDialogue();
    }

    private GameObject FindNPCGameObject(NPCDialogue targetDialogue)
    {
        foreach (GameObject guest in nightManager.guestList)
        {
            NPC npcComponent = guest.GetComponent<NPC>();
            if (npcComponent != null && npcComponent.npcDialogue == targetDialogue)
            {
                if (npcComponent.uniqueID == targetDialogue.npcName)
                {
                    return guest;
                }
            }
        }
        return null;
    }

    private GameObject GetDeadBodyPrefabForNPC(string npcName)
    {
        NPCDeadBodyPair pair = npcDeadBodyPairs.Find(p => p.npcName == npcName);
        return pair != null ? pair.deadBodyPrefab : null;
    }

    private void HearOutNPC()
    {
        // Determine which response to use based on the last clicked feature
        switch (lastClickedFeatureName)
        {
            case "Elongated Limbs":
                activeResponseType = "HearOutFeature1"; // Set the active response type for Hear Out
                break;

            case "Dark Eyes":
                activeResponseType = "HearOutFeature2"; // Set the active response type for Hear Out
                break;

            case "Sharp Teeth":
                activeResponseType = "HearOutFeature3"; // Set the active response type for Hear Out
                break;

            case "False Smile":
                activeResponseType = "HearOutFeature4"; // Set the active response type for Hear Out
                break;

            default:
                dialogueText.text = "No response for this feature.";
                break;
        }

        // Start the sequential dialogue for Hear Out
        DisplaySequentialResponse();
    }

    private void HideFeatureButtons()
    {
        elongateArmsButton.gameObject.SetActive(false);
        darkEyesButton.gameObject.SetActive(false);
        sharpTeethButton.gameObject.SetActive(false);
        falseSmileButton.gameObject.SetActive(false);
    }

    private void ShowLearnedNothingMessage()
    {
        messagePanel.SetActive(true);
        messagePanelBG.gameObject.SetActive(true);
        messageText.text = "You haven't learned anything about the Aswangs.";
        exitButton.gameObject.SetActive(true);
    }
    private void ExitLearnedNothingMessage()
    {
        messagePanel.SetActive(false);
        messagePanelBG.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        dialogueText.text = "";

        // End the dialogue after exiting, remove if ugly
        EndDialogue();
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        npcImage.gameObject.SetActive(false);

        if (featureImage != null)
        {
            featureImage.gameObject.SetActive(false);
        }
        if (featureImageEyes != null)
        {
            featureImageEyes.gameObject.SetActive(false);
        }
        if (featureImageTeeth != null)
        {
            featureImageTeeth.gameObject.SetActive(false);
        }
        if (featureImageFalseSmile != null)
        {
            featureImageFalseSmile.gameObject.SetActive(false);
        }

        buttonsPanel.SetActive(false);
        isInDialogue = false;
        ResetButtons();
    }




    private void ResetButtons()
    {
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);
        option3Button.gameObject.SetActive(true);
        option4Button.gameObject.SetActive(true);
        
        // Reset responses
        killButton.gameObject.SetActive(false);
        hearOutButton.gameObject.SetActive(false);
    }

    private void InitializeFeatureButtons()
    {
        elongateArmsButton.gameObject.SetActive(false);
        darkEyesButton.gameObject.SetActive(false);
        sharpTeethButton.gameObject.SetActive(false);
        falseSmileButton.gameObject.SetActive(false);
    }
    
    
    public void ShowCannotTalkMessage()
    {
        if (cannotTalkPanel != null && cannotTalkText != null)
        {
            cannotTalkText.text = "You no longer have the energy to speak with guests. You should go to bed.";
            cannotTalkPanel.SetActive(true);
            cannotTalkText.enabled = true; // Explicitly enable the text
            StartCoroutine(HideCannotTalkMessage());
        }
        else
        {
            Debug.LogWarning("Cannot Sleep Panel or Text is not assigned in the inspector!");
        }
    }


    private IEnumerator HideCannotTalkMessage()
    {
        yield return new WaitForSeconds(cannotTalkMessageDuration);
        cannotTalkText.enabled = false; // Disable the text first
        cannotTalkPanel.SetActive(false); // Then disable the panel
    }
}
