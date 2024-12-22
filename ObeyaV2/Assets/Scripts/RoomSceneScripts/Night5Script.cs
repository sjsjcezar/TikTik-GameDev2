using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Night5Script : NightBaseScript
{
    [Header("UI Elements")]
    public TextMeshProUGUI endingText;
    public GameObject endingPanel;
    public TextMeshProUGUI scoreText;
    public Button titleScreenButton;
    public float textDisplayDuration = 3f;

    [Header("Typewriter Effect")]
    public float typingSpeed = 0.05f;  // Time between each character
 //   public AudioClip typingSoundEffect;
 //   private AudioSource audioSource;
    
    private EnergyManager energyManager;
    private RadioSystem radioSystem;
    private PlayerMovement playerMovement;
    private int finalScore = 0;

    void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
        radioSystem = FindObjectOfType<RadioSystem>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        // Initially disable score and button
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (titleScreenButton != null)
        {
            titleScreenButton.gameObject.SetActive(false);
            titleScreenButton.onClick.AddListener(() => SceneManager.LoadScene("TitleScreen"));
        }
    }

    // Public method to trigger OnNightComplete
    public void TriggerNightComplete()
    {
        OnNightComplete();
    }

    protected override void OnNightComplete()
    {
        energyManager.DisableEnergy();
        radioSystem.DisableJournal();

        NightManager nightManager = FindObjectOfType<NightManager>();
        energyManager = FindObjectOfType<EnergyManager>();
        radioSystem = FindObjectOfType<RadioSystem>();
        
        int humansAccepted = 0;
        int aswangsAccepted = 0;

        // Count accepted guests
        foreach (var guest in nightManager.guestList)
        {
            if (guest.CompareTag("Human"))
            {
                humansAccepted++;
            }
            else if (guest.CompareTag("Aswang"))
            {
                aswangsAccepted++;
            }
        }

        // Calculate final score
        finalScore = (humansAccepted * 25) - (aswangsAccepted * 15);

        if (humansAccepted >= 6 && aswangsAccepted == 0)
        {
            StartCoroutine(DisplayEnding("Best Ending: GGS! You've successfully protected your house! You have a keen eye in identifying and accepting only humans."));
        }
        else if (humansAccepted >= 4 && aswangsAccepted == 0 && playerMovement.killCount <= 4)
        {
            StartCoroutine(DisplayEnding("Good Ending: You've done well in protecting your house, though some humans were left for dead."));
        }
        else if (humansAccepted >= 2 && aswangsAccepted == 0 && playerMovement.killCount <= 5)
        {
            StartCoroutine(DisplayEnding("Basic Ending: You survived, but many were left for dead."));
        }
        else if (playerMovement.killCount >= 6 && aswangsAccepted >= 0 && humansAccepted >= 0)
        {
            StartCoroutine(DisplayEnding("Violent Ending: Your paranoia led to unnecessary bloodshed. The shadows of your actions continue to haunt you, and in the end, you decided to pull the trigger."));
        }
        else if (playerMovement.killCount >= 12 && aswangsAccepted >= 0 && humansAccepted >= 0)
        {
            StartCoroutine(DisplayEnding("Bloodlust Ending: Your bloodlust has led you to murder. You've killed all the guests, leaving you with no bullets to spare. The aswangs have overrun your home."));
        } 
        else if (aswangsAccepted >= 1 && humansAccepted >= 0)
        {
            StartCoroutine(DisplayEnding("Bad Ending: You've unknowingly aided the aswangs. They have overrun your home."));
        }
        else
        {
            StartCoroutine(DisplayEnding("Game Over: Your indecision has led to chaos."));
        }
    }

    private IEnumerator DisplayEnding(string message)
    {
        endingPanel.SetActive(true);
        endingText.text = message;
        endingText.text = ""; // Clear the text first

        // Typewriter effect
        foreach (char letter in message.ToCharArray())
        {
            endingText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(textDisplayDuration);

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.text = $"Score: {finalScore}";
        }

        if (titleScreenButton != null)
        {
            titleScreenButton.gameObject.SetActive(true);
        }
    }
}