using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputNumber : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region 
    [Header("上限值")]
    public float maxValue;
    [Header("下限值")]
    public float minValue;
    [Header("幅度")]
    public float frequency;
    [Header("单位")]
    public string unit;
    [Header("单位位置")]
    public UnitPos unitPos = UnitPos.back;
    [Header("是否按钮常显")]
    public bool ifBtnShow = false;
    #endregion

    #region 
    //编辑结束回调方法
    private Action<float> ChangeNumCallback;
    //输入框
    private InputField inputfield;
    //递增、递减按钮
    private Button[] changeNumBtns;
    //缓存数据
    private float currentData;
    #endregion

    #region Interface
    /// <summary>
    /// 初始化
    /// </summary>
    public void InitInput(Action<float> ChangeNumCallback = null)
    {
        this.ChangeNumCallback = ChangeNumCallback;
        inputfield = GetComponent<InputField>();
        changeNumBtns = GetComponentsInChildren<Button>();
        inputfield.onValueChanged.AddListener(InputOnValueChanged);
        inputfield.onEndEdit.AddListener(EndEdit);
        changeNumBtns[0].onClick.AddListener(UpCountData);
        changeNumBtns[1].onClick.AddListener(DownCountData);
    }

    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value)
    {
        inputfield.text = unitPos == UnitPos.back ? $"{value}{unit}" : $"{unit}{value}";
    }

    /// <summary>
    /// 返回当前值
    /// </summary>
    /// <value></value>
    public float CurrentData
    {
        get
        {
            return currentData;
        }
    }
    #endregion

    #region 
    /// <summary>
    /// 递增按钮方法
    /// </summary>
    private void UpCountData()
    {
        currentData = currentData + frequency;
        inputfield.text = unitPos == UnitPos.back ? $"{currentData}{unit}" : $"{unit}{currentData}";
        EndEdit("");
    }

    /// <summary>
    /// 递减按钮方法
    /// </summary>
    private void DownCountData()
    {
        currentData = currentData - frequency;
        inputfield.text = unitPos == UnitPos.back ? $"{currentData}{unit}" : $"{unit}{currentData}";
        EndEdit("");
    }

    /// <summary>
    /// 输入框监听
    /// </summary>
    /// <param name="inputValue"></param>
    private void InputOnValueChanged(string inputValue)
    {
        if (string.IsNullOrEmpty(inputValue))
        {
            currentData = 0;
        }
        else
        {
            if (string.IsNullOrEmpty(unit))
            {
                currentData = float.Parse(inputValue);
            }
            else
            {
                currentData = string.IsNullOrEmpty(inputValue.Replace(unit, "")) ? 0 : float.Parse(inputValue.Replace(unit, ""));
            }

            changeNumBtns[0].interactable = currentData < maxValue;
            changeNumBtns[1].interactable = currentData > minValue;
            currentData = currentData < minValue ? minValue : currentData;
            currentData = currentData > maxValue ? maxValue : currentData;
            //ChangeNumCallback?.Invoke(currentData);
            inputfield.text = unitPos == UnitPos.back ? $"{currentData}{unit}" : $"{unit}{currentData}";
        }
    }

    /// <summary>
    /// 输入框结束编辑监听
    /// </summary>
    /// <param name="inputValue"></param>
    private void EndEdit(string inputValue)
    {
        ChangeNumCallback?.Invoke(currentData);
    }

    /// <summary>
    /// 调整数值按钮是否显示
    /// </summary>
    /// <param name="isOver"></param>
    private void SetStatus(bool isOver)
    {
        for (int i = 0; i < changeNumBtns.Length; i++)
        {
            changeNumBtns[i].gameObject.SetActive(isOver);
        }
    }

    #region 鼠标移入和移出操作
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ifBtnShow)
        {
            SetStatus(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!ifBtnShow)
        {
            SetStatus(false);
        }
    }
    #endregion
    #endregion
}

public enum UnitPos
{
    back = 0,
    front = 1
}