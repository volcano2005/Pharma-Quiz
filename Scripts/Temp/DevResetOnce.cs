using UnityEngine;
using UnityEngine.UI;

public class DevResetOnce : MonoBehaviour
{
    [Header("Buttons")]
    public Button resetLessonsButton;
    public Button refillHeartsButton;
    public Button addXPButton;

    [Header("Settings")]
    [Range(1, 100)]
    public int unlockUpTo = 1;   // How many lessons to unlock
    public int refillTo = 3;     // How many hearts to refill
    public int xpToAdd = 50;     // XP to add each click

    void Start()
    {
        if (resetLessonsButton != null)
            resetLessonsButton.onClick.AddListener(ResetLessons);

        if (refillHeartsButton != null)
            refillHeartsButton.onClick.AddListener(RefillHearts);

        if (addXPButton != null)
            addXPButton.onClick.AddListener(AddXP);
    }

    void ResetLessons()
    {
        GameManager.I.highestLessonUnlocked = unlockUpTo;
        PlayerPrefs.SetInt("HIGHEST_LESSON", unlockUpTo);
        PlayerPrefs.Save();
        Debug.Log("Lessons reset → Only up to Lesson " + unlockUpTo + " unlocked.");
    }

    void RefillHearts()
    {
        GameManager.I.RefillHearts(refillTo);
        Debug.Log("Hearts refilled to " + refillTo);
    }

    void AddXP()
    {
        GameManager.I.AddXP(xpToAdd);
        Debug.Log("Added " + xpToAdd + " XP. Total XP = " + GameManager.I.xp);
    }
}
