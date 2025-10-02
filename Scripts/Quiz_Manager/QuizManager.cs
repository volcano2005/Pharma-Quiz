using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;       // size = 4
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI heartsText;

    [Header("Progress System")]
    public Slider progressSlider;
    public Image[] starIcons; // 3 stars (set in inspector)
    public Sprite filledStar;
    public Sprite emptyStar;

    [Header("Explanation Popup")]
    public GameObject explanationPanel;
    public TextMeshProUGUI explanationText;
    public Button nextButton;

    [Header("Lesson Data")]
    private List<Jsonhandler.Question> questions = new List<Jsonhandler.Question>();
    private int currentQuestionIndex = 0;
    private Jsonhandler.Question pendingQuestion;
    private string pendingAltText;
    private bool waitingForRetry = false;

    private int starsEarned = 0;

    void Start()
    {
        if (GameManager.I == null)
        {
            Debug.LogError("GameManager not found. Make sure it exists in the first scene and uses DontDestroyOnLoad.");
            return;
        }

        Jsonhandler.LessonData lesson = LessonLoader.LoadLesson(GameManager.I.currentLessonId);
        if (lesson == null)
        {
            Debug.LogError("Failed to load lesson.");
            return;
        }

        questions = new List<Jsonhandler.Question>(lesson.questions);

        explanationPanel.SetActive(false);

        // Setup Progress
        progressSlider.maxValue = questions.Count;
        progressSlider.value = 0;

        ResetStars();

        UpdateHUD();
        ShowQuestion();
    }

    void UpdateHUD()
    {
        xpText.text = "XP: " + GameManager.I.xp;
        heartsText.text = "Hearts: " + GameManager.I.hearts;
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            OnLessonComplete();
            return;
        }

        Jsonhandler.Question q = questions[currentQuestionIndex];
        questionText.text = waitingForRetry && !string.IsNullOrEmpty(pendingAltText) ? pendingAltText : q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int choice = i;
            var label = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            var btnImage = answerButtons[i].GetComponent<Image>();

            if (i < q.options.Length)
            {
                label.text = q.options[i];
                answerButtons[i].interactable = true;
            }
            else
            {
                label.text = "";
                answerButtons[i].interactable = false;
            }

            btnImage.color = Color.white;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswer(choice));
        }

        waitingForRetry = false;
    }

    void OnAnswer(int chosenIndex)
    {
        foreach (var btn in answerButtons)
            btn.interactable = false;

        Jsonhandler.Question q = questions[currentQuestionIndex];

        if (chosenIndex == q.correctOptionIndex)
        {
            // ✅ Correct
            answerButtons[chosenIndex].GetComponent<Image>().color = Color.green;
            GameManager.I.AddXP(10);
            currentQuestionIndex++;

            // Update progress
            progressSlider.value = currentQuestionIndex;
            CheckStarMilestones();

            UpdateHUD();
            Invoke(nameof(ShowQuestion), 1f);
        }
        else
        {
            // ❌ Wrong
            answerButtons[chosenIndex].GetComponent<Image>().color = Color.red;
            answerButtons[q.correctOptionIndex].GetComponent<Image>().color = Color.green;

            GameManager.I.LoseHeart(1);
            UpdateHUD();

            if (GameManager.I.hearts > 0)
            {
                pendingQuestion = q;

                if (q.alternativeTexts != null && q.alternativeTexts.Length > 0)
                {
                    int rand = Random.Range(0, q.alternativeTexts.Length);
                    pendingAltText = q.alternativeTexts[rand];
                }
                else
                {
                    pendingAltText = q.questionText;
                }

                ShowExplanation(q.explanation, q);
            }
        }
    }

    void ShowExplanation(string explanation, Jsonhandler.Question q)
    {
        questionText.gameObject.SetActive(false);
        foreach (var btn in answerButtons) btn.gameObject.SetActive(false);

        explanationPanel.SetActive(true);
        explanationText.text = explanation;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            explanationPanel.SetActive(false);
            questionText.gameObject.SetActive(true);
            foreach (var btn in answerButtons) btn.gameObject.SetActive(true);

            waitingForRetry = true;
            ShowQuestion();
        });
    }

    void OnLessonComplete()
    {
        GameManager.I.UnlockNextLesson(GameManager.I.currentLessonId);
        GameManager.I.GoToLessonComplete();
    }

    // --------------------
    // ⭐ Progress + Stars
    // --------------------
    void ResetStars()
    {
        starsEarned = 0;
        foreach (var icon in starIcons)
        {
            icon.sprite = emptyStar;
        }
    }

    void CheckStarMilestones()
    {
        // Example: 7th, 9th, and 10th question
        if (currentQuestionIndex == 7) GiveStar(0);
        if (currentQuestionIndex == 9) GiveStar(1);
        if (currentQuestionIndex == questions.Count) GiveStar(2);
    }

    void GiveStar(int index)
    {
        if (index < starIcons.Length)
        {
            starIcons[index].sprite = filledStar;
            starsEarned++;
        }
    }
}
