using UnityEngine;
using UnityEngine.UI;   // for Button
using TMPro;           // for TextMeshPro

public class LessonButton : MonoBehaviour
{
    public int lessonId;              // Lesson number (1, 2, 3…)
    public TextMeshProUGUI label;     // Text on the button
    public Button button;             // The button itself

    void Start()
    {
        if (label == null || button == null)
        {
            Debug.LogError("LessonButton is missing references!");
            return;
        }

        bool isUnlocked = (lessonId <= GameManager.I.highestLessonUnlocked);
        Debug.Log($"Lesson {lessonId} unlocked? {isUnlocked}");

        if (isUnlocked)
        {
            button.interactable = true;
            label.text = "Lesson " + lessonId;

            button.onClick.AddListener(OnLessonClicked);
        }
        else
        {
            button.interactable = false;
            label.text = "Lesson " + lessonId + " (Locked)";
        }
    }

    void OnLessonClicked()
    {
        if (GameManager.I.hearts > 0)
        {
            //  Player has hearts → Start lesson
            GameManager.I.StartLesson(lessonId);
        }
        else
        {
            // ❌ No hearts → Show popup
            NoHeartPopup.Show("You don’t have hearts!\nPlease wait until they regenerate.");
        }
    }
}
