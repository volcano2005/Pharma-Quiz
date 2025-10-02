using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutOfHeartsPopup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupMessage;
    public TextMeshProUGUI timerText;
    public Button watchAdButton;
    public Button waitButton;
    public Button backButton;
  //  public TextMeshProUGUI streakText;

    private void Awake()
    {
        popupPanel.SetActive(false); // hide at start
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        popupMessage.text = "You’re out of hearts!";

        // setup button listeners
        watchAdButton.onClick.RemoveAllListeners();
        watchAdButton.onClick.AddListener(OnWatchAd);

        waitButton.onClick.RemoveAllListeners();
        waitButton.onClick.AddListener(OnWait);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBack);

        // always show live countdown
        StopAllCoroutines();
        StartCoroutine(UpdateTimerCoroutine());
    }

    void OnWatchAd()
    {
        // TODO: Replace with Unity Ads reward call
        GameManager.I.RefillHearts(GameManager.I.maxHearts);
        popupPanel.SetActive(false);
    }

    void OnWait()
    {
        // just continue watching timer until hearts regenerate
        StopAllCoroutines();
        StartCoroutine(UpdateTimerCoroutine());
    }

    void OnBack()
    {
        popupPanel.SetActive(false);
        GameManager.I.GoToMenu();
    }

    IEnumerator UpdateTimerCoroutine()
    {
        while (GameManager.I.hearts < GameManager.I.maxHearts)
        {
            timerText.text = $"Next heart in: {GameManager.I.GetFormattedTimeLeft()}";
            yield return new WaitForSeconds(1);
        }

        // hearts are full → close popup automatically
        popupPanel.SetActive(false);
    }
}
