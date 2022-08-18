using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOutline : MonoBehaviour
{
    //lineRender材质
    public Material material;

    //lineRender进行边框绘制
    [HideInInspector]
    public LineRenderer line;

    //获取mesh顶点
    [HideInInspector]
    public List<Vector3> originVertex = new List<Vector3>();

    #region Interface
    /// <summary>
    /// 边框绘制
    /// </summary>
    public void AddLine()
    {
        if (material == null)
        {
            material = new Material(Shader.Find("HDRP/Lit"));
        }
        AddLine(GetComponent<MeshFilter>().sharedMesh, transform);
    }

    /// <summary>
    /// 边框删除
    /// </summary>
    public void DeleteLine()
    {
        DeleteLine(transform);
    }
    #endregion

    /// <summary>
    /// 添加边缘
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="parent"></param>
    private void AddLine(Mesh mesh, Transform parent)
    {
        //DeleteLine(parent);
        Vector3[] meshVertices = mesh.vertices;
        List<Vector3> vertices = TrianglesAndVerticesEdge(mesh.vertices, mesh.triangles);
        originVertex = vertices;
        AddSingleLine(vertices.ToArray(), parent);

    }

    /// <summary>
    /// 添加单个物体边缘
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="parent"></param>
    private void AddSingleLine(Vector3[] vertices, Transform parent)
    {
        LineRenderer lineRenderer = new LineRenderer();
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).name.Contains("MeshVertexLine"))
                {
                    parent.GetChild(i).gameObject.SetActive(true);
                    lineRenderer = parent.GetChild(i).GetComponent<LineRenderer>();
                }
            }
        }
        else
        {
            lineRenderer = new GameObject("MeshVertexLine_0", new System.Type[] { typeof(LineRenderer) }).GetComponent<LineRenderer>();

            lineRenderer.transform.parent = parent;
            lineRenderer.transform.localPosition = new Vector3(0, 0.01f, 0.01f);
            lineRenderer.transform.localRotation = Quaternion.identity;
            lineRenderer.transform.localScale = Vector3.one;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.allowOcclusionWhenDynamic = false;
            lineRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.sortingLayerName = "GamePlay";
            lineRenderer.sortingOrder = 501;
            lineRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            lineRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.material = material;
        }

        lineRenderer.positionCount = vertices.Length;
        lineRenderer.SetPositions(vertices);

        line = lineRenderer;
    }

    /// <summary>
    /// 删除边缘
    /// </summary>
    /// <param name="parent"></param>
    private void DeleteLine(Transform parent)
    {
        if (parent.childCount == 0)
            return;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name.Contains("MeshVertexLine"))
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 网格系统边缘查找
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    /// <returns></returns>
    private List<Vector3> TrianglesAndVerticesEdge(Vector3[] vertices, int[] triangles)
    {
        List<Vector2Int> edgeLines = TrianglesEdgeAnalysis(triangles);
        List<Vector3> result = SpliteLines(edgeLines, vertices);
        return result;
    }

    /// <summary>
    /// 三角面组边缘提取
    /// </summary>
    /// <param name="triangles"></param>
    /// <returns></returns>
    private List<Vector2Int> TrianglesEdgeAnalysis(int[] triangles)
    {
        int[,] edges = new int[triangles.Length, 2];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    int index = (j + k) % 3;
                    edges[i + j, k] = triangles[i + index];
                }
            }
        }

        bool[] invalidFlag = new bool[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            for (int j = i + 1; j < triangles.Length; j++)
            {
                if ((edges[i, 0] == edges[j, 0] && edges[i, 1] == edges[j, 1]) || (edges[i, 0] == edges[j, 1] && edges[i, 1] == edges[j, 0]))
                {
                    invalidFlag[i] = true;
                    invalidFlag[j] = true;
                }
            }
        }

        List<Vector2Int> edgeLines = new List<Vector2Int>();
        for (int i = 0; i < triangles.Length; i++)
        {
            if (!invalidFlag[i])
            {
                edgeLines.Add(new Vector2Int(edges[i, 0], edges[i, 1]));
            }
        }

        if (edgeLines.Count == 0)
        {
            Debug.LogError("Calculate wrong, there is not an valid line.");
        }
        return edgeLines;
    }

    /// <summary>
    /// 边缘排序与分离
    /// </summary>
    /// <param name="edgeLines"></param>
    /// <param name="vertices"></param>
    /// <returns></returns>
    private List<Vector3> SpliteLines(List<Vector2Int> edgeLines, Vector3[] vertices)
    {
        List<Vector3> result = new List<Vector3>();

        List<int> edgeIndex = new List<int>();
        int startIndex = edgeLines[0].x;
        edgeIndex.Add(edgeLines[0].x);
        int removeIndex = 0;
        int currentIndex = edgeLines[0].y;

        while (true)
        {
            edgeLines.RemoveAt(removeIndex);
            edgeIndex.Add(currentIndex);

            bool findNew = false;
            for (int i = 0; i < edgeLines.Count && !findNew; i++)
            {
                if (currentIndex == edgeLines[i].x)
                {
                    currentIndex = edgeLines[i].y;
                    removeIndex = i;
                    findNew = true;
                }
                else if (currentIndex == edgeLines[i].y)
                {
                    currentIndex = edgeLines[i].x;
                    removeIndex = i;
                    findNew = true;
                }
            }

            if (findNew && currentIndex == startIndex)
            {
                edgeLines.RemoveAt(removeIndex);
                List<Vector3> singleVertices = new List<Vector3>();
                for (int i = 0; i < edgeIndex.Count; i++)
                {
                    singleVertices.Add(vertices[edgeIndex[i]]);
                }
                result = singleVertices;

                if (edgeLines.Count > 0)
                {
                    edgeIndex = new List<int>();
                    startIndex = edgeLines[0].x;
                    edgeIndex.Add(edgeLines[0].x);
                    removeIndex = 0;
                    currentIndex = edgeLines[0].y;
                }
                else
                {
                    break;
                }
            }
            else if (!findNew)
            {
                Debug.LogError("Complete curve, but not closed.");
                List<Vector3> singleVertices = new List<Vector3>();
                for (int i = 0; i < edgeIndex.Count; i++)
                {
                    singleVertices.Add(vertices[edgeIndex[i]]);
                }
                result = singleVertices;

                if (edgeLines.Count > 0)
                {
                    edgeIndex = new List<int>();
                    startIndex = edgeLines[0].x;
                    edgeIndex.Add(edgeLines[0].x);
                    removeIndex = 0;
                    currentIndex = edgeLines[0].y;
                }
                else
                {
                    break;
                }
            }
        }
        return result;
    }
}
