using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StarAnimationManager : MonoBehaviour
{
    [Header("Star UI References")]
    public Image[] starIcons; // 3 star icons
    public Sprite filledStar; // Filled star sprite
    public Sprite emptyStar;  // Empty star sprite

    [Header("Animation Settings")]
    public float scaleUpDuration = 0.2f;  // Duration of the scaling effect
    public float scaleMax = 1.2f;         // Scale value when star grows
    public float pulseSpeed = 1f;         // Speed of the glow pulse effect

    private int currentStars = 0;

    void Start()
    {
        // Reset stars at the start
        foreach (var star in starIcons)
        {
            star.sprite = emptyStar;
            star.transform.localScale = Vector3.one;  // Reset scale
        }
    }

    public void UnlockStar(int starIndex)
    {
        if (starIndex < starIcons.Length && starIcons[starIndex].sprite == emptyStar)
        {
            // Set the star to filled
            starIcons[starIndex].sprite = filledStar;

            // Trigger the star animation
            StartCoroutine(AnimateStar(starIcons[starIndex].transform));

            // Optional: You can add a sound effect here when the star is unlocked
            // AudioManager.PlaySound("StarUnlock"); // For example

            currentStars++;
        }
    }

    private IEnumerator AnimateStar(Transform starTransform)
    {
        // Scale up the star
        float elapsedTime = 0;
        Vector3 originalScale = starTransform.localScale;
        Vector3 targetScale = originalScale * scaleMax;

        while (elapsedTime < scaleUpDuration)
        {
            starTransform.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / scaleUpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        starTransform.localScale = targetScale;

        // Optional: Add a glowing pulse effect (if needed)
        StartCoroutine(PulseStar(starTransform));

        // Return the star to its normal size after the animation is complete
        elapsedTime = 0;
        while (elapsedTime < scaleUpDuration)
        {
            starTransform.localScale = Vector3.Lerp(targetScale, originalScale, (elapsedTime / scaleUpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        starTransform.localScale = originalScale;
    }

    private IEnumerator PulseStar(Transform starTransform)
    {
        // Make the star pulse/glow
        float pulseTime = 0;
        Color originalColor = starTransform.GetComponent<Image>().color;

        while (true)
        {
            float pulseFactor = Mathf.PingPong(pulseTime * pulseSpeed, 0.2f);  // Glowing effect range
            starTransform.GetComponent<Image>().color = new Color(1f, 1f, 0f, 1f - pulseFactor); // Yellow color

            pulseTime += Time.deltaTime;
            yield return null;
        }
    }
}
