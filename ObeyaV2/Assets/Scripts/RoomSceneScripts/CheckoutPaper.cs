using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CheckoutPaper : MonoBehaviour
{
    [System.Serializable]
    public class GuestEntry
    {
        public string guestName;
        public bool[] featureChecks = new bool[4]; // Limbs, Eyes, Teeth, Smile
        public GameObject entryObject; // The UI object containing this guest's row
        public Image crossoutImage; // Reference to the crossout image
        public Button[] checkboxButtons; // Array of checkbox buttons
        public GuestData guestData; // Add this line
        public bool isKilled = false;
    }

    [Header("UI References")]
    public GameObject guestEntryPrefab; // Prefab for each guest's row
    public Transform contentParent; // Parent transform for the guest entries (grid layout)
    public Sprite uncheckedBoxSprite;
    public Sprite checkedBoxSprite;
    public TextMeshProUGUI[] featureHeaders; // Headers for each feature column
    public GameObject paperUI; // Reference to the entire paper UI panel
    public Button enableUIButton;

    [Header("System References")]
    public RadioSystem radioSystem;
    public NightManager nightManager;
    public GuestManager guestManager;

    private List<GuestEntry> guestEntries = new List<GuestEntry>();
    private readonly string[] featureNames = { "Limbs", "Eyes", "Teeth", "Smile" };

    void Start()
    {
        UpdateFeatureHeaders();

        if (radioSystem != null)
        {
            radioSystem.OnFeatureLearned += OnFeatureLearned;
        }

        if (guestManager != null)
        {
            guestManager.OnGuestAccepted += OnGuestAccepted;
        }

        // Set up the enable UI button
        if (enableUIButton != null)
        {
            enableUIButton.onClick.AddListener(TogglePaperUI);
        }

        // Initially hide the paper UI
        if (paperUI != null)
        {
            paperUI.SetActive(false);
        }
    }
    void OnFeatureLearned()
    {
        Debug.Log("Feature learned - updating all guest entries");
        UpdateFeatureHeaders();
        
        // Update all existing guest entries
        foreach (GuestEntry entry in guestEntries)
        {
            UpdateAllGuestButtons(entry);
        }
    }

    void TogglePaperUI()
    {
        if (paperUI != null)
        {
            paperUI.SetActive(!paperUI.activeSelf);
        }
    }

    void UpdateAllGuestButtons(GuestEntry entry)
    {
        if (entry.checkboxButtons == null) return;

        // Enable buttons based on learned features
        for (int i = 0; i < entry.checkboxButtons.Length && i < 4; i++)
        {
            Button checkbox = entry.checkboxButtons[i];
            if (checkbox == null) continue;

            // Only enable if the feature is learned and guest isn't killed
            bool featureLearned = IsFeatureLearned(i);
            checkbox.interactable = featureLearned && !entry.isKilled;
            
            // Make sure the button shows the correct sprite
            Image checkboxImage = checkbox.GetComponent<Image>();
            if (checkboxImage != null)
            {
                checkboxImage.sprite = entry.featureChecks[i] ? checkedBoxSprite : uncheckedBoxSprite;
            }
        }
    }

    void UpdateFeatureHeaders()
    {
        for (int i = 0; i < featureHeaders.Length; i++)
        {
            bool isFeatureLearned = IsFeatureLearned(i);
            featureHeaders[i].text = isFeatureLearned ? featureNames[i] : "??";
        }
    }

    bool IsFeatureLearned(int featureIndex)
    {
        if (radioSystem == null) return false;
        
        switch (featureIndex)
        {
            case 0: return radioSystem.hasLearnedFeature1; // Limbs
            case 1: return radioSystem.hasLearnedFeature2; // Eyes
            case 2: return radioSystem.hasLearnedFeature3; // Teeth
            case 3: return radioSystem.hasLearnedFeature4; // Smile
            default: return false;
        }
    }

    void Update()
    {
        // Use the same toggle method for both button and 'N' key
        if (Input.GetKeyDown(KeyCode.N))
        {
            TogglePaperUI();
        }

        // Update crossout images based on NPCDialogue.isDead
        foreach (GuestEntry entry in guestEntries)
        {
            if (entry.guestData != null && entry.guestData.npcDialogue != null)
            {
                if (entry.crossoutImage != null)
                {
                    entry.crossoutImage.gameObject.SetActive(entry.guestData.npcDialogue.isDead);
                }
                UpdateGuestButtons(entry);
            }
        }
    }

    void OnGuestAccepted()
    {
        Debug.Log("OnGuestAccepted called!");
        // Get the latest guest from NightManager's list
        if (nightManager.guestList.Count > 0)
        {
            Debug.Log($"Found {nightManager.guestList.Count} guests in NightManager");
            GameObject latestGuest = nightManager.guestList[nightManager.guestList.Count - 1];
            GuestData guestData = latestGuest.GetComponent<GuestData>();
            Debug.Log($"Latest guest name: {(guestData != null ? guestData.npcDialogue.npcName : "No GuestData found")}");
            
            if (!IsGuestInEntries(latestGuest))
            {
                Debug.Log("Adding new guest to entries");
                AddNewGuest(latestGuest);
            }
            else
            {
                Debug.Log("Guest already in entries");
            }
        }
        else
        {
            Debug.Log("No guests in NightManager's list");
        }
    }

    bool IsGuestInEntries(GameObject guest)
    {
        GuestData guestData = guest.GetComponent<GuestData>();
        if (guestData == null) return false;

        return guestEntries.Exists(entry => entry.guestName == guestData.npcDialogue.npcName);
    }

    void AddNewGuest(GameObject guest)
    {
        GuestData guestData = guest.GetComponent<GuestData>();
        if (guestData == null) return;

        // Create new guest entry
        GuestEntry newEntry = new GuestEntry
        {
            guestName = guestData.npcDialogue.npcName,
            featureChecks = new bool[4],
            entryObject = Instantiate(guestEntryPrefab, contentParent),
            guestData = guestData  // Store the GuestData reference
        };

        SetupGuestEntryUI(newEntry);
        guestEntries.Add(newEntry);
    }

    void SetupGuestEntryUI(GuestEntry entry)
    {
        // Set guest name
        TextMeshProUGUI nameText = entry.entryObject.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = entry.guestName;
        }

        // Get crossout image reference
        entry.crossoutImage = entry.entryObject.transform.Find("CrossoutImage")?.GetComponent<Image>();
        if (entry.crossoutImage != null)
        {
            entry.crossoutImage.gameObject.SetActive(entry.guestData.npcDialogue.isDead);
        }

        // Setup checkboxes
        Button[] checkboxes = entry.entryObject.GetComponentsInChildren<Button>();
        entry.checkboxButtons = checkboxes;

        // Add click handlers for each button
        for (int i = 0; i < entry.checkboxButtons.Length && i < 4; i++)
        {
            int index = i; // Capture the index for the lambda
            Button checkbox = entry.checkboxButtons[i];
            Image checkboxImage = checkbox.GetComponent<Image>();
            
            checkbox.onClick.RemoveAllListeners();
            checkbox.onClick.AddListener(() => {
                entry.featureChecks[index] = !entry.featureChecks[index];
                checkboxImage.sprite = entry.featureChecks[index] ? checkedBoxSprite : uncheckedBoxSprite;
            });
        }

        UpdateGuestButtons(entry);
    }

    
    void UpdateGuestButtons(GuestEntry entry)
    {
        if (entry.checkboxButtons == null) return;

        bool isGuestDead = entry.guestData != null && entry.guestData.npcDialogue != null && entry.guestData.npcDialogue.isDead;

        for (int i = 0; i < entry.checkboxButtons.Length && i < 4; i++)
        {
            Button checkbox = entry.checkboxButtons[i];
            if (checkbox == null) continue;

            // Only enable if the feature is learned and guest isn't dead
            bool featureLearned = IsFeatureLearned(i);
            checkbox.interactable = featureLearned && !isGuestDead;
            
            Image checkboxImage = checkbox.GetComponent<Image>();
            if (checkboxImage != null)
            {
                checkboxImage.sprite = entry.featureChecks[i] ? checkedBoxSprite : uncheckedBoxSprite;
            }
        }
    }

    void UpdateCheckboxInteractability(GuestEntry entry)
    {
        if (entry.checkboxButtons == null) return;

        for (int i = 0; i < entry.checkboxButtons.Length && i < 4; i++)
        {
            int featureIndex = i; // Capture for lambda
            Button checkbox = entry.checkboxButtons[i];
            Image checkboxImage = checkbox.GetComponent<Image>();
            
            // Only enable button if feature is learned and guest is not killed
            checkbox.interactable = IsFeatureLearned(featureIndex) && !entry.isKilled;
            
            // Add or update click listener
            checkbox.onClick.RemoveAllListeners();
            checkbox.onClick.AddListener(() => {
                if (!entry.isKilled)
                {
                    entry.featureChecks[featureIndex] = !entry.featureChecks[featureIndex];
                    checkboxImage.sprite = entry.featureChecks[featureIndex] ? checkedBoxSprite : uncheckedBoxSprite;
                }
            });
        }
    }

    public void MarkGuestAsKilled(string guestName)
    {
        GuestEntry entryToKill = guestEntries.Find(entry => entry.guestName == guestName);
        if (entryToKill != null && !entryToKill.isKilled)
        {
            Debug.Log($"Marking guest as killed: {guestName}");
            entryToKill.isKilled = true;
            
            // Show crossout
            if (entryToKill.crossoutImage != null)
            {
                entryToKill.crossoutImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("CrossoutImage not found for guest: " + guestName);
            }

            // Disable all checkboxes
            UpdateAllGuestButtons(entryToKill);
        }
    }

    void OnDestroy()
    {
        if (radioSystem != null)
        {
            radioSystem.OnFeatureLearned -= OnFeatureLearned;
        }
        if (guestManager != null)
        {
            guestManager.OnGuestAccepted -= OnGuestAccepted;
        }
        if (enableUIButton != null)
        {
            enableUIButton.onClick.RemoveListener(TogglePaperUI);
        }
    }
}