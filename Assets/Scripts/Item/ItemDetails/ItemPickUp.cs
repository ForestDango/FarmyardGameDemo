using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ҹ��أ��������ʰȡ��Ʒ
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    /// <summary>
    /// ����Һ�Item�Ĵ������Ӵ���ʱ��������Ʒ
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
