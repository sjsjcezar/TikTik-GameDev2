using UnityEngine;

public class NightBasedSpriteSwapper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NightManager nightManager;
    private SpriteRenderer spriteRenderer;

    [Header("Sprite Swap Settings")]
    [Tooltip("The night number when the sprite should change")]
    [SerializeField] private int nightToSwapOn = 2;
    [SerializeField] private Sprite newSprite;
    [SerializeField] private bool hasSwapped = false;

    private void Start()
    {
        if (nightManager == null)
            nightManager = FindObjectOfType<NightManager>();
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (!hasSwapped && nightManager.currentNight >= nightToSwapOn)
        {
            if (newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
                hasSwapped = true;
            }
            else
            {
                Debug.LogWarning("No new sprite assigned for the swap!");
            }
        }
    }
}