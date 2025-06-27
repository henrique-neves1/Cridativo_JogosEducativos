using UnityEngine;
using UnityEngine.UI;

public class SectionChangeButton : MonoBehaviour
{
    public Button button1;
    public Button button2;

    void Start()
    {
        button1.onClick.AddListener(() => OnButtonClick(button1, button2));
        button2.onClick.AddListener(() => OnButtonClick(button2, button1));
    }

    void OnButtonClick(Button activeButton, Button inactiveButton)
    {
        Color32 opaqueWhite = new Color32(255, 255, 255, 255);
        ColorBlock activeColors = activeButton.colors;
        activeColors.normalColor = opaqueWhite;
        activeColors.highlightedColor = opaqueWhite;
        activeColors.pressedColor = opaqueWhite;
        activeColors.selectedColor = opaqueWhite;
        activeButton.colors = activeColors;

        ColorBlock inactiveColors = inactiveButton.colors;
        inactiveColors.normalColor = new Color32(255, 255, 255, 0);
        inactiveColors.highlightedColor = new Color32(255, 255, 255, 50);
        inactiveColors.pressedColor = new Color32(255, 255, 255, 100);
        inactiveColors.selectedColor = new Color32(255, 255, 255, 50);
        inactiveButton.colors = inactiveColors;
    }
}
