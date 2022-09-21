using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class InputTip : MonoBehaviour
{
    [Header("条目模板")]
    [SerializeField]
    protected GameObject itemTemplate;
    [Header("当前是否可以多选")]
    [SerializeField]
    protected bool isMutiple;
    [Header("未选中时tip文字颜色")]
    [SerializeField]
    private Color unselectColor;
    [Header("选中时tip文字颜色")]
    [SerializeField]
    private Color selectColor;

    //箭头图片
    protected Image arrowImg;
    //输入框
    protected InputField input;
    //TipView
    private ScrollRect content;

    protected Action callback;

    private GameObject m_Blocker;
    Canvas rootCanvas;

    #region 条目生成池
    protected List<InputTipItem> leftTipItems = new List<InputTipItem>();
    protected List<InputTipItem> usingTipItems = new List<InputTipItem>();
    #endregion


    //原始数据
    protected Dictionary<string, string> itemData = new Dictionary<string, string>();

    //是否展示TipView
    public bool isShowContent
    {
        set
        {
            // Debug.Log(content.name);
            arrowImg.transform.localScale = new Vector3(1, 1 * (value ? (-1) : (1)), 1);
            content.gameObject.SetActive(value);
        }
    }

    #region 方法
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void InitInputTip(Action callback = null)
    {
        content = GetComponentInChildren<ScrollRect>();
        input = GetComponentInChildren<InputField>();

        // Debug.Log(content.name + ";;;;" + input.name);
        arrowImg = this.transform.Find("Arrow").GetComponent<Image>();
        Hide();
        AddInputNameClickEvent();
        itemData.Clear();
        input.onValueChanged.AddListener(InputOnValueChanged);
        this.callback = callback;
    }

    /// <summary>
    /// 初始化【含数据
    /// </summary>
    /// <param name="dataList"></param>
    public virtual void InitInputTip(Dictionary<string, string> dataDic, Action callback = null)
    {
        InitInputTip(callback);
        SetValue(dataDic);
    }

    /// <summary>
    /// 刷新tip数据
    /// </summary>
    /// <param name="dataList"></param>
    public virtual void SetValue(Dictionary<string, string> dataDic)
    {
        itemData = dataDic;
        // Debug.Log("Init");
        CreateTipItem(dataDic);
    }
    #endregion

    /// <summary>
    /// Tip点击方法
    /// </summary>
    /// <param name="item"></param>
    public virtual void OnItemClick(InputTipItem item)
    {

    }

    /// <summary>
    /// InputField添加点击事件
    /// </summary>
    private void AddInputNameClickEvent()
    {
        var eventTrigger = input.gameObject.AddComponent<EventTrigger>();
        UnityAction<BaseEventData> selectEvent = OnInputFieldClicked;
        EventTrigger.Entry onClick = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerClick
        };

        onClick.callback.AddListener(selectEvent);
        eventTrigger.triggers.Add(onClick);
    }


    /// <summary>
    /// 点击InputField事件
    /// </summary>
    /// <param name="data"></param>
    protected virtual void OnInputFieldClicked(BaseEventData data)
    {
        if (isMutiple)
        {
            input.text = " ";
        }
        Show();

        // Debug.Log("ClickInputField");
        CreateTipItem(itemData);

    }

    /// <summary>
    /// 输入框OnValueChanged回调
    /// </summary>
    /// <param name="value"></param>
    private void InputOnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Hide();
        }
        else
        {
            Show();
        }

        // Debug.Log("onValueChanged");

        CreateTipItem(FilterString(value));
    }

    /// <summary>
    /// 输入框OnEndEdit回调
    /// </summary>
    /// <param name="value"></param>
    protected void OnEndEdit(string value)
    {
        input.text = value;
        Hide();
    }

    /// <summary>
    /// 筛选
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private Dictionary<string, string> FilterString(string value)
    {
        Dictionary<string, string> filterStr = new Dictionary<string, string>();
        foreach (var item in itemData)
        {
            if (item.Value.Contains(value.Trim()))
            {
                filterStr.Add(item.Key, item.Value);
            }
        }
        return filterStr;
    }

    #region blocker
    protected virtual void Hide()
    {
        isShowContent = false;

        if (m_Blocker != null)
            DestroyBlocker(m_Blocker);
        m_Blocker = null;
        // Select();
    }

    protected virtual void Show()
    {
        isShowContent = true;
        if (m_Blocker != null)
        {
            return;
        }
        var list = new List<Canvas>();
        gameObject.GetComponentsInParent(false, list);
        if (list.Count == 0)
            return;
        // case 1064466 rootCanvas should be last element returned by GetComponentsInParent()
        var listCount = list.Count;
        rootCanvas = list[listCount - 1];
        for (int i = 0; i < listCount; i++)
        {
            if (list[i].isRootCanvas || list[i].overrideSorting)
            {
                rootCanvas = list[i];
                break;
            }
        }

        m_Blocker = CreateBlocker(rootCanvas);
    }

    /// <summary>
    /// Create a blocker that blocks clicks to other controls while the dropdown list is open.
    /// </summary>
    /// <remarks>
    /// Override this method to implement a different way to obtain a blocker GameObject.
    /// </remarks>
    /// <param name="rootCanvas">The root canvas the dropdown is under.</param>
    /// <returns>The created blocker object</returns>
    protected virtual GameObject CreateBlocker(Canvas rootCanvas)
    {
        // Create blocker GameObject.
        GameObject blocker = new GameObject("Blocker");

        // Setup blocker RectTransform to cover entire root canvas area.
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.SetParent(rootCanvas.transform, false);
        blockerRect.anchorMin = Vector3.zero;
        blockerRect.anchorMax = Vector3.one;
        blockerRect.sizeDelta = Vector2.zero;

        // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
        Canvas blockerCanvas = blocker.AddComponent<Canvas>();
        blockerCanvas.overrideSorting = true;
        //放置到最外层，防止无法点击
        blockerCanvas.sortingOrder = 29999;
        //Canvas dropdownCanvas = m_Dropdown.GetComponent<Canvas>();
        //blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
        //blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

        // Find the Canvas that this dropdown is a part of
        Canvas parentCanvas = null;
        Transform parentTransform = content.transform.parent;
        while (parentTransform != null)
        {
            parentCanvas = parentTransform.GetComponent<Canvas>();
            if (parentCanvas != null)
                break;

            parentTransform = parentTransform.parent;
        }

        // If we have a parent canvas, apply the same raycasters as the parent for consistency.
        if (parentCanvas != null)
        {
            Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
            for (int i = 0; i < components.Length; i++)
            {
                Type raycasterType = components[i].GetType();
                if (blocker.GetComponent(raycasterType) == null)
                {
                    blocker.AddComponent(raycasterType);
                }
            }
        }
        else
        {
            // Add raycaster since it's needed to block.
            GetOrAddComponent<GraphicRaycaster>(blocker);
        }


        // Add image since it's needed to block, but make it clear.
        Image blockerImage = blocker.AddComponent<Image>();
        blockerImage.color = Color.clear;

        // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
        Button blockerButton = blocker.AddComponent<Button>();
        blockerButton.onClick.AddListener(Hide);

        return blocker;
    }

    protected virtual void DestroyBlocker(GameObject blocker)
    {
        Destroy(blocker);
    }

    private static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (!comp)
            comp = go.AddComponent<T>();
        return comp;
    }
    #endregion

    #region 操作下拉框内容模板
    /// <summary>
    /// 根据模版创建一个条目物体
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected InputTipItem CreateTipItem(KeyValuePair<string, string> data)
    {
        GameObject ob = null;
        if (leftTipItems.Count > 0)
        {
            for (int i = 0; i < leftTipItems.Count; i++)
            {
                if (leftTipItems[i].Label.Key.Equals(data.Key))
                {
                    ob = leftTipItems[i].gameObject;
                    continue;
                }
            }
            // ob = leftTipItems[0].gameObject;
        }

        if (ob == null)
        {
            ob = Instantiate(itemTemplate);
            ob.GetComponent<InputTipItem>().InitItem(this, isMutiple, selectColor, unselectColor);
        }
        else
        {
            ob.gameObject.SetActive(true);
            leftTipItems.Remove(ob.GetComponent<InputTipItem>());
        }

        ob.transform.SetParent(content.content);
        ob.transform.localPosition = Vector3.zero;
        ob.transform.localScale = Vector3.one;
        usingTipItems.Add(ob.GetComponent<InputTipItem>());
        ob.GetComponent<InputTipItem>().SetLabel(data);
        return ob.GetComponent<InputTipItem>();
    }

    /// <summary>
    /// 根据数据列表生成所有条目
    /// </summary>
    /// <param name="dataList"></param>
    protected void CreateTipItem(Dictionary<string, string> dataDic)
    {
        DestroyItemView();
        foreach (var item in dataDic)
        {
            // Debug.Log(item.Key);
            CreateTipItem(item);
        }
    }

    /// <summary>
    /// 销毁一个条目
    /// </summary>
    /// <param name="item"></param>
    protected void DestroyItem(InputTipItem item)
    {
        usingTipItems.Remove(item);
        leftTipItems.Add(item);
        item.gameObject.SetActive(false);
    }

    /// <summary>
    /// 销毁所有条目
    /// </summary>
    protected void DestroyItemView()
    {
        for (var i = 0; i < usingTipItems.Count; i++)
        {
            usingTipItems[i].gameObject.SetActive(false);
            leftTipItems.Add(usingTipItems[i]);
        }
        usingTipItems.Clear();
    }
    #endregion
}
