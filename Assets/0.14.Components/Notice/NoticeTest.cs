using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeTest : MonoBehaviour
{
    [SerializeField]
    private InputToContent input;

    void Start()
    {
        input.Init();
    }
}
