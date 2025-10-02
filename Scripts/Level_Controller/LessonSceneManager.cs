using UnityEngine;
using TMPro;

public class LessonSceneManager : MonoBehaviour
{
    public TextMeshProUGUI lessonLabel;

    void Start()
    {
        if (lessonLabel == null)
        {
            Debug.LogError("LessonLabel not assigned!");
            return;
        }

        // Show current lesson number from GameManager
        lessonLabel.text = "Lesson " + GameManager.I.currentLessonId;
    }
}
