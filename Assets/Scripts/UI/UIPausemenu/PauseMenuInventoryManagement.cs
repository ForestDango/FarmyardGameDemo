using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField] private PauseMenuInventoryManagerSlot[] inventoryManagementSlots;
    public GameObject inventoryManagementDraggedItemPrefab;
    [SerializeField] private Sprite transparent16 = null;
    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += PopulatePlayerInventory;

        if(PlayerController.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player]);

        }
    }
    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= PopulatePlayerInventory;

        DestoryInventoryTextboxGameObject();


    }

    public void DestoryInventoryTextboxGameObject()
    {
        if(inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);
        }
    }

    public void DestroyCurrentlyDraggedItem()
    {
        for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
        {
            if(inventoryManagementSlots[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlots[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playerInventoryLists)
    {
       if(inventoryLocation == InventoryLocation.player)
        {
            IntializeInventoryManagementSlots();

            for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
            {
                inventoryManagementSlots[i].itemDetails = InventoryManager.Instance.GetItemDetails(playerInventoryLists[i].itemCode);
                inventoryManagementSlots[i].itemQuantity = playerInventoryLists[i].itemQuantity;

                if(inventoryManagementSlots[i] .itemDetails != null)
                {
                    inventoryManagementSlots[i].inventoryManagementSlotImage.sprite = inventoryManagementSlots[i].itemDetails.itemSprite;
                    inventoryManagementSlots[i].textMeshProUGUI.text = inventoryManagementSlots[i].itemQuantity.ToString();
                }

            }
        }
    }

    private void IntializeInventoryManagementSlots()
    {
        for (int i = 0; i < Settings.playerMaximumInventoryCapcity; i++)
        {
            inventoryManagementSlots[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlots[i].itemDetails = null;
            inventoryManagementSlots[i].itemQuantity = 0;
            inventoryManagementSlots[i].inventoryManagementSlotImage.sprite = transparent16;
            inventoryManagementSlots[i].textMeshProUGUI.text = "";
        }

        for (int i = InventoryManager.Instance.inventoryListInCapcityInArray[(int)InventoryLocation.player]; i < Settings.playerMaximumInventoryCapcity; i++)
        {
            inventoryManagementSlots[i].greyedOutImageGO.SetActive(true);
        }
    }
}
