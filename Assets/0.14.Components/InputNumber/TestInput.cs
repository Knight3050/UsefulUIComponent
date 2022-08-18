using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    public InputNumber inputNumber;
    void Start()
    {
        inputNumber.InitInput(test);
    }

    private void test(float value)
    {
        Debug.Log(value);
    }

    [ContextMenu("SetValue")]
    public void SetValue()
    {
        inputNumber.SetValue(Random.Range(1, 30));
    }
}
