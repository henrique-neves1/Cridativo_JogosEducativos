using UnityEngine;
using UnityEngine.UI;

public class ImageOrSound : MonoBehaviour
{
    public Button targetButton;
    public GameObject sectionToOpen;
    public GameObject sectionToClose;

    private void Start()
    {
        targetButton.onClick.AddListener(SwapSections);
    }

    public void SwapSections()
    {
        if (sectionToOpen != null)
            sectionToOpen.SetActive(true);

        if (sectionToClose != null)
            sectionToClose.SetActive(false);
    }

}
