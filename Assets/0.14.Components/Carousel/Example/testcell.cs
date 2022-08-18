using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testcell : RollCell
{
    public Text text;

    public override void UpdateData(int index, object data)
    {
        base.UpdateData(index, data);
        text.text = index.ToString();
        //text.text = index.ToString() + ";;;;;" + data.ToString();
    }
}
