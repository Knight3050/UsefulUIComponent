using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 作者：张骥
 * 时间：2022-01-25
 * 作用：公告板输入框，content大小随输入框输入内容改变
 */

public class InputToContent : MonoBehaviour
{
    private static readonly string no_breaking_space = "\u00A0";

    public InputField m_InputField;
    Text m_Text;

    public void Init()
    {
        m_InputField = this.GetComponent<InputField>();
        m_Text = this.transform.parent.GetComponent<Text>();

        m_InputField.onValueChanged.AddListener((value) =>
        {
            m_InputField.text = m_InputField.text.Replace(" ", no_breaking_space);
            m_Text.text = m_InputField.text;
        }
        );
    }
}
