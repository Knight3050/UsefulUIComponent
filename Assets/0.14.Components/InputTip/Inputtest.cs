using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputtest : MonoBehaviour
{
    [SerializeField]
    private SingleInputTip inputTip;

    [SerializeField]
    private MultipleInputTip multipleInputTip;

    void Start()
    {
        Dictionary<string, string> strList = new Dictionary<string, string>();
        for (var i = 0; i < 10; i++)
        {
            strList.Add(i.ToString(), i.ToString() + ";;;" + Random.Range(0, 10).ToString());
        }

        inputTip.InitInputTip(strList);
        multipleInputTip.InitInputTip(strList);
    }

    [ContextMenu("七夕青蛙特供")]
    private void ValentineFrog()
    {
        string content = "孤寡";
        int benefit = 1;

        string result = "";
        for (var i = 0; i < benefit * 50; i++)
        {
            result = string.Join(" ", content, result);
        }

        Debug.Log(result);
    }

    [ContextMenu("获取单选数据")]
    private void GetSingleData()
    {
        Debug.Log(inputTip.ReturnSelectData().Key + ":::::" + inputTip.ReturnSelectData().Value);
    }

    [ContextMenu("获取多选数据")]
    private void GetMultipleData()
    {
        foreach (var item in multipleInputTip.ReturnSelectData())
        {
            Debug.Log(item.Key + ";;;;;" + item.Value);
        }
    }

    [ContextMenu("设置多选数据显示")]
    private void SetMultipleData()
    {
        List<string> keys = new List<string>() { "1", "3", "5" };
        multipleInputTip.SetSelectData(keys);
    }
}