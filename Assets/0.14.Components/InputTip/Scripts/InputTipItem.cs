using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTipItem : MonoBehaviour
{
    private Text itemText;
    private Button itemButton;
    [SerializeField]
    private Image checkMark;

    private KeyValuePair<string, string> label;
    public KeyValuePair<string, string> Label
    {
        set
        {
            if (itemText == null)
            {
                itemText = GetComponentInChildren<Text>();
            }
            itemText.text = value.Value;
            label = value;
        }
        get { return label; }
    }

    private int clickCount = 0;
    public int ClickCount
    {
        set
        {
            clickCount = value;
            MultipleInputTip inputTip = GetComponentInParent<MultipleInputTip>();
            if (value % 2 == 1)
            {
                inputTip.OnItemClick(this);
            }
            else
            {
                inputTip.DestroyButton(label);
            }
            checkMark.gameObject.SetActive(value % 2 == 1);
        }
        get
        {
            return clickCount;
        }
    }

    public void InitItem(bool isMutiple, KeyValuePair<string, string> data)
    {
        Label = data;
        // clickCount = 0;
        if (itemButton == null)
        {
            itemButton = GetComponentInChildren<Button>();
        }

        itemButton.onClick.RemoveAllListeners();
        if (isMutiple)
        {
            itemButton.onClick.AddListener(delegate { ClickCount++; });
        }
        else
        {
            SingleInputTip inputTip = GetComponentInParent<SingleInputTip>();
            itemButton.onClick.AddListener(delegate { inputTip.OnItemClick(this); });
        }
    }
}
