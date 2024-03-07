using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// 挂载同名脚本，用TextMeshProUGUI来设置上层还有下层的文本信息，
/// </summary>
public class UIInventoryTextBox : MonoBehaviour
{
    public TextMeshProUGUI textMeshTop1;
    public TextMeshProUGUI textMeshTop2;
    public TextMeshProUGUI textMeshTop3;

    public TextMeshProUGUI textMeshBottom1;
    public TextMeshProUGUI textMeshBottom2;
    public TextMeshProUGUI textMeshBottom3;

    public void SetTextboxText(string textMeshTop1, string textMeshTop2, string textMeshTop3,
        string textMeshBottom1, string textMeshBottom2, string textMeshBottom3)
    {
        this.textMeshTop1.text = textMeshTop1;
        this.textMeshTop2.text = textMeshTop2;
        this.textMeshTop3.text = textMeshTop3;

        this.textMeshBottom1.text = textMeshBottom1;
        this.textMeshBottom2.text = textMeshBottom2;
        this.textMeshBottom3.text = textMeshBottom3;
    }

   
}
