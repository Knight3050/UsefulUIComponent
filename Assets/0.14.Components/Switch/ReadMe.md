# 开关使用说明

#### 说明

- 使用时需要调用InitSwitch()，如需要传入开关状态更改方法、是否初始化需要执行回调方法、开启和关闭状态文字显示内容
- 如不需要文字，初始化文字string传空
- 可以通过调用获取和更改switch当前状态、更改switch是否可用

#### 参数

```c#
    //开启状态时背景颜色
    public Color SwitchOnColor;
    //关闭状态时背景颜色
    public Color SwitchOffColor;
    //开启状态时handle颜色
    public Color HandleOnColor;
    //关闭状态时handle颜色
    public Color HandleOffColor;
    //开启状态时字体颜色
    public Color ContentOnColor;
	//关闭状态时字体颜色
    public Color ContentOffColor;
```

#### 方法

```c#
    /// <summary>
    /// 初始化，默认开关状态为false
    /// </summary>
    /// <param name="onValueChanged">开光状态更改回调方法</param>
    /// /// <param name="isNeedCallback">是否需要执行回调方法</param>
    /// <param name="openStr">开启状态时文字显示内容</param>
    /// <param name="closeStr">关闭状态时文字显示内容</param>
    public void InitSwitch(Action<bool> onValueChanged, bool isNeedCallback, string openStr, string closeStr)

    /// <summary>
    /// 设置switch当前状态
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="isNeedCallback"></param>
    public void SetStatus(bool isOn, bool isNeedCallback)

    /// <summary>
    /// 设置switch是否可用
    /// </summary>
    /// <param name="isInteractbale"></param>
    public void SetInteractable(bool isInteractbale)

    /// <summary>
    /// 获取当前开关状态
    /// </summary>
    /// <returns></returns>
    public bool GetSwitchStatus()
    {
        return toggle.isOn;
    }
```

