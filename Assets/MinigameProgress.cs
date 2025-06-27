using System.Collections.Generic;

public static class MinigameProgress
{
    private static Dictionary<string, int> progress = new Dictionary<string, int>()
    {
        { "Estudo da leitura", 1 },
        { "Identifica��o de sons", 1 },
        { "Associa��o de quantidades", 1 },
        { "Identifica��o de emo��es", 1 }
    };

    public static int GetCurrentExercise(string minigameName)
    {
        return progress[minigameName];
    }

    public static void SetCurrentExercise(string minigameName, int exerciseNumber)
    {
        progress[minigameName] = exerciseNumber;
    }

    public static void AdvanceExercise(string minigameName)
    {
        if (progress[minigameName] < 10)
        {
            progress[minigameName]++;
        }
        else
        {
            progress[minigameName] = 1; // Reset after exercise 10
        }
    }

    public static void ResetExercise(string minigameName)
    {
        progress[minigameName] = 1;
    }

    /*public static void ReportSceneChange(string sceneName)
    {
        // Match e.g., "Estudo da leitura - exerc�cio 3"
        var match = Regex.Match(sceneName, @"^(.*?) - exerc�cio (\d+)$");
        if (match.Success)
        {
            string minigame = match.Groups[1].Value;
            int exercise = int.Parse(match.Groups[2].Value);
            //progress[minigame] = exercise;
        }
        else if (sceneName == "Parab�ns")
        {
            // Reset all minigames to 1 when congratulation scene loads
            foreach (var key in progress.Keys)
            {
                progress[key] = 1;
            }
        }
    }*/
}