using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader1 : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMinigame(string minigameName)
    {
        int exerciseNumber = MinigameProgress.GetCurrentExercise(minigameName);
        string sceneName = $"{minigameName} - exercício {exerciseNumber}";
        SceneManager.LoadScene(sceneName);
    }
}
