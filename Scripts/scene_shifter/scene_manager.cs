using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene_manager : MonoBehaviour
{
   public void backtoMainMenu()
   {
        SceneManager.LoadScene("MainMenu");
   }
    
}
