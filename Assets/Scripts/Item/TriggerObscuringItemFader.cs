using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载到Player的游戏对象上，当经过挂载了ObscuringItemFader的游戏对象时触发这些对象的Fade方法
/// </summary>
public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObscuringItemFader[] obscuringFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if(obscuringFaders.Length > 0)
        {
            for (int i = 0; i < obscuringFaders.Length; i++)
            {
                obscuringFaders[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ObscuringItemFader[] obscuringFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringFaders.Length > 0)
        {
            for (int i = 0; i < obscuringFaders.Length; i++)
            {
                obscuringFaders[i].FadeIn();
            }
        }
    }
}
