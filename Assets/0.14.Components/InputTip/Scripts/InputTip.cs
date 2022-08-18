using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Blocker))]
public class InputTip : MonoBehaviour
{
    [Header("条目模板")]
    [SerializeField]
    protected GameObject itemTemplate;
    [Header("当前是否可以多选")]
    [SerializeField]
    protected bool isMutiple;

    //输入框
    protected InputField input;
    //TipView
    private ScrollRect content;
    //箭头图片
    protected Image arrowImg;

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
            arrowImg.transform.localScale = new Vector3(1, 1 * (value ? (-1) : (1)), 1);
            content.gameObject.SetActive(value);
        }
    }

    #region 方法
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void InitInputTip()
    {
        content = GetComponentInChildren<ScrollRect>();
        input = GetComponentInChildren<InputField>();
        arrowImg = this.transform.Find("Arrow").GetComponent<Image>();
        isShowContent = false;
        AddInputNameClickEvent();
        itemData.Clear();
        input.onValueChanged.AddListener(InputOnValueChanged);
    }

    /// <summary>
    /// 初始化【含数据
    /// </summary>
    /// <param name="dataList"></param>
    public virtual void InitInputTip(Dictionary<string, string> dataDic)
    {
        InitInputTip();
        SetValue(dataDic);
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="dataList"></param>
    public virtual void SetValue(Dictionary<string, string> dataDic)
    {
        itemData = dataDic;
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
    private void OnInputFieldClicked(BaseEventData data)
    {
        input.text = "";
        isShowContent = true;
        SetValue(itemData);
    }

    /// <summary>
    /// 输入框OnValueChanged回调
    /// </summary>
    /// <param name="value"></param>
    private void InputOnValueChanged(string value)
    {
        isShowContent = !string.IsNullOrEmpty(value);

        CreateTipItem(FilterString(value));
    }

    /// <summary>
    /// 输入框OnEndEdit回调
    /// </summary>
    /// <param name="value"></param>
    protected void OnEndEdit(string value)
    {
        input.text = value;
        isShowContent = false;
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
            if (item.Value.Contains(value))
            {
                filterStr.Add(item.Key, item.Value);
            }
        }
        return filterStr;
    }

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
            ob = leftTipItems[0].gameObject;
        }

        if (ob == null)
        {
            ob = Instantiate(itemTemplate);
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
        ob.GetComponent<InputTipItem>().InitItem(isMutiple, data);
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
