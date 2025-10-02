using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class AdaptiveAsyncLoading : MonoBehaviour
{
    [Header("UI References")]
    public Image loadingBarFill;          // Green bar (set Image Type = Filled)
    public TextMeshProUGUI loadingText;   // TMP text for % progress
    public string sceneToLoad = "MainMenu"; // Scene name to load

    private float loadMultiplier;

    private void Start()
    {
        // Detect device specs
        int ram = SystemInfo.systemMemorySize; // RAM in MB
        int cpuCores = SystemInfo.processorCount;

        // Adjust multiplier based on specs
        if (ram >= 6000 && cpuCores >= 8) loadMultiplier = 1.5f;   // High-end
        else if (ram >= 3000 && cpuCores >= 4) loadMultiplier = 1f; // Mid-range
        else loadMultiplier = 0.7f;                                 // Low-end

        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false; // Wait until we hit 100%

        float progress = 0f;

        while (!op.isDone)
        {
            // Unity's real loading progress (0 → 0.9, then waits)
            float targetProgress = Mathf.Clamp01(op.progress / 0.9f);

            // Smooth progress influenced by phone specs
            progress = Mathf.MoveTowards(progress, targetProgress, Time.deltaTime * loadMultiplier);

            // Update UI
            loadingBarFill.fillAmount = progress;
            loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";

            // When fully loaded, activate the scene
            if (progress >= 1f && op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.3f); // Small delay for smoothness
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
