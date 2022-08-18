using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LimitUI_Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("表示限制的区域")]
    public RectTransform LimitContainer;
    [Header("场景中Canvas")]
    public Canvas canvas;

    RectTransform rt;
    // 位置偏移量
    Vector3 offset = Vector3.zero;
    // 最小、最大X、Y坐标
    float minX, maxX, minY, maxY;

    private Vector3 currentPos;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.enterEventCamera, out Vector3 globalMousePos))
        {
            // 计算偏移量
            offset = rt.position - globalMousePos;
            // 设置拖拽范围
            SetDragRange();
        }
    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // 将屏幕空间上的点转换为位于给定RectTransform平面上的世界空间中的位置
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            rt.position = DragRangeLimit(globalMousePos + offset);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentPos = rt.transform.localPosition;
        // Debug.Log(currentPos + "endDrag");
    }

    // 设置最大、最小坐标
    void SetDragRange()
    {
        // 最小x坐标 = 容器当前x坐标 - 容器轴心距离左边界的距离 + UI轴心距离左边界的距离
        minX = LimitContainer.position.x
            - LimitContainer.pivot.x * LimitContainer.rect.width * canvas.scaleFactor
            + rt.rect.width * canvas.scaleFactor * rt.pivot.x;
        // 最大x坐标 = 容器当前x坐标 + 容器轴心距离右边界的距离 - UI轴心距离右边界的距离
        maxX = LimitContainer.position.x
            + (1 - LimitContainer.pivot.x) * LimitContainer.rect.width * canvas.scaleFactor
            - rt.rect.width * canvas.scaleFactor * (1 - rt.pivot.x);

        // 最小y坐标 = 容器当前y坐标 - 容器轴心距离底边的距离 + UI轴心距离底边的距离
        minY = LimitContainer.position.y
            - LimitContainer.pivot.y * LimitContainer.rect.height * canvas.scaleFactor
            + rt.rect.height * canvas.scaleFactor * rt.pivot.y;

        // 最大y坐标 = 容器当前x坐标 + 容器轴心距离顶边的距离 - UI轴心距离顶边的距离
        maxY = LimitContainer.position.y
            + (1 - LimitContainer.pivot.y) * LimitContainer.rect.height * canvas.scaleFactor
            - rt.rect.height * canvas.scaleFactor * (1 - rt.pivot.y);
    }
    // 限制坐标范围
    Vector3 DragRangeLimit(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }

    /// <summary>
    /// 获取当前位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentPos()
    {
        // Debug.Log(currentPos + "get");
        return currentPos;
    }
}