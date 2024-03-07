using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式，限制继承该脚本的脚本必须继承自Component类
/// 继承该脚本会具有全局功能
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
