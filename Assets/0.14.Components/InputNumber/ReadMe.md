# 数字输入框使用说明

#### 说明

- 使用时需要调用InitInput()，如需要可传入每次修改数值后回调方法
- 调用修改数值的回调方法基于inputfield监听OnEndEdit，不结束输入或者直接赋值无法调用，点按钮修改可以调用

#### 参数

```c#
    //上限值
    public float maxValue;
    //下限值
    public float minValue;
    //幅度
    public float frequency;
    //单位
    public string unit;
    //单位位置
    public UnitPos unitPos = UnitPos.back;
    //是否按钮常显
    public bool ifBtnShow = false;
```

#### 方法

```c#
    /// <summary>
    /// 初始化
    /// </summary>
	public void InitInput(Action<float> ChangeNumCallback = null)
        
    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value)
        
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
```

