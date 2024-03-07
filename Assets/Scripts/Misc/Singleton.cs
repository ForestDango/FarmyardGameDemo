using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ģʽ�����Ƽ̳иýű��Ľű�����̳���Component��
/// �̳иýű������ȫ�ֹ���
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
