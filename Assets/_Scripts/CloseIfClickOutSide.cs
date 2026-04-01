using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseIfClickOutSide : MonoBehaviour, IPointerEnterHandler,
                             IPointerExitHandler
{
    public bool mouseOnTop = false;

    private void Update()
    {
        if (!mouseOnTop)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (this.gameObject.activeSelf)
                {
                    this.gameObject.SetActive(false);
                    ObjectPlacer.OnMenu = false;
                    mouseOnTop = true;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOnTop = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOnTop = false;
    }

}
