using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڴ洢���еǼ�ItemDetaisl�������Ʒ
/// </summary>
[CreateAssetMenu(fileName  = "so_Itemlist", menuName = "Scriptable Objects/Item list")]
public class SO_ItemList : ScriptableObject
{
    public List<ItemDetails> itemDetails;
}
