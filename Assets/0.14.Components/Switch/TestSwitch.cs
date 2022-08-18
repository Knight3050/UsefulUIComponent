using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSwitch : MonoBehaviour
{

    public Switch Switch;
    void Start()
    {
        Switch.InitSwitch(OnValueChanged, false, "开启", "关闭");
    }

    [ContextMenu("SetStatus")]
    public void ChangeSwitchStatus()
    {
        bool status = Switch.GetSwitchStatus();
        Switch.SetStatus(!status, false);
    }

    [ContextMenu("不让用")]
    public void SetSwitchInteractable()
    {
        Switch.SetInteractable(false);
    }

    private void OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("开启");
        }
        else
        {
            Debug.Log("关闭");
        }
    }
}
