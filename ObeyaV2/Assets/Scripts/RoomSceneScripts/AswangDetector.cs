using UnityEngine;

public class AswangDetector : MonoBehaviour
{
    public AudioClip crowSound;
    private AudioSource audioSource;
    private GuestManager guestManager;
    private NightManager nightManager;
    private float soundTimer = 0f;
    private const float SOUND_INTERVAL = 30f; // 30 seconds

    void Start()
    {
        // Get required components
        audioSource = gameObject.AddComponent<AudioSource>();
        guestManager = FindObjectOfType<GuestManager>();
        nightManager = FindObjectOfType<NightManager>();
    }

    void Update()
    {
        if (nightManager != null && nightManager.currentNight == 4)
        {
            soundTimer += Time.deltaTime;
            
            if (soundTimer >= SOUND_INTERVAL)
            {
                CheckForAswang();
                soundTimer = 0f; // Reset timer
            }
        }
    }

    void CheckForAswang()
    {
        if (nightManager.guestList != null && nightManager.guestList.Count > 0)
        {
            bool aswangFound = false;
            foreach (GameObject guest in nightManager.guestList)
            {
                // Check if the guest's name or tag contains "Aswang"
                if (guest.CompareTag("Aswang") || guest.name.Contains("Aswang"))
                {
                    aswangFound = true;
                    break;
                }
            }

            if (aswangFound)
            {
                PlayCrowSound();
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