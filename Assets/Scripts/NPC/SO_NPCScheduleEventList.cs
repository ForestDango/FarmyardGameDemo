using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleEventList", menuName = "Scriptable Objects/NPC/Schedule Event List")]
public class SO_NPCScheduleEventList : ScriptableObject
{
    [SerializeField] public List<NPCScheduleEvent> npcScheduleEventList;
}
