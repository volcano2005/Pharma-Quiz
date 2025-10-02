using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;  // Singleton instance

    [Header("Audio Setup")]
    public AudioSource sfx;
    public AudioClip click;
    public AudioClip correct;
    public AudioClip wrong;
    public AudioClip celebration;

    void Awake()
    {
        // Singleton setup
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Play Methods ---
    public void PlayClick() { if (click) sfx.PlayOneShot(click); }
    public void PlayCorrect() { if (correct) sfx.PlayOneShot(correct); }
    public void PlayWrong() { if (wrong) sfx.PlayOneShot(wrong); }
    public void PlayCelebration() { if (celebration) sfx.PlayOneShot(celebration); }

}
