using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ÿһ����ҪItem���Ե���Ʒ��Ϸ������Ҫ���ش˽ű�
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
    /// ��ʼ����Ʒ
    /// </summary>
    /// <param name="itemCodeParams"></param>
    public void Init(int itemCodeParams)
    {
        if(itemCodeParams != 0)
        {
            ItemCode = itemCodeParams;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;

            if(itemDetails.itemType == ItemType.Reapable_scenery) //�������Ʒ�Ƿ羰���͵Ļ�������������һζ��Ľű�
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
