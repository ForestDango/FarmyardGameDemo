using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// 挂载到Cinemachine游戏对象上，用来切换不同场景的m_BoundingShape2D
/// </summary>
public class SwitchBoundingConfiner : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundsConfiner; //订阅AfterSceneLoadEvent事件
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundsConfiner;
    }

    /// <summary>
    /// 通过Tags来获取不同场景BoundsConfiner
    /// </summary>
    public void SwitchBoundsConfiner()
    {
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = polygonCollider2D;

        confiner.InvalidatePathCache();
    }
}
