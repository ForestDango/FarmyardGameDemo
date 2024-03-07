using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家挂载，负责控制拾取物品
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    /// <summary>
    /// 当玩家和Item的触发器接触的时候就添加物品
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Item>(out Item item))
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickUp)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
                AudioManager.Instance.PlaySound(SoundName.effectPickUpSound);
            }
        }
    }
}
