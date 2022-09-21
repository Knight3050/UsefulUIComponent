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