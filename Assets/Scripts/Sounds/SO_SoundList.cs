using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "Scriptable Objects/Sounds/Sound List")]
public class SO_SoundList : ScriptableObject
{
    [SerializeField] public List<SoundItem> soundDetails;
}
