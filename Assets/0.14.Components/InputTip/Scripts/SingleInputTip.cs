using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleInputTip : InputTip
{
    private KeyValuePair<string, string> selectData = new KeyValuePair<string, string>();

    #region 方法
    /// <summary>
    /// Tip点击方法
    /// </summary>
    /// <param name="item"></param>
    public override void OnItemClick(InputTipItem item)
    {
        selectData = item.Label;
        OnEndEdit(item.Label.Value);
        base.OnItemClick(item);
    }

    /// <summary>
    /// 返回选中数据
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<string, string> ReturnSelectData()
    {
        return selectData;
    }

    /// <summary>
    /// 根据key设置选中
    /// </summary>
    /// <param name="key"></param>
    public void SetSelectData(string key)
    {
        for (int i = 0; i < usingTipItems.Count; i++)
        {
            if (usingTipItems[i].Label.Key == key)
            {
                usingTipItems[i].isOn = true;
                OnItemClick(usingTipItems[i]);
                return;
            }
        }
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    public void ClearInputContent()
    {
        if (selectData.Key != null)
        {
            for (int i = 0; i < usingTipItems.Count; i++)
            {
                if (usingTipItems[i].Label.Key == selectData.Key)
                {
                    usingTipItems[i].isOn = false;
                    selectData = new KeyValuePair<string, string>();
                    continue;
                }
            }
        }
        input.text = string.Empty;
    }
    #endregion

    protected override void OnInputFieldClicked(BaseEventData data)
    {
        base.OnInputFieldClicked(data);

        for (int i = 0; i < usingTipItems.Count; i++)
        {
            usingTipItems[i].isOn = usingTipItems[i].Label.Key.Equals(selectData.Key);
        }
    }
}