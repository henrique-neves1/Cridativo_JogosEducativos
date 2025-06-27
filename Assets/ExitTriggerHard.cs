using UnityEngine;

public class ExitTriggerHard : MonoBehaviour
{
    public MazeEndControllerHard mazeEndController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            mazeEndController.TriggerEndSequence();
            Destroy(gameObject); // prevent retriggering
        }
        if (mazeEndController == null)
        {
            Debug.LogError("MazeEndController is null on ExitTrigger!");
            return;
        }
    }
}
