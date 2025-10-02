using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnStartJourney()
    {
        if (GameManager.I != null)
            GameManager.I.GoToLevelSelect();
    }

    public void OnProgress()
    {
        if (GameManager.I != null)
            GameManager.I.GoToProgress();
    }

    public void About_Scene()
    {
       SceneManager.LoadScene("About");
    }
    public void BackTOMainMenu()
    {
       SceneManager.LoadScene("MainMenu");
    }
    public void OnSettings()
    {
        // TODO: open settings panel or scene if you add one
        Debug.Log("Settings button clicked!");
    }

    public void OnQuit()
    {
        Application.Quit();
        Debug.Log("Quit Game (won't close in Editor)");
    }
    public void EditProfile()
    {
        SceneManager.LoadScene("Profile");
    }
    public void Store_Scene()
    {
        SceneManager.LoadScene("Store");
    }
    public void Daily_Quiz_Scene()
    {
        SceneManager.LoadScene("Daiy_Quiz");
    }
   
}
