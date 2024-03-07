using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每一个需要Item特性的物品游戏对象都需要挂载此脚本
/// </summary>
public class Item : MonoBehaviour
{
    [ItemCodeDescription][SerializeField] private int _itemCode;
    private SpriteRenderer spriteRenderer;

    public int ItemCode
    {
        get
        {
            return _itemCode;
        }
        set
        {
            _itemCode = value;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    /// <summary>
    /// 初始化物品
    /// </summary>
    /// <param name="itemCodeParams"></param>
    public void Init(int itemCodeParams)
    {
        if(itemCodeParams != 0)
        {
            ItemCode = itemCodeParams;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;

            if(itemDetails.itemType == ItemType.Reapable_scenery) //如果该物品是风景类型的话，有添加能左右晃动的脚本
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
