using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̳�ScriptableObject�������洢ScriptableObject���ݼ�������
/// </summary>
[CreateAssetMenu(fileName = "lightingSchedule", menuName = "Scriptable Objects/Lighting/Lighting Schedule")]
public class LightingSchedule : ScriptableObject
{
    public LightingBrtightness[] lightingBrtightnessArray;
}
