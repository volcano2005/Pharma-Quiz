using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [Header("Profile UI Elements")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI bestStreakText;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI TimeRegeneration_Time;

    [Header("Name Edit Buttons")]
    public Button editNameButton;
    public Button saveNameButton;

    [Header("Live Data from GameManager")]
    public TextMeshProUGUI currentHeartsText;
    public TextMeshProUGUI currentStreakText;
    public TextMeshProUGUI currentXPText;

    private bool isEditingName = false;

    private void Start()
    {
        // Profile Manager bindings
        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.OnProfileUpdated += UpdateProfileUI;
            ProfileManager.Instance.OnNameChanged += UpdateName;
            ProfileManager.Instance.OnBestStreakUpdated += UpdateBestStreak;

            // Force initial update
            UpdateProfileUI(ProfileManager.Instance.GetProfile());
        }

        // Game Manager bindings
        if (GameManager.I != null)
        {
            GameManager.I.OnHeartsUpdated += UpdateLiveData;
            GameManager.I.OnXPUpdated += UpdateLiveData;
        }

        SetEditMode(false);
        UpdateLiveData();

        // Update timer every second instead of every frame
        InvokeRepeating(nameof(UpdateTimerUI), 1f, 1f);
    }

    // --- Profile UI ---
    private void UpdateProfileUI(PlayerProfile profile)
    {
        if (playerNameText) playerNameText.text = profile.playerName;
        if (bestStreakText) bestStreakText.text = $"Best Streak: {profile.bestStreak}";
        if (nameInputField) nameInputField.text = profile.playerName;
    }

    private void UpdateName(string newName)
    {
        if (playerNameText) playerNameText.text = newName;
        if (nameInputField) nameInputField.text = newName;
    }

    private void UpdateBestStreak(int bestStreak)
    {
        if (bestStreakText) bestStreakText.text = $"Best Streak: {bestStreak}";
        Debug.Log($"New best streak: {bestStreak}!");
    }

    // --- Game Live Data ---
    private void UpdateLiveData()
    {
        if (GameManager.I == null) return;

        if (currentHeartsText != null)
            currentHeartsText.text = $"{GameManager.I.hearts}/{GameManager.I.maxHearts}";

        if (currentStreakText != null)
            currentStreakText.text = $"Streak: {GameManager.I.streakCount}";

        if (currentXPText != null)
            currentXPText.text = $"XP: {GameManager.I.xp}";
    }

    private void UpdateTimerUI()
    {
        GameManager G = GameManager.I;
        if (G == null || G.hearts >= G.maxHearts)
        {
            if (TimeRegeneration_Time != null)
                TimeRegeneration_Time.text = "";
            return;
        }

        if (TimeRegeneration_Time != null)
            TimeRegeneration_Time.text = G.GetFormattedTimeLeft();
    }


    // --- Name Editing ---
    public void OnEditNameClicked()
    {
        SetEditMode(true);

        if (nameInputField != null)
        {
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
    }

    public void OnSaveNameClicked()
    {
        if (nameInputField != null && ProfileManager.Instance != null)
        {
            string newName = nameInputField.text.Trim();
            if (!string.IsNullOrEmpty(newName))
            {
                ProfileManager.Instance.SetPlayerName(newName);
            }
        }
        SetEditMode(false);
    }

    private void SetEditMode(bool editing)
    {
        isEditingName = editing;

        if (playerNameText) playerNameText.gameObject.SetActive(!editing);
        if (nameInputField) nameInputField.gameObject.SetActive(editing);
        if (editNameButton) editNameButton.gameObject.SetActive(!editing);
        if (saveNameButton) saveNameButton.gameObject.SetActive(editing);
    }

    private void Update()
    {
        if (isEditingName)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnSaveNameClicked();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (nameInputField && ProfileManager.Instance != null)
                    nameInputField.text = ProfileManager.Instance.GetProfile().playerName;

                SetEditMode(false);
            }
        }
        UpdateTimerUI();
    }

    // --- Navigation ---
    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
