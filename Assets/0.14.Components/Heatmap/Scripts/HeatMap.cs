using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 作者：张骥
 * 时间：2021-06-23
 * 作用：热力图
 */

public class HeatMap : MonoBehaviour
{
    //热力图材质球（只显示热力图）
    public Material heatMat;

    #region Interface
    /// <summary>
    /// 绘制热力图
    /// </summary>
    /// <param name="dataDic">传入数据，key为场景内数据点位置，value为强度</param>
    public void DrawHeat(Dictionary<Vector3, float> dataDic)
    {
        float[] shaderData = new float[dataDic.Count * 3];
        List<Vector2> shaderPos = new List<Vector2>();
        List<float> intensity = new List<float>();

        foreach (var item in dataDic)
        {
            shaderPos.Add(ChangeScreenPos(item.Key));
            intensity.Add(item.Value);
        }

        float max = GetMax(intensity);

        for (int i = 0; i < dataDic.Count; i++)
        {
            shaderData[i * 3] = shaderPos[i].x;
            shaderData[i * 3 + 1] = shaderPos[i].y;
            shaderData[i * 3 + 2] = ChangeIntensity(intensity[i], max);
        }

        heatMat.SetInt("_HitCount", dataDic.Count);
        heatMat.SetFloatArray("_Hits", shaderData);
    }

    /// <summary>
    /// 清除绘制数据
    /// </summary>
    public void ClearDraw()
    {
        heatMat.SetInt("_HitCount", 0);
    }
    #endregion

    #region 
    /// <summary>
    /// 场景坐标转换uv坐标
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>F
    private Vector2 ChangeScreenPos(Vector3 worldPos)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
        return new Vector2((2 * 3.8f * worldPos.x - 1920 * 3.8f) / 3840, (2 * 3.8f * worldPos.y - 1080 * 3.8f) / 2160);
    }

    /// <summary>
    /// 获取强度最大值
    /// </summary>
    /// <param name="dataList"></param>
    /// <returns></returns>
    private float GetMax(List<float> dataList)
    {
        float maxValue = 0;
        for (int i = 0; i < dataList.Count; i++)
        {
            if (maxValue < dataList[i])
            {
                maxValue = dataList[i];
            }
        }
        return maxValue;
    }

    /// <summary>
    /// 强度转换
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="dataMax"></param>
    /// <returns></returns>
    private float ChangeIntensity(float intensity, float dataMax)
    {
        float changeInten = 1.5f * intensity / dataMax;
        return changeInten;
    }
    #endregion
}
