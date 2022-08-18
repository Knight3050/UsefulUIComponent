using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicOutline : MonoBehaviour
{
    #region Public Parameter
    [Header("边框颜色")]
    public Color setColor = new Color(13.0f / 255.0f, 1, 247.0f / 255.0f, 1);
    [Header("边框颜色强度")]
    [Range(4.0f, 20.0f)]
    public float intensity = 20.0f;
    [Header("边框宽度")]
    public float lineWidth = 5;
    [Header("边框辐射范围（相对于面片长宽百分比）")]
    public float moveRange = 0.05f;
    [Header("边框移动速度")]
    public float moveSpeed = 0.04f;
    [Header("边框移动间隔")]
    public float moveInterval = 0.1f;
    #endregion

    #region Private Parameter
    //绘制边框脚本
    private DrawOutline drawOutline;
    //协程
    private Coroutine moveCoroutine;
    //缓存顶点三方向最大值和最小值
    private Dictionary<string, Vector2> vertexDic = new Dictionary<string, Vector2>();
    #endregion

    #region Interface
    /// <summary>
    /// 开始绘制边框并进行移动
    /// </summary>
    [ContextMenu("createLine")]
    public void CreateLine()
    {
        drawOutline = GetComponent<DrawOutline>();
        if (drawOutline == null)
        {
            drawOutline = this.gameObject.AddComponent<DrawOutline>();
        }
        drawOutline.material.SetColor("_EmissiveColor", setColor * intensity);
        drawOutline.material.SetFloat("_EmissiveExposureWeight", 0.77f);
        drawOutline.AddLine();
        drawOutline.line.widthMultiplier = lineWidth;
        GetMaxMin(drawOutline.originVertex);
        float width = vertexDic["x"].y - vertexDic["x"].x;
        float height = vertexDic["y"].y - vertexDic["y"].x;
        float length = vertexDic["z"].y - vertexDic["z"].x;
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(ScaleIEnumator(width, height, length, moveSpeed));
        }
    }

    /// <summary>
    /// 停止移动、删除边框
    /// </summary>
    [ContextMenu("deleteLine")]
    public void DeleteLine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        drawOutline.DeleteLine();
    }
    #endregion

    /// <summary>
    /// 移动协程
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="length"></param>
    /// <param name="duration"></param>
    /// <param name="ignoreTimeScale"></param>
    /// <returns></returns>
    IEnumerator ScaleIEnumator(float width, float height, float length, float duration = 1)
    {
        float time = 0.0f;
        Vector3[] movePosArr = new Vector3[drawOutline.line.positionCount];
        while (true)
        {
            if (movePosArr[0].x > drawOutline.originVertex[0].x + width * moveRange || movePosArr[0].x < drawOutline.originVertex[0].x - width * moveRange)
            {
                movePosArr = drawOutline.originVertex.ToArray();
                //drawOutline.line.widthMultiplier = 0;
                time = 0;
                yield return new WaitForSeconds(moveInterval);
            }
            else
            {
                time += Time.deltaTime;
                for (int i = 0; i < drawOutline.line.positionCount; i++)
                {
                    Vector3 mPos = drawOutline.originVertex[i];
                    mPos.x = mPos.x > (vertexDic["x"].x + vertexDic["x"].y) / 2 ? mPos.x + width * time * duration : mPos.x - width * time * duration;
                    mPos.y = mPos.y > (vertexDic["y"].x + vertexDic["y"].y) / 2 ? mPos.y + height * time * duration : mPos.y - height * time * duration;
                    mPos.z = mPos.z > (vertexDic["z"].x + vertexDic["z"].y) / 2 ? mPos.z + length * time * duration : mPos.z - length * time * duration;
                    movePosArr[i] = mPos;
                }
            }
            drawOutline.line.widthMultiplier = Mathf.Sin(time * 3.14159274F / moveRange * duration) * lineWidth;
            //drawOutline.line.widthMultiplier = time * lineWidth;
            drawOutline.line.SetPositions(movePosArr);
            yield return 0;
        }
    }

    /// <summary>
    /// 获取每方向顶点最大值和最小值
    /// </summary>
    /// <param name="pointList"></param>
    private void GetMaxMin(List<Vector3> pointList)
    {
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        List<float> z = new List<float>();
        for (int i = 0; i < pointList.Count; i++)
        {
            x.Add(pointList[i].x);
            y.Add(pointList[i].y);
            z.Add(pointList[i].z);
        }

        x.Sort();
        y.Sort();
        z.Sort();

        vertexDic["x"] = new Vector2(x[0], x[pointList.Count - 1]);
        vertexDic["y"] = new Vector2(y[0], y[pointList.Count - 1]);
        vertexDic["z"] = new Vector2(z[0], z[pointList.Count - 1]);
    }
}
