using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eggs : MonoBehaviour
{
    private int harvestActionCount = 0;
    [HideInInspector] public Vector2Int eggGridPosition;

    [Tooltip("This should be pupulated from child gameObject")]
    [SerializeField] private SpriteRenderer eggsHarvestedSpriteRenderer;

    [Tooltip("Spawn position")]
    [SerializeField] private Transform harvestActionEffectTransform;

    public void ProcessActionTool(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropetyDetails(eggGridPosition.x, eggGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        ItemDetails eggItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.eggItemCode);
        if (eggItemDetails == null)
            return;

        EggDetails eggDetails = GridPropertiesManager.Instance.GetEggDetails(eggItemDetails.itemCode);
        if (eggDetails == null)
            return;

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        if (eggDetails.harvsestSound != SoundName.none)
        {
            AudioManager.Instance.PlaySound(eggDetails.harvsestSound);
        }

        if (eggDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEvent(harvestActionEffectTransform.position, eggDetails.harvestActionEffect);
        }

        int requiredHarvestAction = eggDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestAction == -1)
            return;

        harvestActionCount += 1;

        if (harvestActionCount >= requiredHarvestAction)
        {
            HarvestEgg(isToolRight, isToolUp, eggDetails, gridPropertyDetails, animator);
        }
    }

    private void HarvestEgg(bool isUsingToolRight, bool isUsingToolUp, EggDetails eggDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        if (eggDetails.isHarvestAnimation && animator != null)
        {
            if (eggDetails.harvestedSprite != null)
            {
                eggsHarvestedSpriteRenderer.sprite = eggDetails.harvestedSprite;
            }
        }

        if (isUsingToolRight || isUsingToolUp)
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

        if (eggDetails.hideEggBeforeHarvestAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (eggDetails.disableEggColliderBeforeHarvsetAnimation)
        {
            Collider2D[] collider2DArray = GetComponentsInChildren<BoxCollider2D>();
            foreach (Collider2D collider2D in collider2DArray)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        if (eggDetails.isHarvestAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionAfterAnimation(eggDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestAction(eggDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionAfterAnimation(EggDetails eggDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestAction(eggDetails, gridPropertyDetails);
    }

    public void HarvestAction(EggDetails eggDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItem(eggDetails);

        if (eggDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestTransformCode(eggDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }

    private void SpawnHarvestedItem(EggDetails eggDetails)
    {
        for (int i = 0; i < eggDetails.eggsProducedItemCode.Length; i++)
        {
            int eggsToProduce;

            if (eggDetails.eggsProducedMinQuantity[i] == eggDetails.eggsProducedMaxQuantity[i] ||
                eggDetails.eggsProducedMaxQuantity[i] < eggDetails.eggsProducedMinQuantity[i])
            {
                eggsToProduce = eggDetails.eggsProducedMinQuantity[i];
            }
            else
            {
                eggsToProduce = UnityEngine.Random.Range(eggDetails.eggsProducedMinQuantity[i], eggDetails.eggsProducedMaxQuantity[i]);
            }

            for (int x = 0; x < eggsToProduce; x++)
            {
                Vector3 spawnPosition;
                if (eggDetails.spawnEggProduceAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, eggDetails.eggsProducedItemCode[i]);

                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f), transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(eggDetails.eggsProducedItemCode[i], spawnPosition);
                }
            }
        }
    }

    private void CreateHarvestTransformCode(EggDetails eggDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = eggDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceHatched = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}
