using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class PlaySoundOnButtonClick1 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private AudioSource audioSource;

    private Button button;
    private Toggle toggle;
    private Dropdown dropdown;
    private Slider slider;
    private Scrollbar scrollbar;

    private void Awake()
    {
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();
        dropdown = GetComponent<Dropdown>();
        slider = GetComponent<Slider>();
        scrollbar = GetComponent<Scrollbar>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource assigned on " + gameObject.name, this);
            return;
        }

        // Add listeners for elements that support onClick or onValueChanged
        if (button != null)
            button.onClick.AddListener(PlaySound1);
        else if (toggle != null)
            toggle.onValueChanged.AddListener((_) => PlaySound1());
        else if (dropdown != null)
            dropdown.onValueChanged.AddListener((_) => PlaySound1()); // fallback
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only play once when pointer is pressed down, not dragged
        if (scrollbar != null)
        {
            PlaySound1();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Only play once when pointer is pressed down, not dragged
        if (slider != null || dropdown != null)
        {
            PlaySound1();
        }
    }

    private void PlaySound1()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.Play();
    }
}
