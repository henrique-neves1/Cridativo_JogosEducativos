using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressReporter : MonoBehaviour
{
    public string minigameName;

    private QuizGame quizGame;
    private bool hasReported = false;

    void Awake()
    {
        quizGame = GetComponent<QuizGame>();
    }

    void Update()
    {
        // Check if the next scene is about to load AND it's not Parabéns
        // and this scene hasn't reported progress yet
        if (quizGame != null && !hasReported)
        {
            if (quizGame.allowSkip)
            {
                if (quizGame.nextSceneName == "Parabéns")
                {
                    MinigameProgress.ResetExercise(minigameName);
                }
                else
                {
                    MinigameProgress.AdvanceExercise(minigameName);
                }
                hasReported = true;
            }
        }
    }

    // Helper to guess that the next scene is about to load
    private bool quizGameIsAboutToLoad()
    {
        // QuizGame sets allowSkip = true when correct answer is hit
        // and triggers scene loading soon after
        return quizGame.allowSkip;
    }

    string DetermineAndReportNextScene(string nextScene)
    {
        // If we're finishing exercise 10 and going to "Parabéns", reset
        if (nextScene == "Parabéns")
        {
            MinigameProgress.ResetExercise(minigameName);
        }
        else
        {
            // Otherwise, we're progressing to the next exercise
            MinigameProgress.AdvanceExercise(minigameName);
        }

        return nextScene; // Return unchanged
    }
}
