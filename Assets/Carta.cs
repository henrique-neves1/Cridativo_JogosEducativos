using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class Carta : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public Sprite hiddenIconSprite;
    public Sprite iconSprite;

    public bool isSelected;
    //public bool isFlipping = false;

    public CartasController controller;

    //public bool isFlipping = false;


    public void OnCartaClick()
    {
        Debug.Log("OnCartaClick triggered");
        controller.SetSelected(this);
    }

    public void GreyOut()
    {
        // Get all Image components in this card and its children
        Image[] images = GetComponentsInChildren<Image>();

        foreach (var img in images)
        {
            img.color = Color.grey; // Applies grey tint to everything
        }

        // Optionally disable interaction
        GetComponent<Button>().interactable = false;
    }


    public void SetIconSprite (Sprite sp)
    {
        iconSprite = sp;
    }

    public void Show()
    {
       // if (isFlipping) return;
        //isFlipping = true;

        Tween.ScaleX(transform, 0f, 0.1f)
            .OnComplete(() =>
            {
                iconImage.sprite = iconSprite;
                Tween.ScaleX(transform, 1f, 0.1f);
                isSelected = true;
                //isFlipping = false;
            });
    }

    public void Hide()
    {
        //if (isFlipping) return;
        //isFlipping = true;

        Tween.ScaleX(transform, 0f, 0.1f)
           .OnComplete(() =>
           {
            iconImage.sprite = hiddenIconSprite;
            Tween.ScaleX(transform, 1f, 0.1f);
            isSelected = false;
            //isFlipping = false;
           });


    }

    public void Disable()
    {
        isSelected = true;
        GetComponent<Button>().interactable = false;
        iconImage.color = new Color(0.5f, 0.5f, 0.5f); // greys out
    }

    /*public void ResetState()
    {
        isSelected = false;
        // Reset other flags if needed
    }*/

}
