using System.Collections.Generic;

public class SingleInputTip : InputTip
{
    #region 方法
    /// <summary>
    /// Tip点击方法
    /// </summary>
    /// <param name="item"></param>
    public override void OnItemClick(InputTipItem item)
    {
        OnEndEdit(item.Label.Value);
        base.OnItemClick(item);
    }

    /// <summary>
    /// 返回选中数据【即InputField.text
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<string, string> ReturnSelectData()
    {
        foreach (var item in itemData)
        {
            if (item.Value.Equals(input.text))
            {
                return item;
            }
        }
        return new KeyValuePair<string, string>();
    }
    #endregion
}