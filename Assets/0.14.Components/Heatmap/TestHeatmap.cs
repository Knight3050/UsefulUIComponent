using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHeatmap : MonoBehaviour
{
    public HeatMap heatMap;
    // Start is called before the first frame update
    void Start()
    {
        DrawMap();
    }

    [ContextMenu("ClearData")]
    public void ClearData()
    {
        heatMap.ClearDraw();
    }

    [ContextMenu("DrawMap")]
    public void DrawMap()
    {
        int index = Random.Range(0, 1000);
        Dictionary<Vector3, float> test = new Dictionary<Vector3, float>();
        for (int i = 0; i < index; i++)
        {
            Vector3 pos = new Vector3(Random.Range(0, 1920), Random.Range(0, 1080), 0);
            test[pos] = Random.Range(0f, 1.5f);
        }
        heatMap.DrawHeat(test);
    }
}
