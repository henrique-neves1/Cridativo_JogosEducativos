using UnityEngine;
using UnityEngine.EventSystems;

public class ExitTriggerUI : MonoBehaviour, IPointerEnterHandler
{
    public MazeEndController mazeEndController;

    private bool triggered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (triggered) return;
        if (eventData.pointerEnter.CompareTag("Player")) // This likely won't work as expected
        {
            triggered = true;
            mazeEndController.TriggerEndSequence();
        }
    }
}