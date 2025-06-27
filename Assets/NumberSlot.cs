using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;
using UnityEngine.UI;
using System.Collections;

public class NumberSlot : MonoBehaviour, IDropHandler
{
    public string correctTextNumber; // e.g., "sete"
    public GameObject checkmark;
    public GameObject textReveal;
    public GameObject wrongX;
    public AudioSource sfxPlayer;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var draggable = dragged.GetComponent<DraggableText>();
        if (draggable == null) return;

        if (draggable.numberValue == correctTextNumber)
        {
            draggable.wasDroppedOnSlot = true; // ✅ Mark as handled
            Destroy(dragged.gameObject);
            textReveal.SetActive(true);
            FindObjectOfType<GameManager>().SaveProgress(correctTextNumber);

            GameManager gm = FindObjectOfType<GameManager>();
            gm.ShowRandomFeedback(true);
            StartCoroutine(DelayedCheckCompletion(gm));

            StartCoroutine(HandleCorrect());
        }
        else
        {
            FindObjectOfType<GameManager>().ShowRandomFeedback(false);

            draggable.wasDroppedOnSlot = false; // Explicit but optional
            StartCoroutine(FlashWrongX());
            sfxPlayer.PlayOneShot(wrongSound);
            draggable.transform.SetParent(draggable.originalParent, false); // Reinsert to layout
            draggable.transform.SetAsLastSibling();
        }
    }

    private IEnumerator DelayedCheckCompletion(GameManager gm)
    {
        // Wait a few frames to ensure the object is destroyed and reveal is active
        yield return null;
        yield return null;
        gm.CheckForCompletion();
    }

    private IEnumerator HandleCorrect()
    {
        checkmark.SetActive(true);
        checkmark.transform.localScale = Vector3.zero;

        sfxPlayer.PlayOneShot(correctSound);

        Tween.Scale(checkmark.transform, Vector3.one, 0.7f, Ease.OutBack);
        yield return new WaitForSeconds(2f);
        checkmark.SetActive(false);
    }

    private IEnumerator FlashWrongX()
    {
        wrongX.SetActive(true);
        for (int i = 0; i < 2; i++)
        {
            wrongX.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            wrongX.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
        wrongX.SetActive(true); // Final blink stays visible slightly longer
        yield return new WaitForSeconds(1f);
        wrongX.SetActive(false); // Hide it again
    }
}
