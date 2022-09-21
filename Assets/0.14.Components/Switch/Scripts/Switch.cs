using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    #region Public Parameter
    [Header("开启状态时背景颜色")]
    public Color SwitchOnColor;
    [Header("关闭状态时背景颜色")]
    public Color SwitchOffColor;
    [Header("开启状态时handle颜色")]
    public Color HandleOnColor;
    [Header("关闭状态时handle颜色")]
    public Color HandleOffColor;
    [Header("开启状态时字体颜色")]
    public Color ContentOnColor;
    [Header("关闭状态时字体颜色")]
    public Color ContentOffColor;
    #endregion

    #region Private Parameter
    //开关状态更改回调方法
    private Action<bool> onValueChanged;
    //获取背景和handle图片
    private Image[] images;
    //获取文字内容
    private Text content;
    //单选
    private Toggle toggle;
    //背景长度
    private float parentSize;
    //文字长度
    private float contentSize;
    //handle长度
    private float handleSize;
    //移动协程
    private Coroutine moveCoroutine;
    //是否需要进行回调
    private bool isNeedCallback = true;
    //开启状态文字内容
    private string openStr;
    //关闭状态文字内容
    private string closeStr;
    #endregion

    #region Interface
    /// <summary>
    /// 初始化，默认开关状态为false
    /// </summary>
    /// <param name="onValueChanged">开光状态更改回调方法</param>
    /// /// <param name="isNeedCallback">是否需要执行回调方法</param>
    /// <param name="openStr">开启状态时文字显示内容</param>
    /// <param name="closeStr">关闭状态时文字显示内容</param>
    public void InitSwitch(Action<bool> onValueChanged, bool isNeedCallback, string openStr, string closeStr)
    {
        this.onValueChanged = onValueChanged;
        this.openStr = openStr;
        this.closeStr = closeStr;
        toggle = this.GetComponent<Toggle>();
        toggle.isOn = false;
        images = GetComponentsInChildren<Image>();
        content = this.GetComponentInChildren<Text>();
        handleSize = images[1].GetComponent<RectTransform>().sizeDelta.x;
        contentSize = content.GetComponent<RectTransform>().sizeDelta.x;
        parentSize = GetComponent<RectTransform>().sizeDelta.x;
        toggle.onValueChanged.AddListener(ValueChange);
        SetStatus(isNeedCallback, false);
        ChangeColor(false);
    }

    /// <summary>
    /// 设置switch当前状态
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="isNeedCallback"></param>
    public void SetStatus(bool isOn, bool isNeedCallback)
    {
        this.isNeedCallback = isNeedCallback;

        toggle.isOn = isOn;
        this.isNeedCallback = true;
    }

    /// <summary>
    /// 设置switch是否可用
    /// </summary>
    /// <param name="isInteractbale"></param>
    public void SetInteractable(bool isInteractbale)
    {
        toggle.interactable = isInteractbale;
    }

    /// <summary>
    /// 获取当前开关状态
    /// </summary>
    /// <returns></returns>
    public bool GetSwitchStatus()
    {
        return toggle.isOn;
    }
    #endregion

    #region 内部逻辑
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="isOn"></param>
    /// <returns></returns>
    IEnumerator Move(bool isOn)
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            if (isOn)
            {
                images[1].transform.localPosition = Vector2.Lerp(Vector2.zero, new Vector2(parentSize - handleSize, 0), time * 5);
                content.transform.localPosition = Vector2.Lerp(new Vector2(parentSize - contentSize, 0), Vector2.zero, time * 5);
            }
            else
            {
                content.transform.localPosition = Vector2.Lerp(Vector2.zero, new Vector2(parentSize - contentSize, 0), time * 5);
                images[1].transform.localPosition = Vector2.Lerp(new Vector2(parentSize - handleSize, 0), Vector2.zero, time * 5);
            }
            yield return null;
        }
    }

    /// <summary>
    /// 给Toggle添加监听
    /// </summary>
    /// <param name="isOn"></param>
    private void ValueChange(bool isOn)
    {
        //Debug.Log(isNeedCallback);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(Move(isOn));
        }
        ChangeColor(isOn);
        if (!isNeedCallback)
        {
            isNeedCallback = true;
            return;
        }

        // Debug.Log(isOn);
        onValueChanged?.Invoke(isOn);
    }

    /// <summary>
    /// 根据当前状态更改显示部分颜色和文字
    /// </summary>
    /// <param name="isOn"></param>
    private void ChangeColor(bool isOn)
    {
        images[0].color = isOn ? SwitchOnColor : SwitchOffColor;
        images[1].color = isOn ? HandleOnColor : HandleOffColor;
        content.color = isOn ? ContentOnColor : ContentOffColor;
        content.text = isOn ? openStr : closeStr;
    }
    #endregion

}
