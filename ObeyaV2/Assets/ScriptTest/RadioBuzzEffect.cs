using TMPro;
using UnityEngine;

public class RadioBuzzEffect : MonoBehaviour
{
    [Header("Text Animation Settings")]
    public TMP_Text radioText;
    public string[] buzzPatterns = { "bzz", "bzzz", "bzzzz", "bzz bzz", "bzzz bzz" };
    public float textChangeInterval = 0.5f;

    [Header("Vibration Effect Settings")]
    public bool enableVibration = true;
    public float vibrationMagnitude = 1.0f;
    public float vibrationSpeed = 10.0f;

    private Vector3 initialLocalPosition;
    private float timer;

    private void Start()
    {
        if (radioText == null)
        {
            enabled = false;
            return;
        }

        initialLocalPosition = radioText.rectTransform.localPosition;
        timer = textChangeInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            radioText.text = buzzPatterns[Random.Range(0, buzzPatterns.Length)];
            timer = textChangeInterval;
        }

        if (enableVibration)
        {
            float xOffset = Mathf.Sin(Time.time * vibrationSpeed) * vibrationMagnitude;
            float yOffset = Mathf.Cos(Time.time * vibrationSpeed) * vibrationMagnitude;
            radioText.rectTransform.localPosition = initialLocalPosition + new Vector3(xOffset, yOffset, 0);
        }
    }

    private void OnDisable()
    {
        if (radioText != null)
        {
            radioText.rectTransform.localPosition = initialLocalPosition;
        }
    }
}
