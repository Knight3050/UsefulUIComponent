using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTipItem : MonoBehaviour
{
    private Text itemText;
    private Button itemButton;
    [SerializeField]
    private Image checkMark;
    private Color selectColor;
    private Color unSelectColor;

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
            // itemText.color = unSelectColor;
            label = value;
        }
        get { return label; }
    }

    private bool ison = false;
    public bool isOn
    {
        set
        {
            ison = value;
            checkMark.gameObject.SetActive(value);
            itemText.color = (value ? selectColor : unSelectColor);
        }
        get
        {
            return ison;
        }
    }

    public void InitItem(InputTip inputTip, bool isMutiple, Color selectColor, Color unSelectColor)
    {
        this.selectColor = selectColor;
        this.unSelectColor = unSelectColor;

        if (itemButton == null)
        {
            itemButton = GetComponentInChildren<Button>();
        }

        // itemButton.onClick.RemoveAllListeners();
        if (isMutiple)
        {
            MultipleInputTip multiInputTip = (MultipleInputTip)inputTip;
            itemButton.onClick.AddListener(delegate
            {
                isOn = !isOn;
                if (isOn)
                {
                    multiInputTip.OnItemClick(this);
                }
                else
                {
                    multiInputTip.DestroyButton(label);
                }
            });
        }
        else
        {
            SingleInputTip singleInputTip = (SingleInputTip)inputTip;
            // SingleInputTip inputTip = GetComponentInParent<SingleInputTip>();
            itemButton.onClick.AddListener(delegate
            {
                // Debug.Log(inputTip.name);
                // isOn = !isOn;
                singleInputTip.OnItemClick(this);
            });
        }
    }

    public void SetLabel(KeyValuePair<string, string> data)
    {
        Label = data;
    }
}
