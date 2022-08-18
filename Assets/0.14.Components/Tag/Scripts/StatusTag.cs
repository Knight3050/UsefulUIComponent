using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusTag : MonoBehaviour
{
    /// <summary>
    /// 设置tag状态
    /// </summary>
    /// <param name="color">16进制</param>
    /// <param name="content"></param>
    public void SetStatus(string color, string content)
    {
        Color statusColor = new Color();
        ColorUtility.TryParseHtmlString(color, out statusColor);
        this.GetComponent<Image>().color = statusColor;
        this.GetComponentInChildren<Text>().color = statusColor;
        this.GetComponentInChildren<Text>().text = content;
    }

    /// <summary>
    /// 设置tag状态
    /// </summary>
    /// <param name="color">颜色</param>
    /// <param name="content"></param>
    public void SetStatus(Color color, string content)
    {
        this.GetComponent<Image>().color = color;
        this.GetComponentInChildren<Text>().color = color;
        this.GetComponentInChildren<Text>().text = content;
    }
}
