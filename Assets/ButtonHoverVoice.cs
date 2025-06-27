using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverVoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public AudioClip hoverVoice;
    public AudioSource audioSource;

    private bool isPressed = false;
    private bool isHovered = false;
    private bool soundHasPlayedDuringHover = false;

    /*private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }*/

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (!isPressed && !audioSource.isPlaying)
        {
            PlayHoverSound();
        }

        if (isPressed)
        {
            soundHasPlayedDuringHover = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (!isPressed && audioSource.isPlaying)
        {
            audioSource.Stop();
            soundHasPlayedDuringHover = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        if (isHovered)
        {
            soundHasPlayedDuringHover = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (!isHovered && audioSource.isPlaying)
        {
            audioSource.Stop();
            soundHasPlayedDuringHover = false;
        }
    }

    private void PlayHoverSound()
    {
        if (hoverVoice != null && audioSource != null)
        {
            audioSource.clip = hoverVoice;
            audioSource.loop = false;
            audioSource.Play();
            soundHasPlayedDuringHover = true;
        }
    }

    /*private void TryStopHoverSound()
    {
        if (!isHovered && !isPressed && audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }*/
}
