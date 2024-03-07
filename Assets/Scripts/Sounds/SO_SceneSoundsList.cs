using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneSoundList", menuName = "Scriptable Objects/Sounds/Scene Sound List")]

public class SO_SceneSoundsList : ScriptableObject
{
    [SerializeField] public List<SceneSoundItem> sceneSoundDetails;
}
