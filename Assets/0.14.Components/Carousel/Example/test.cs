using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Carousel carousel;
    public Transform testTran;

    private void Start()
    {
        carousel.InitBanner(null);
    }
    [ContextMenu("Test2")]
    public void Test2()
    {
        List<int> testList = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            testList.Add(Random.Range(1, 20));
        }
        carousel.StartMove(testList);

    }
    [ContextMenu("Test1")]
    public void Test1()
    {
        List<int> testList = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            testList.Add(Random.Range(1, 20));
        }
        carousel.StartMove(testList);

    }

    [ContextMenu("SetItemIndex")]
    public void SetItemIndex()
    {
        carousel.SetItemIndex(2);
    }

    [ContextMenu("WithoutData")]
    public void NoData()
    {
        carousel.StartMove(new List<int>());
    }

    [ContextMenu("EndMove")]
    public void Clear()
    {
        carousel.EndMove();
    }

    [ContextMenu("MoveTran")]
    public void SetDifferTrans()
    {
        carousel.transform.SetParent(testTran);
    }
}

