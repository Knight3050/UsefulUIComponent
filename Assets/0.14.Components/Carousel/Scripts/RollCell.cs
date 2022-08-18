using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*
* 作者：张骥
* 时间：2021-03-03
* 作用：轮播图子物体
*/
public class RollCell : MonoBehaviour
{

    /// <summary>
    /// 初始化物体
    /// </summary>
    public virtual void InitItem()
    {

    }

    /// <summary>
    /// 显示物体，并且刷新数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    public void ShowItem(int index, object data)
    {
        gameObject.SetActive(true);
        UpdateData(index, data);
    }

    /// <summary>
    /// 隐藏物体
    /// </summary>
    public void DisShowItem()
    {
        ClearData();
        gameObject.SetActive(false);
    }


    /// <summary>
    /// 无论自动轮播还是停止都会执行
    /// </summary>
    /// <param name="index"></param>
    public virtual void UpdateData(int index, object data)
    {

    }
    /// <summary>
    /// 清除数据
    /// </summary>
    protected virtual void ClearData()
    {

    }
}
