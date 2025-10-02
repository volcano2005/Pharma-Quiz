using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jsonhandler : MonoBehaviour
{
    // Make the Question class public so other scripts can access it.
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctOptionIndex;
        public string explanation;
        public string[] alternativeTexts; // This property is now correctly defined and public.
    }

    [System.Serializable]
    public class LessonData
    {
        public string lessonName;
        public Question[] questions;
    }
}