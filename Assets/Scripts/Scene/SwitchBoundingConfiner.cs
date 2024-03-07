using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// ���ص�Cinemachine��Ϸ�����ϣ������л���ͬ������m_BoundingShape2D
/// </summary>
public class SwitchBoundingConfiner : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundsConfiner; //����AfterSceneLoadEvent�¼�
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundsConfiner;
    }

    /// <summary>
    /// ͨ��Tags����ȡ��ͬ����BoundsConfiner
    /// </summary>
    public void SwitchBoundsConfiner()
    {
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = polygonCollider2D;

        confiner.InvalidatePathCache();
    }
}
