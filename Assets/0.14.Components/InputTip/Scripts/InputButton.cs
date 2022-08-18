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

    public void InitInputButton(KeyValuePair<string, string> value)
    {
        data = value;
        labelTxt.text = value.Value;
        MultipleInputTip multipleInput = GetComponentInParent<MultipleInputTip>();
        clearBtn.onClick.RemoveAllListeners();
        // Debug.Log(multipleInput.GetInputTipItem(value.Key).name);
        clearBtn.onClick.AddListener(delegate { multipleInput.GetInputTipItem(value.Key).ClickCount = 2; });
    }

    public KeyValuePair<string, string> GetData()
    {
        return data;
    }
}
