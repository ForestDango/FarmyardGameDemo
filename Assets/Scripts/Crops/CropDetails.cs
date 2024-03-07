using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;
    public int[] growthDays;
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite;


    [ItemCodeDescription]
    public int harvestedTransformItemCode;
    public bool hideCropBeforeHarvestAnimation;
    public bool disableCropColliderBeforeHarvsetAnimation;

    public bool isHarvestAnimation;
    public bool isHarvestActionEffect = false;
    public bool spawnCropProduceAtPlayerPosition;
    public HarvestEffect harvestActionEffect;
    public SoundName harvsestSound;

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // 收获的工具的Item编号
    public int[] requiredHarvestActions; //numbers of harvest actions required for corressponding tool in harvset tool item code array所需要的收获行为动作和harvestToolItemCode对应

    [ItemCodeDescription]
    public int[] cropProducedItemCode;
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;


    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;

        }
        else
        {
            return true;
        }
    }

    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if(harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
