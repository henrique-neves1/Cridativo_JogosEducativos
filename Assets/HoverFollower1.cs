using UnityEngine;
using UnityEngine.EventSystems;

public class HoverFollower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject followerObject;
    private bool isHovering = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (isHovering && followerObject != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            followerObject.transform.position = mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (followerObject != null)
        {
            followerObject.SetActive(true);
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (followerObject != null)
        {
            followerObject.SetActive(false);
            isHovering = false;
        }
    }

    /*public void OnPointerUp(PointerEventData eventData)
    {
        if (followerObject != null)
        {
            followerObject.SetActive(false);
            isHovering = false;
        }
    }*/
}
