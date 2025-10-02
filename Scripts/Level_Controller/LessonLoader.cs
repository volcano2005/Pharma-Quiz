using System.Collections.Generic;
using UnityEngine;

// Add a using statement to access the classes from Jsonhandler
// This assumes your Jsonhandler script is in the same namespace or accessible.
// If not, you may need to specify the full namespace.
// For example: using YourProject.Scripts.Data;
// If the classes are nested inside Jsonhandler, you don't need a using statement.

public static class LessonLoader
{
    // The LoadLesson method must return an object of the correct type,
    // which is the LessonData class defined inside Jsonhandler.
    public static Jsonhandler.LessonData LoadLesson(int lessonId)
    {
        string path = $"Lessons/Lesson{lessonId}";
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogError($"[LessonLoader] Missing JSON at Resources/{path}.json");
            return null;
        }

        Jsonhandler.LessonData data = null;
        try
        {
            // Deserialize the JSON into the correct type: Jsonhandler.LessonData
            data = JsonUtility.FromJson<Jsonhandler.LessonData>(json.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LessonLoader] Parse error: {e.Message}");
            return null;
        }

        if (data == null || data.questions == null || data.questions.Length == 0)
        {
            Debug.LogError($"[LessonLoader] Invalid or empty data in {path}.json");
            return null;
        }

        return data;
    }
}