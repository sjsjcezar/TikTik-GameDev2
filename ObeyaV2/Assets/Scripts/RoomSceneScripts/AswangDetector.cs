using UnityEngine;

public class AswangDetector : MonoBehaviour
{
    public AudioClip crowSound;
    private AudioSource audioSource;
    private GuestManager guestManager;
    private NightManager nightManager;
    private bool hasPlayedCrowTonight = false;
    private bool hasCheckedNightFour = false;  // New flag to track if we've done the night 4 check

    void Start()
    {
        // Get required components
        audioSource = gameObject.AddComponent<AudioSource>();
        guestManager = FindObjectOfType<GuestManager>();
        nightManager = FindObjectOfType<NightManager>();
    }

    void Update()
    {
        // Only check if we haven't done the night 4 check yet
        if (!hasCheckedNightFour && nightManager != null && nightManager.currentNight == 4)
        {
            CheckForAswang();
            hasCheckedNightFour = true;  // Mark that we've done the night 4 check
        }
    }

    void CheckForAswang()
    {
        if (nightManager.guestList != null && nightManager.guestList.Count > 0)
        {
            foreach (GameObject guest in nightManager.guestList)
            {
                // Check if the guest's name or tag contains "Aswang"
                if (guest.CompareTag("Aswang") || guest.name.Contains("Aswang"))
                {
                    PlayCrowSound();
                    break;
                }
            }
        }
    }

    void PlayCrowSound()
    {
        if (crowSound != null)
        {
            audioSource.clip = crowSound;
            audioSource.Play();
        }
    }
}
