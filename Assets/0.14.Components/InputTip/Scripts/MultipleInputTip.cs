using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: 暂时关闭输入框交互，不能进行筛选，但仍然留有筛选逻辑

public class MultipleInputTip : InputTip
{
    [Header("多选选中显示按钮模版")]
    [SerializeField]
    private GameObject buttonTemplate;

    [SerializeField]
    private Transform itemBtnList;

    #region 生成池
    private List<InputButton> leftInputButton = new List<InputButton>();
    private List<InputButton> usingInputButton = new List<InputButton>();
    private int rowCount;
    private float rectx, recty;
    #endregion

    #region 方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void InitInputTip(Action callback)
    {
        base.InitInputTip(callback);
        input.interactable = false;
        input.onValueChanged.RemoveAllListeners();

        rowCount = itemBtnList.GetComponent<GridLayoutGroup>().constraintCount;
        rectx = this.GetComponent<RectTransform>().sizeDelta.x;
        recty = this.GetComponent<RectTransform>().sizeDelta.y;
    }

    public override void SetValue(Dictionary<string, string> dataDic)
    {
        base.SetValue(dataDic);
        // this.GetComponent<RectTransform>().sizeDelta = new Vector2(rectx, recty);
        // DestroyAllButton();
    }

    /// <summary>
    /// Tip点击方法
    /// </summary>
    /// <param name="item"></param>
    public override void OnItemClick(InputTipItem item)
    {
        CreateInputButton(item.Label);
        input.text = " ";
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemBtnList.GetComponent<RectTransform>());
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(rectx,
        (usingInputButton.Count % rowCount == 0 ? usingInputButton.Count / rowCount : usingInputButton.Count / rowCount + 1) * recty);
        // this.GetComponent<RectTransform>().sizeDelta = input.GetComponent<RectTransform>().sizeDelta;
        callback?.Invoke();
        base.OnItemClick(item);
    }

    /// <summary>
    /// 设置已选中数据
    /// /// </summary>
    /// <param name="dataDic"></param>
    public void SetSelectData(List<string> keys)
    {
        DestroyAllButton();
        if (keys.Count > 0)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                // Debug.Log(keys[i]);
                if (GetTipButton(keys[i]))
                {
                    continue;
                }
                InputTipItem tipItem = GetInputTipItem(keys[i]);
                if (tipItem != null)
                {
                    tipItem.isOn = true;
                    OnItemClick(tipItem);
                }
                else
                {
                    Debug.Log("null");
                }
            }
        }
    }

    /// <summary>
    /// 返回选中数据
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> ReturnSelectData()
    {
        Dictionary<string, string> resultDic = new Dictionary<string, string>();
        for (var i = 0; i < usingInputButton.Count; i++)
        {
            resultDic.Add(usingInputButton[i].GetData().Key, usingInputButton[i].GetData().Value);
        }
        return resultDic;
    }
    #endregion

    protected override void Hide()
    {
        itemBtnList.GetComponent<Canvas>().sortingOrder = 10;
        base.Hide();
    }

    protected override void Show()
    {
        itemBtnList.GetComponent<Canvas>().sortingOrder = 30000;
        base.Show();
    }

    /// <summary>
    /// 根据key获取条目物体
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public InputTipItem GetInputTipItem(string key)
    {
        for (int i = 0; i < usingTipItems.Count; i++)
        {
            if (usingTipItems[i].Label.Key.Equals(key))
            {
                return usingTipItems[i];
            }
        }
        return null;
    }

    #region 操作选中按钮模板
    /// <summary>
    /// 根据模版创建一个按钮
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private InputButton CreateInputButton(KeyValuePair<string, string> value)
    {
        GameObject ob = null;
        if (leftInputButton.Count > 0)
        {
            ob = leftInputButton[0].gameObject;
        }

        if (ob == null)
        {
            ob = Instantiate(buttonTemplate);
            ob.GetComponent<InputButton>().InitInputButton(this);
        }
        else
        {
            leftInputButton.Remove(ob.GetComponent<InputButton>());
        }

        ob.gameObject.SetActive(true);
        ob.transform.SetParent(itemBtnList);
        ob.transform.localPosition = Vector3.zero;
        ob.transform.localScale = Vector3.one;
        usingInputButton.Add(ob.GetComponent<InputButton>());
        ob.GetComponent<InputButton>().SetLabel(value);

        // input.GetComponent<RectTransform>().sizeDelta = itemBtnList.GetComponent<RectTransform>().sizeDelta;
        return ob.GetComponent<InputButton>();
    }

    /// <summary>
    /// 销毁一个按钮
    /// </summary>
    /// <param name="item"></param>
    public void DestroyButton(InputButton item)
    {
        usingInputButton.Remove(item);
        leftInputButton.Add(item);
        item.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemBtnList.GetComponent<RectTransform>());

        if (usingInputButton.Count == 0)
        {
            input.text = string.Empty;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(rectx, recty);
        }
        else
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(rectx,
            (usingInputButton.Count % rowCount == 0 ? usingInputButton.Count / rowCount : usingInputButton.Count / rowCount + 1) * recty);
        }

        callback?.Invoke();
        // input.GetComponent<RectTransform>().sizeDelta =
        // new Vector2(itemBtnList.GetComponent<RectTransform>().sizeDelta.x + arrowImg.rectTransform.sizeDelta.x,
        // itemBtnList.GetComponent<RectTransform>().sizeDelta.y);
        // this.GetComponent<RectTransform>().sizeDelta = input.GetComponent<RectTransform>().sizeDelta;
    }

    public void DestroyButton(KeyValuePair<string, string> value)
    {
        for (var i = 0; i < usingInputButton.Count; i++)
        {
            if (usingInputButton[i].GetData().Key == value.Key)
            {
                DestroyButton(usingInputButton[i]);
            }
        }
    }

    /// <summary>
    /// 销毁所有按钮
    /// </summary>
    private void DestroyAllButton()
    {
        if (usingInputButton.Count > 0)
        {
            for (var i = 0; i < usingInputButton.Count; i++)
            {
                usingInputButton[i].gameObject.SetActive(false);
                leftInputButton.Add(usingInputButton[i]);
            }
            usingInputButton.Clear();
        }

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(rectx, recty);

        for (int i = 0; i < usingTipItems.Count; i++)
        {
            usingTipItems[i].isOn = false;
        }
    }

    private InputButton GetTipButton(string key)
    {
        if (usingInputButton.Count > 0)
        {
            for (int i = 0; i < usingInputButton.Count; i++)
            {
                if (usingInputButton[i].GetData().Key.Equals(key))
                {
                    return usingInputButton[i];
                }
            }
        }
        return null;
    }
    #endregion
}
