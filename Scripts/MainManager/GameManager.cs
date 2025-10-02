using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager I;
    public event Action OnXPUpdated;
    public event Action OnHeartsUpdated;

    #region Player State
    [Header("Player State")]
    public int xp;
    public int hearts = 10;
    public int currentLessonId = 1;
    public int highestLessonUnlocked = 1;
    #endregion

    #region Heart System
    [Header("Heart System")]
    public int maxHearts = 10;
    public float heartRegenTime = 300f; // seconds
    public double nextHeartTime;
    private bool isRegenerating;
    #endregion

    #region UI References
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI heartsText;
    [SerializeField] private TextMeshProUGUI heartTimerText;
    [SerializeField] private GameObject hudCanvas;
    #endregion

    #region Streak System
    [Header("Streak System")]
    public int streakCount = 0;
    [SerializeField] private TextMeshProUGUI streakText;
    public int bestStreak = 0;

    private const string LAST_PLAY_KEY = "LAST_PLAY_DATE";
    private const string STREAK_KEY = "STREAK_COUNT";
    #endregion

    #region Daily Quiz System
    [Header("Daily Quiz System")]
    public bool isDailyQuizAvailable = true;
    public int dailyQuizXPReward = 50;

    private const string LAST_DAILY_KEY = "LAST_DAILY_DATE";
    private const string QUIZ_STREAK_KEY = "QUIZ_STREAK";
    #endregion

    #region Constants
    private const string XP_KEY = "XP";
    private const string HEARTS_KEY = "HEARTS";
    private const string HIGHEST_KEY = "HIGHEST_LESSON";
    private const string NEXT_HEART_KEY = "NEXT_HEART_TIME";
    #endregion

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        LoadState();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        CheckDailyStreak();
        CheckDailyQuiz();
        UpdateStreakUI();
    }

    void Update()
    {
        HandleHeartRegen();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (hudCanvas == null) return;

        bool isMainMenu = scene.name == "MainMenu" || scene.name == "LevelSelect" || scene.name == "Progress";
        hudCanvas.SetActive(isMainMenu);

        if (isMainMenu)
        {
            UpdateHeartsUI();
            UpdateStreakUI();
        }
    }

    #region Heart Logic
    private void HandleHeartRegen()
    {
        if (hearts >= maxHearts)
        {
            isRegenerating = false;
            if (heartTimerText != null) heartTimerText.text = "";
            return;
        }

        UpdateHeartTimer();

        if (nextHeartTime > 0 && GetCurrentUnixTime() >= nextHeartTime)
        {
            RegenerateHearts();
        }
    }

    private void StartHeartRegeneration()
    {
        if (hearts >= maxHearts) return;

        double currentTime = GetCurrentUnixTime();
        nextHeartTime = currentTime + heartRegenTime;
        isRegenerating = true;
        SaveState();
    }

    private void UpdateHeartTimer()
    {
        if (hearts >= maxHearts)
        {
            if (heartTimerText != null) heartTimerText.text = "";
            return;
        }

        double currentTime = GetCurrentUnixTime();
        double timeLeft = nextHeartTime - currentTime;

        if (timeLeft <= 0)
        {
            RegenerateHearts();
        }
        else
        {
            if (heartTimerText != null)
            {
                TimeSpan t = TimeSpan.FromSeconds(timeLeft);
                heartTimerText.text = $"{t.Minutes:D2}:{t.Seconds:D2}";
            }
        }
    }

    private void RegenerateHearts()
    {
        hearts = Mathf.Min(maxHearts, hearts + 1);

        if (hearts < maxHearts)
        {
            double currentTime = GetCurrentUnixTime();
            nextHeartTime = currentTime + heartRegenTime;
            isRegenerating = true;
        }
        else
        {
            nextHeartTime = 0;
            isRegenerating = false;
        }

        SaveState();
        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        if (heartsText != null)
        {
            heartsText.text = $"{hearts}/{maxHearts}";
        }
        OnHeartsUpdated?.Invoke();
    }

    public void LoseHeart(int amount = 1)
    {
        hearts = Mathf.Max(0, hearts - amount);

        if (hearts < maxHearts && !isRegenerating)
        {
            StartHeartRegeneration();
        }

        SaveState();
        UpdateHeartsUI();

        if (hearts <= 0)
        {
            OutOfHeartsPopup popup = FindObjectOfType<OutOfHeartsPopup>();
            if (popup != null) popup.ShowPopup();
        }
    }

    public void AddHearts(int amount)
    {
        hearts = Mathf.Min(maxHearts, hearts + amount);
        isRegenerating = hearts < maxHearts;

        SaveState();
        UpdateHeartsUI();
    }

    public void RefillHearts(int to = -1)
    {
        hearts = Mathf.Min(maxHearts, to == -1 ? maxHearts : to);
        isRegenerating = hearts < maxHearts;

        SaveState();
        UpdateHeartsUI();
    }
    #endregion

    #region Streak Logic
    public void CheckDailyStreak()
    {
        string lastPlay = PlayerPrefs.GetString(LAST_PLAY_KEY, "");
        string today = DateTime.UtcNow.ToString("yyyyMMdd");

        if (string.IsNullOrEmpty(lastPlay))
        {
            streakCount = 1;
        }
        else if (lastPlay != today)
        {
            DateTime lastDate = DateTime.ParseExact(lastPlay, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.UtcNow;

            if ((currentDate - lastDate).Days == 1)
            {
                streakCount++;
            }
            else
            {
                streakCount = 1;
            }
        }

        int bestStreakSaved = PlayerPrefs.GetInt("BEST_STREAK", 0);
        if (streakCount > bestStreakSaved)
        {
            bestStreak = streakCount;
            PlayerPrefs.SetInt("BEST_STREAK", bestStreak);
        }

        PlayerPrefs.SetString(LAST_PLAY_KEY, today);
        PlayerPrefs.SetInt(STREAK_KEY, streakCount);
        PlayerPrefs.Save();
    }

    public void UpdateStreakUI()
    {
        if (streakText != null)
        {
            streakText.text = streakCount.ToString();
        }
    }
    #endregion

    #region Daily Quiz Logic
    public void CheckDailyQuiz()
    {
        string lastDaily = PlayerPrefs.GetString(LAST_DAILY_KEY, "");
        string today = DateTime.UtcNow.ToString("yyyyMMdd");
        isDailyQuizAvailable = (lastDaily != today);
    }

    public void CompleteDailyQuiz()
    {
        string today = DateTime.UtcNow.ToString("yyyyMMdd");
        string lastQuizDay = PlayerPrefs.GetString(LAST_DAILY_KEY, "");

        if (lastQuizDay == today)
        {
            Debug.Log("Quiz already completed today!");
            return;
        }

        PlayerPrefs.SetString(LAST_DAILY_KEY, today);
        isDailyQuizAvailable = false;

        int quizStreak = PlayerPrefs.GetInt(QUIZ_STREAK_KEY, 0);
        if (string.IsNullOrEmpty(lastQuizDay))
        {
            quizStreak = 1;
        }
        else
        {
            DateTime lastDate = DateTime.ParseExact(lastQuizDay, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.UtcNow;

            if ((currentDate - lastDate).Days == 1)
            {
                quizStreak++;
            }
            else
            {
                quizStreak = 1;
            }
        }

        PlayerPrefs.SetInt(QUIZ_STREAK_KEY, quizStreak);
        PlayerPrefs.Save();

        AddXP(dailyQuizXPReward);
    }

    public void UpdateQuizStreakUI(TextMeshProUGUI quizStreakText)
    {
        int quizStreak = PlayerPrefs.GetInt(QUIZ_STREAK_KEY, 0);
        if (quizStreakText != null)
        {
            quizStreakText.text = quizStreak.ToString();
        }
    }
    #endregion

    #region XP Logic
    public void AddXP(int amount)
    {     
        if (StoreManager.IsXPBoostActive())
        {
            amount *= 2; // Double XP
            Debug.Log("⚡ XP Boost applied! Gained extra XP.");
        }

        xp += amount;

        SaveState();
        OnXPUpdated?.Invoke();
    }
    #endregion

    #region Lesson Logic
    public void UnlockNextLesson(int lessonId)
    {
        if (lessonId + 1 > highestLessonUnlocked)
        {
            highestLessonUnlocked = lessonId + 1;
            SaveState();
        }
    }

    public void StartLesson(int lessonId)
    {
        currentLessonId = lessonId;
        SceneManager.LoadScene("Quiz");
    }
    #endregion

    #region Scene Navigation
    public void GoToMenu() => SceneManager.LoadScene("MainMenu");
    public void GoToLevelSelect() => SceneManager.LoadScene("LevelSelect");
    public void GoToProgress() => SceneManager.LoadScene("Progress");
    public void GoToLessonComplete() => SceneManager.LoadScene("LessonComplete");
    public void GoToOutOfHearts() => SceneManager.LoadScene("OutOfHearts");
    #endregion

    #region Time Utility
    public double GetTimeLeftForNextHeart()
    {
        if (hearts >= maxHearts) return 0;
        return Math.Max(0, nextHeartTime - GetCurrentUnixTime());
    }

    public string GetFormattedTimeLeft()
    {
        double timeLeft = GetTimeLeftForNextHeart();
        if (timeLeft <= 0) return "00:00";

        TimeSpan t = TimeSpan.FromSeconds(timeLeft);
        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }
    public double GetCurrentUnixTime()
    {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }
    #endregion

    #region Save/Load Logic
    void SaveState()
    {
        PlayerPrefs.SetInt(XP_KEY, xp);
        PlayerPrefs.SetInt(HEARTS_KEY, hearts);
        PlayerPrefs.SetInt(HIGHEST_KEY, highestLessonUnlocked);
        PlayerPrefs.SetString(NEXT_HEART_KEY, nextHeartTime.ToString());
        PlayerPrefs.Save();
    }

    void LoadState()
    {
        xp = PlayerPrefs.GetInt(XP_KEY, 0);
        hearts = PlayerPrefs.GetInt(HEARTS_KEY, maxHearts);
        highestLessonUnlocked = PlayerPrefs.GetInt(HIGHEST_KEY, 1);
        streakCount = PlayerPrefs.GetInt(STREAK_KEY, 0);

        LoadHeartState();
        UpdateHeartsUI();
        UpdateStreakUI();
    }

    private void LoadHeartState()
    {
        double currentTime = GetCurrentUnixTime();
        double savedNextHeart = 0;

        if (PlayerPrefs.HasKey(NEXT_HEART_KEY))
        {
            double.TryParse(PlayerPrefs.GetString(NEXT_HEART_KEY), out savedNextHeart);
        }

        if (hearts < maxHearts)
        {
            if (savedNextHeart > 0)
            {
                if (savedNextHeart <= currentTime)
                {
                    double timePassed = currentTime - savedNextHeart;
                    int heartsToAdd = 1 + (int)(timePassed / heartRegenTime);

                    hearts = Mathf.Min(maxHearts, hearts + heartsToAdd);

                    if (hearts < maxHearts)
                    {
                        double remainder = timePassed % heartRegenTime;
                        nextHeartTime = currentTime + (heartRegenTime - remainder);
                        isRegenerating = true;
                    }
                    else
                    {
                        nextHeartTime = 0;
                        isRegenerating = false;
                    }
                }
                else
                {
                    nextHeartTime = savedNextHeart;
                    isRegenerating = true;
                }
            }
            else
            {
                StartHeartRegeneration();
            }
        }
        else
        {
            nextHeartTime = 0;
            isRegenerating = false;
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) SaveState();
    }

    void OnApplicationQuit()
    {
        SaveState();
    }
    #endregion
}
