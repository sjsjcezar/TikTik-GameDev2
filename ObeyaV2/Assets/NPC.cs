using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class NPC : MonoBehaviour
{
    public NPCDialogue npcDialogue;
    public SpriteRenderer spriteRenderer;
    public Sprite deadBodySprite;
    private Animator animator;
    public string uniqueID;
    private bool isDead = false;
    public GameObject killPanel;
    public float fadeOutDuration = 3f;
    
    [Header("Kill Sequence")]
    public AudioClip gunCockSound;
    public AudioClip gunShotSound;
    public float blackScreenDuration = 3f;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayGunCockSound()
    {
        if (gunCockSound != null)
        {
            audioSource.PlayOneShot(gunCockSound);
        }
    }

    public void SwapToDeadSprite()
    {
        StartCoroutine(KillSequenceCoroutine());
    }

    private IEnumerator KillSequenceCoroutine()
    {
        // Instantly show black panel
        killPanel.SetActive(true);
        CanvasGroup canvasGroup = killPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = killPanel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        // Play gunshot sound immediately with black screen
        if (gunShotSound != null)
        {
            audioSource.PlayOneShot(gunShotSound);
        }

        // During the black screen, swap the sprite
        if (animator != null)
        {
            animator.enabled = false;
        }

        if (gameObject.CompareTag("Human"))
        {
            spriteRenderer.sprite = deadBodySprite;
        }
        else if (gameObject.CompareTag("Aswang") && deadBodySprite != null)
        {
            spriteRenderer.sprite = deadBodySprite;
        }
        
        isDead = true;

        // Keep black screen for specified duration
        yield return new WaitForSeconds(blackScreenDuration);

        // Start fade out
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }

        // Ensure panel is fully transparent and disabled
        canvasGroup.alpha = 0f;
        killPanel.SetActive(false);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool HasFeature(int featureIndex)
    {
        return featureIndex >= 0 && featureIndex < npcDialogue.featureDialoguesList.Count;
    }
}