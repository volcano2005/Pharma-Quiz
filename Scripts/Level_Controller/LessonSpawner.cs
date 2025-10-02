using UnityEngine;

public class LessonSpawner : MonoBehaviour
{
    public GameObject lessonButtonPrefab;   // Prefab of button
    public Transform lessonContainer;       // ScrollView Content
    public int totalLessons = 5;            // How many lessons exist

    void Start()
    {
        SpawnLessons();
    }

    void SpawnLessons()
    {
        // Clear old children (if any)
        foreach (Transform child in lessonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= totalLessons; i++)
        {
            GameObject newButton = Instantiate(lessonButtonPrefab, lessonContainer);

            LessonButton lb = newButton.GetComponent<LessonButton>();
            lb.lessonId = i; // Each button gets its lesson number
        }
    }
}
