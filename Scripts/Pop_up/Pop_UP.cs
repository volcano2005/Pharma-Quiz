using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pop_UP : MonoBehaviour
{
    public TextMeshProUGUI DisplayPOP_UP;
    private IEnumerator popup()
    {
        DisplayPOP_UP.enabled = true;
        yield return new WaitForSeconds(0.5f);
        DisplayPOP_UP.enabled = false;
    }

    public void openPop_up()
    {
        StartCoroutine(popup());
        DisplayPOP_UP.text = "Streaks";       
    }

   
   
}
