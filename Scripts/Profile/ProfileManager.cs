using System;
using UnityEngine;
using System.IO;
using System.Collections;
using Unity.VisualScripting;

#region Player Profile Data
/// <summary>
/// A serializable class to hold all player data. This can be easily converted to and from JSON.
/// </summary>
[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    public int hearts;
    public int maxHearts;
    public int currentStreak;
    public int bestStreak;
    public int xp;
    public DateTime lastPlayed;

    /// <summary>
    /// Initializes a new PlayerProfile with default values.
    /// </summary>
    public PlayerProfile()
    {
        playerName = "Player";
        hearts = 10;
        maxHearts = 10;
        currentStreak = 0;
        bestStreak = 0;
        xp = 0;
        lastPlayed = DateTime.UtcNow;
    }
}
#endregion

#region Profile Manager
/// <summary>
/// A singleton class that manages saving and loading player profiles to a file.
/// It syncs data with the GameManager to ensure consistency.
/// </summary>
public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    [Header("Profile Settings")]
    [Tooltip("The name of the profile data file.")]
    public string profileFileName = "profile.json";

    private PlayerProfile currentProfile;
    private string profilePath;

    // Events to notify other systems of profile changes
    public event Action<PlayerProfile> OnProfileUpdated;
    public event Action<string> OnNameChanged;
    public event Action<int> OnBestStreakUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(WaitForGameManagerAndLoad());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Coroutine that waits for the GameManager to be initialized before loading and syncing data.
    /// This is crucial for ensuring the order of operations is correct.
    /// </summary>
    private IEnumerator WaitForGameManagerAndLoad()
    {
        // Step 1: Wait for GameManager to be ready.
        while (GameManager.I == null)
        {
            yield return null;
        }

        // Step 2: Load the profile from the file or create a new one.
        profilePath = Path.Combine(Application.persistentDataPath, profileFileName);

        if (File.Exists(profilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(profilePath);
                currentProfile = JsonUtility.FromJson<PlayerProfile>(jsonData);
                Debug.Log("Profile loaded successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading profile: {e.Message}. Creating a new profile.");
                CreateNewProfile();
            }
        }
        else
        {
            CreateNewProfile();
        }

        // Step 3: Now that both are ready, sync the profile data to the GameManager.
        if (currentProfile != null)
        {
            GameManager.I.hearts = currentProfile.hearts;
            GameManager.I.xp = currentProfile.xp;
            GameManager.I.streakCount = currentProfile.currentStreak;
            // Note: highestLessonUnlocked is still handled by GameManager for now.
        }

        // Step 4: Subscribe to events for future changes and update UI.
        GameManager.I.OnHeartsUpdated += SyncHeartsFromGameManager;
        GameManager.I.OnXPUpdated += SyncXPFromGameManager;
        // Assuming you have an OnStreakUpdated event in GameManager
        // GameManager.I.OnStreakUpdated += SyncStreakFromGameManager; 

        // Update the UI after everything is loaded
        GameManager.I.UpdateHeartsUI();
        GameManager.I.UpdateStreakUI();

        // Final save to make sure everything is consistent.
        SaveProfile();
    }

    /// <summary>
    /// Saves the current player profile to a JSON file.
    /// </summary>
    public void SaveProfile()
    {
        try
        {
            if (currentProfile == null)
            {
                Debug.LogWarning("SaveProfile called but currentProfile is null. Creating new profile.");
                currentProfile = new PlayerProfile();
            }

            currentProfile.lastPlayed = DateTime.UtcNow;
            string jsonData = JsonUtility.ToJson(currentProfile, true);
            File.WriteAllText(profilePath, jsonData);
            OnProfileUpdated?.Invoke(currentProfile);
            Debug.Log("Profile saved to: " + profilePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving profile: {e.Message}");
        }
    }

    /// <summary>
    /// Resets the player profile by deleting the file and creating a new one.
    /// Accessible from the Unity context menu.
    /// </summary>
    [ContextMenu("Reset Profile")]
    public void ResetProfile()
    {
        if (File.Exists(profilePath))
        {
            File.Delete(profilePath);
        }
        CreateNewProfile();
        Debug.Log("Profile has been reset.");
    }

    /// <summary>
    /// Creates a new player profile with default values.
    /// </summary>
    public void CreateNewProfile()
    {
        currentProfile = new PlayerProfile();
        SaveProfile();
    }

    /// <summary>
    /// Returns the current PlayerProfile object.
    /// </summary>
    public PlayerProfile GetProfile()
    {
        return currentProfile;
    }

    /// <summary>
    /// Sets the player's name and saves the profile.
    /// </summary>
    /// <param name="name">The new name for the player.</param>
    public void SetPlayerName(string name)
    {
        currentProfile.playerName = name;
        OnNameChanged?.Invoke(name);
        SaveProfile();
    }

    /// <summary>
    /// Syncs data from the GameManager to the PlayerProfile object.
    /// This is the primary method for keeping the data consistent before saving.
    /// </summary>
    public void SyncFromGameManager()
    {
        if (GameManager.I == null || currentProfile == null) return;

        currentProfile.hearts = GameManager.I.hearts;
        currentProfile.xp = GameManager.I.xp;
        currentProfile.currentStreak = GameManager.I.streakCount;

        // Update best streak if the current streak is higher.
        if (currentProfile.currentStreak > currentProfile.bestStreak)
        {
            currentProfile.bestStreak = currentProfile.currentStreak;
            OnBestStreakUpdated?.Invoke(currentProfile.bestStreak);
        }
        SaveProfile();
    }

    // Event handlers to sync specific data from GameManager
    private void SyncHeartsFromGameManager() => SyncFromGameManager();
    private void SyncXPFromGameManager() => SyncFromGameManager();
    private void SyncStreakFromGameManager() => SyncFromGameManager();

    // Auto-save on important application lifecycle events
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SyncFromGameManager();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SyncFromGameManager();
    }

    private void OnApplicationQuit()
    {
        SyncFromGameManager();
    }

    private void OnDestroy()
    {
        SyncFromGameManager();
    }
}
#endregion
