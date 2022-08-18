# 轮播图使用说明

#### 说明

- 使用时需要调用InitBanner()，如需要可传入单个元素移动结束后回调方法
- 可以通过传入数据生成，也可提前摆好
- 子物体继承RollCell脚本

#### 参数

```c#
    //生成时所需物体(如此部分为空则默认为手动摆好)
    public RollCell prefab;
    //是否需要正中元素重新刷新数据
    public bool isNeedUpdate = false;
    //物体之间间隔
    public float Spacing;
    //是否自动轮播
    public bool AutoLoop = true;
    //是否可以拖动
    public bool Drag = true;
    //移动方向
    public MoveDirection moveDirection = MoveDirection.RightToLeft;
    //过图速度（帧）
    public int tweenStepCount = 50;
    //停图时间（秒）
    public float LoopSpace = 3;
```

#### 方法

```c#
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="MoveEndCallback"></param>
    public void InitBanner(Action<int> MoveEndCallback = null)

    /// <summary>
    /// 根据传入数据生成、刷新预制体并开始移动
    /// </summary>
    /// <param name="dataList"></param>
    /// <typeparam name="T"></typeparam>
    public void StartMove<T>(List<T> dataList)

    /// <summary>
    /// 停止移动并关闭所有预制体
    /// </summary>
    public void EndMove()

    /// <summary>
    /// 移动到指定元素
    /// </summary>
    /// <param name="index"></param>
    public void SetItemIndex(int index)
```

