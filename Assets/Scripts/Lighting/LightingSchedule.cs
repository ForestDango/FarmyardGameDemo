using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承ScriptableObject，用来存储ScriptableObject数据集合数组
/// </summary>
[CreateAssetMenu(fileName = "lightingSchedule", menuName = "Scriptable Objects/Lighting/Lighting Schedule")]
public class LightingSchedule : ScriptableObject
{
    public LightingBrtightness[] lightingBrtightnessArray;
}
