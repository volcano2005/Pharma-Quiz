using System;

[Serializable]
public class Question
{
    public string questionText;
    public string[] options;          // length should match your number of buttons (e.g., 4)
    public int correctOptionIndex;    // 0-based index into options[]
}

[Serializable]
public class LessonData
{
    public string lessonName;         // e.g., "Lesson 1: Basics"
    public Question[] questions;      // at least 1
}
