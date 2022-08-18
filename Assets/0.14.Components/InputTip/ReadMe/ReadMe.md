---
typora-root-url: ./
---

# 可输入下拉框使用说明

#### 说明

##### 功能描述

可对输入内容进行筛选显示的下拉框，返回数据格式为KeyValuePair<string, string>

##### 组件效果展示

![](/可输入下拉框.gif)

##### 组件结构

![](/组件结构.png)

##### 说明

- 点击空白处关闭content，点击输入框清空输入框内容并展示content（包含全部数据）
- 因为与可多选下拉框共用部分代码，所以isMultiple需要手动取消勾选
- 具体使用可以参考inputtest.cs脚本

##### 搭载脚本

挂载在输入框上：*SingleInputTip.cs*、*Blocker.cs*

挂载在itemTemplate上：*InputTipItem.cs*

#### 参数

```c#
    [Header("条目模板")]
    [SerializeField]
    protected GameObject itemTemplate;
    [Header("当前是否可以多选")]
    [SerializeField]
    protected bool isMutiple;
```

#### 方法

```c#
  	/// <summary>
    /// 初始化
    /// </summary>
    public virtual void InitInputTip(){}

    /// <summary>
    /// 初始化【含数据
    /// </summary>
    /// <param name="dataList"></param>
    public virtual void InitInputTip(Dictionary<string, string> dataDic){}

    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="dataList"></param>
    public virtual void SetValue(Dictionary<string, string> dataDic){}

    /// <summary>
    /// 返回选中数据
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<string, string> ReturnSelectData(){}
```

