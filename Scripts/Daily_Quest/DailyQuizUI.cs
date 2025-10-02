using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DailyQuizUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI timerText;
    public Button playButton;
    public GameObject completedBadge;
    public GameObject Back_Ground_Off;

    [Header("Question Panel")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;


    private QuestionData currentQuestion;

    private void Start()
    {
        titleText.text = " Daily Quiz ";
        questionPanel.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (DailyQuizManager.I != null && DailyQuizManager.I.isCompletedToday)
        {
            var tomorrow = System.DateTime.Now.Date.AddDays(1);
            var remain = tomorrow - System.DateTime.Now;
            timerText.text = $"Next quiz in: {remain.Hours:D2}:{remain.Minutes:D2}:{remain.Seconds:D2}";
        }
    }

    void UpdateUI()
    {
        streakText.text = "Streak: " + DailyQuizManager.I.dailyStreak;
        rewardText.text = "Reward: +20 XP";

        if (DailyQuizManager.I.isCompletedToday)
        {
            playButton.interactable = false;
            completedBadge.SetActive(true);
            timerText.text = "Next quiz unlocks tomorrow!";
        }
        else
        {
            playButton.interactable = true;
            completedBadge.SetActive(false);
            timerText.text = "Available Now!";
        }
    }

    public void OnPlayDailyQuiz()
    {
        playButton.gameObject.SetActive(false);
        questionPanel.SetActive(true);
        Back_Ground_Off.SetActive(false);
        LoadNextQuestion();
    }

    void LoadNextQuestion()
    {
        currentQuestion = DailyQuizManager.I.GetNextQuestion();

        if (currentQuestion == null)
        {
            DailyQuizManager.I.CompleteDailyQuiz();
            questionText.text = " Quiz Completed!";
            StartCoroutine(Autosceneload());
            foreach (var btn in optionButtons) btn.gameObject.SetActive(false);
            return;
        }

        questionText.text = currentQuestion.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            // make sure buttons are visible + interactable again
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].interactable = true;

            TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = currentQuestion.options[i];

            // reset button color
            optionButtons[i].GetComponent<Image>().color = normalColor;

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    void OnOptionSelected(int index)
    {
        // disable buttons after click
        foreach (var btn in optionButtons)
            btn.interactable = false;

        if (index == currentQuestion.correctIndex)
        {
            questionText.text = " Correct!";
            optionButtons[index].GetComponent<Image>().color = correctColor;
        }
        else
        {
            questionText.text = " Wrong!";
            optionButtons[index].GetComponent<Image>().color = wrongColor;

            // highlight the correct answer
            optionButtons[currentQuestion.correctIndex].GetComponent<Image>().color = correctColor;
        }

        // next question after delay
        Invoke(nameof(LoadNextQuestion), 1f);
    }

    IEnumerator Autosceneload()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenu");
    }
}
