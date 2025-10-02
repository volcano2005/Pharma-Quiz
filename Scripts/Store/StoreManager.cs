using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class StoreManager : MonoBehaviour
{
    public static StoreManager I;

    [Header("UI References")]
    public TextMeshProUGUI currentXPText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI xpBoostTimerText; // <- Add in Inspector
    public Button buyHeartButton;
    public Button buyXPBoostButton;
    public GameObject xptext;

    private void Awake()
    {
        if (I == null) I = this;
    }

    private void Start()
    {
        UpdateUI();

        // Subscribe to events
        GameManager.I.OnXPUpdated += UpdateUI;
        GameManager.I.OnHeartsUpdated += UpdateUI;
    }

    private void Update()
    {
        // Update XP Boost timer every frame
        if (IsXPBoostActive())
        {
            DateTime expiry = DateTime.Parse(PlayerPrefs.GetString("XPBoostExpiry", ""));
            TimeSpan remain = expiry - DateTime.Now;

            if (remain.TotalSeconds > 0)
            {
                xptext.SetActive(false);
                xpBoostTimerText.text = $"Boost Active: {remain.Hours:D2}:{remain.Minutes:D2}:{remain.Seconds:D2}";
            }
            else
            {
                // Expired → Reset
                xptext.SetActive(true);
                PlayerPrefs.SetInt("XPBoostActive", 0);
               // xpBoostTimerText.text = " Boost expired!";
                UpdateUI();
            }
        }
        //else
        //{
        //    //if (xpBoostTimerText != null)
        //    //    xpBoostTimerText.text = " No Active Boost";
        //}
    }

    private void OnDestroy()
    {
        if (GameManager.I != null)
        {
            GameManager.I.OnXPUpdated -= UpdateUI;
            GameManager.I.OnHeartsUpdated -= UpdateUI;
        }
    }

    void UpdateUI()
    {
        if (currentXPText != null)
            currentXPText.text = "XP: " + GameManager.I.xp;

        // Disable Buy Heart if already max hearts
        if (buyHeartButton != null)
            buyHeartButton.interactable = GameManager.I.hearts < GameManager.I.maxHearts;

        // Disable Buy XP Boost if already active
        if (buyXPBoostButton != null)
            buyXPBoostButton.interactable = !IsXPBoostActive();
    }

    public void BuyHeart()
    {
        if (GameManager.I.hearts >= GameManager.I.maxHearts)
        {
            ShowMessage(" You already have max hearts!");
            return;
        }

        if (GameManager.I.xp >= 100)
        {
            GameManager.I.AddXP(-100);
            GameManager.I.AddHearts(1);
            ShowMessage(" Bought 1 Heart!");
        }
        else
        {
            ShowMessage(" Not enough XP!");
        }

        UpdateUI();
    }

    public void BuyXPBoost()
    {
        if (IsXPBoostActive())
        {
            ShowMessage(" XP Boost is already active!");
            return;
        }

        if (GameManager.I.xp >= 1000)
        {
            GameManager.I.AddXP(-1000);

            DateTime expiry = DateTime.Now.AddHours(24);
            PlayerPrefs.SetInt("XPBoostActive", 1);
            PlayerPrefs.SetString("XPBoostExpiry", expiry.ToString());
            PlayerPrefs.Save();

            ShowMessage(" XP Boost Activated for 24h!");
        }
        else
        {
            ShowMessage(" Not enough XP!");
        }

        UpdateUI();
    }

    public static bool IsXPBoostActive()
    {
        if (PlayerPrefs.GetInt("XPBoostActive", 0) == 1)
        {
            string expiryStr = PlayerPrefs.GetString("XPBoostExpiry", "");
            if (!string.IsNullOrEmpty(expiryStr))
            {
                DateTime expiry = DateTime.Parse(expiryStr);
                return DateTime.Now < expiry;
            }
        }
        return false;
    }

    void ShowMessage(string msg)
    {
        if (messageText != null)
            messageText.text = msg;
        Debug.Log(msg);
    }

    public void Back_TO_Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
