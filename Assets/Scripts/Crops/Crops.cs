using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crops : MonoBehaviour
{
    private int harvestActionCount = 0;
    [HideInInspector] public Vector2Int cropGridPosition;

    [Tooltip("This should be pupulated from child gameObject")]
    [SerializeField] private SpriteRenderer cropsHarvestedSpriteRenderer;

    [Tooltip("Spawn position")]
    [SerializeField] private Transform harvestActionEffectTransform;
    public void ProcessActionTool(ItemDetails equippedItemDetails,bool isToolRight,bool isToolLeft,bool isToolDown, bool isToolUp)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropetyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
            return;

        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
            return;

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if(isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        if(cropDetails.harvsestSound != SoundName.none)
        {
            AudioManager.Instance.PlaySound(cropDetails.harvsestSound);
        }

        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }

        int requiredHarvestAction = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestAction == -1)
            return;

        harvestActionCount += 1;

        if (harvestActionCount >= requiredHarvestAction)
        {
            HarvestCrop(isToolRight,isToolUp,cropDetails, gridPropertyDetails,animator);
        }
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails,Animator animator)
    {
        if(cropDetails.isHarvestAnimation && animator != null)
        {
            if(cropDetails.harvestedSprite != null)
            {
                cropsHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
            }
        }

        if(isUsingToolRight || isUsingToolUp)
        {
            animator.SetTrigger("harvestright");
        }
        else
        {
            animator.SetTrigger("harvestleft");
        }

        //Reset
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        if (cropDetails.hideCropBeforeHarvestAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (cropDetails.disableCropColliderBeforeHarvsetAnimation)
        {
            Collider2D[] collider2DArray = GetComponentsInChildren<BoxCollider2D>();
            foreach (Collider2D collider2D  in collider2DArray)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        if (cropDetails.isHarvestAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {

            HarvestAction(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionAfterAnimation(CropDetails cropDetails,GridPropertyDetails gridPropertyDetails,Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestAction(cropDetails, gridPropertyDetails);
    }

    public void HarvestAction(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItem(cropDetails);

        if(cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestTransformCode(cropDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }

    private void CreateHarvestTransformCode(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

    private void SpawnHarvestedItem(CropDetails cropDetails)
    {
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            if(cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = UnityEngine.Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i]);
            }

            for (int x = 0; x < cropsToProduce; x++)
            {
                Vector3 spawnPosition;
                if(cropDetails.spawnCropProduceAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);

                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f), transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }
}
