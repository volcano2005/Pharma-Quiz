using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LessonComplete : MonoBehaviour
{
    public TextMeshProUGUI infoLabel;
    public Button retryButton;
    public Button homeButton;
    public int xpShown = 10; // optional, just for message

    void Start()
    {
        int lessonId = GameManager.I.currentLessonId;
        GameManager.I.UnlockNextLesson(lessonId);

        if (infoLabel)
            infoLabel.text = $"🎉 Well done! You completed Lesson {lessonId}\n+{xpShown} XP";

        if (retryButton) retryButton.onClick.AddListener(() => GameManager.I.StartLesson(lessonId));
        if (homeButton) homeButton.onClick.AddListener(() => GameManager.I.GoToMenu());
    }
}
