# 动态边框使用说明

#### 说明

- 同一个物体上同时挂DynamicOutline和DrawOutline两个脚本
- DrawOutline上的材质球给一个默认新建材质球就可以
- intensity调的边框亮度比较细致，每次参数调整变化都要重新调用CreateLine

#### 参数

```c#
    //边框颜色
    public Color setColor = new Color(13.0f / 255.0f, 1, 247.0f / 255.0f, 1);
    //边框颜色强度
    [Range(4.0f, 20.0f)]
    public float intensity = 20.0f;
    //边框宽度
    public float lineWidth = 5;
    //边框辐射范围（相对于面片长宽百分比）
    public float moveRange = 0.05f;
    //边框移动速度
    public float moveSpeed = 0.04f;
    //边框移动间隔
    public float moveInterval = 0.1f;
```

#### 方法

```c#
    /// <summary>
    /// 开始绘制边框并进行移动
    /// </summary>
    public void CreateLine()

    /// <summary>
    /// 停止移动、删除边框
    /// </summary>
    public void DeleteLine()
```



2022-08-15：

总之先搬了一个可以根据顶点生成平面边框的shader，静态，不支持hdr颜色，如果这个东西还用的话再改吧
