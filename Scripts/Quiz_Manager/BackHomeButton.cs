using UnityEngine;
using UnityEngine.SceneManagement;

public class BackHomeButton : MonoBehaviour
{
    public void BackToHome()
    {
        // Always go to MainMenu scene safely
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}