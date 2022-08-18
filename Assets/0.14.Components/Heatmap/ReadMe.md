# 热力图使用说明

#### 说明

- 点数上限为1000
- 绘制热力图时续传入待绘制点的世界坐标以及对应的强度

#### 参数

```shader
//根据不同密度，点绘制颜色，标号越高对应密度越高，一般默认最低密度透明度为0
_Color0("Color 0",Color) = (0,0,0,0)
_Color1("Color 1",Color) = (0,0,1,1)
_Color2("Color 2",Color) = (0,0.9,0.2,1)
_Color3("Color 3",Color) = (0.9,1,0.3,1)
_Color4("Color 4",Color) = (0.9,0.7,0.1,1)
_Color5("Color 5",Color) = (1,0,0,1)
//绘制点半径
area_of_effec_size("Area Effect Size", Range(0,1)) = 0.25
```

#### 方法

```c#
    /// <summary>
    /// 绘制热力图
    /// </summary>
    /// <param name="dataDic">传入数据，key为场景内数据点位置，value为强度</param>
    public void DrawHeat(Dictionary<Vector3, float> dataDic)

    /// <summary>
    /// 清除绘制数据
    /// </summary>
    public void ClearDraw()
```

