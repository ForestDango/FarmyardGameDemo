using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ص�Player����Ϸ�����ϣ�������������ObscuringItemFader����Ϸ����ʱ������Щ�����Fade����
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
