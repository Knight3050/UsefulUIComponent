using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Blocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isPointerInside;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isPointerInside)
            {
                var inputTip = GetComponent<InputTip>();
                if (inputTip != null)
                {
                    inputTip.isShowContent = false;
                }
            }
        }
    }
}
