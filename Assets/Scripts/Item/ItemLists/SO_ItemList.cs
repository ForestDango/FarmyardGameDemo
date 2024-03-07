using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于存储所有登记ItemDetaisl链表的物品
/// </summary>
[CreateAssetMenu(fileName  = "so_Itemlist", menuName = "Scriptable Objects/Item list")]
public class SO_ItemList : ScriptableObject
{
    public List<ItemDetails> itemDetails;
}
