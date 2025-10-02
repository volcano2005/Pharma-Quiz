using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NoHeartPopup : MonoBehaviour
{
    public static NoHeartPopup I;

    [Header("UI References")]
    public GameObject popupPanel;      // Assign a Panel in Canvas
    public TextMeshProUGUI messageText; // Popup message
    public Button closeButton;         // Close button

    private void Awake()
    {
        if (I == null) I = this;

        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    public static void Show(string message)
    {
        if (I != null && I.popupPanel != null)
        {
            I.popupPanel.SetActive(true);
            I.messageText.text = message;
        }
    }

    public void Hide()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}
