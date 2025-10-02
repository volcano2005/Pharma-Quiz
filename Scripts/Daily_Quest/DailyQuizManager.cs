using UnityEngine;
using System;

[System.Serializable]
public class QuestionData
{
    public string question;
    public string[] options;
    public int correctIndex;
}

[System.Serializable]
public class QuestionList
{
    public QuestionData[] questions;
}

public class DailyQuizManager : MonoBehaviour
{
    public static DailyQuizManager I;

    public QuestionList quizData;
    private int currentQuestionIndex;

    private string lastDailyKey = "lastDailyQuizDate";
    private string streakKey = "dailyStreak"; // Used in DailyQuizManager and for UI
    private string completedKey = "dailyCompleted";

    public int dailyStreak { get; private set; }
    public bool isCompletedToday { get; private set; }

    private void Awake()
    {
        if (I == null) I = this;
    }

    void Start()
    {
        CheckDailyQuizStatus();
        LoadQuestionsFromJSON();
    }

    void LoadQuestionsFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Questions/daily_quiz");
        if (jsonFile != null)
        {
            quizData = JsonUtility.FromJson<QuestionList>(jsonFile.text);
            currentQuestionIndex = 0;
            Debug.Log("✅ Daily quiz questions loaded!");
        }
        else
        {
            Debug.LogError("❌ daily_quiz.json not found in Resources/Questions/");
        }
    }

    public QuestionData GetNextQuestion()
    {
        if (quizData != null && currentQuestionIndex < quizData.questions.Length)
        {
            return quizData.questions[currentQuestionIndex++];
        }
        else
        {
            return null;
        }
    }

    void CheckDailyQuizStatus()
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        string lastPlayed = PlayerPrefs.GetString(lastDailyKey, "");
        if (lastPlayed != today)
        {
            isCompletedToday = false;
            PlayerPrefs.SetInt(completedKey, 0);

            DateTime yesterday = DateTime.Now.AddDays(-1);
            if (lastPlayed == yesterday.ToString("yyyyMMdd"))
            {
                // Streak continues
                dailyStreak = PlayerPrefs.GetInt(streakKey, 0) + 1;
            }
            else
            {
                // Missed day, reset streak
                dailyStreak = 1;
            }
            PlayerPrefs.SetInt(streakKey, dailyStreak);
            PlayerPrefs.SetString(lastDailyKey, today);
        }
        else
        {
            isCompletedToday = PlayerPrefs.GetInt(completedKey, 0) == 1;
            dailyStreak = PlayerPrefs.GetInt(streakKey, 0);
        }
    }


    public void CompleteDailyQuiz()
    {
        if (!isCompletedToday)
        {
            isCompletedToday = true;
            PlayerPrefs.SetInt(completedKey, 1);

            // ✅ Update streak immediately so UI can refresh
            dailyStreak = PlayerPrefs.GetInt(streakKey, 0);
            PlayerPrefs.Save();

            GiveDailyRewards();
        }
    }

    void GiveDailyRewards()
    {
        GameManager.I.AddXP(20); // base reward

        if (dailyStreak % 3 == 0) GameManager.I.AddHearts(1);
        if (dailyStreak % 7 == 0) GameManager.I.AddXP(100);

        Debug.Log("✅ Daily Quiz Completed! Streak: " + dailyStreak);
    }
}
