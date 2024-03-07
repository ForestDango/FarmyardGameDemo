using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PauseMenuInventoryManagerSlot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private float textBoxDistance = 50f;
    public Image inventoryManagementSlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject greyedOutImageGO;
    [SerializeField] private PauseMenuInventoryManagement inventoryManagement;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

#pragma warning disable 414
    private Vector3 startingPosition;
#pragma warning restore 414
    public GameObject draggedItem;
    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = FindObjectOfType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemQuantity != 0)
        {
            draggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab, inventoryManagement.transform);

            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventoryManagementSlotImage.sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       if(draggedItem != null)
        {
            Destroy(draggedItem);

            if(eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagerSlot>() != null)
            {
                int toSlotNum = eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagerSlot>().slotNumber;

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNum);

                inventoryManagement.DestoryInventoryTextboxGameObject();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            inventoryManagement.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryManagement.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryManagement.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            string itemTypeDescrption = InventoryManager.Instance.GetItemTypeDesciption(itemDetails.itemType);

            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescrption, "", itemDetails.itemDescription, "", "");

            if (slotNumber > 23)
            {
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + textBoxDistance, transform.position.z);
            }
            else
            {
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - textBoxDistance, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManagement.DestoryInventoryTextboxGameObject();
    }
}
