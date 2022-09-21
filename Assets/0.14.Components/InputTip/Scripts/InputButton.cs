using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    [SerializeField]
    private Text labelTxt;
    [SerializeField]
    private Button clearBtn;
    private KeyValuePair<string, string> data;

    public void InitInputButton(MultipleInputTip multipleInput)
    {
        // MultipleInputTip multipleInput = GetComponentInParent<MultipleInputTip>();
        clearBtn.onClick.RemoveAllListeners();
        // Debug.Log(multipleInput.GetInputTipItem(value.Key).isOn);
        clearBtn.onClick.AddListener(delegate
        {
            multipleInput.GetInputTipItem(data.Key).isOn = false;
            multipleInput.DestroyButton(this);
        });
    }

    public void SetLabel(KeyValuePair<string, string> value)
    {
        data = value;
        labelTxt.text = value.Value;
    }

    public KeyValuePair<string, string> GetData()
    {
        return data;
    }
}
