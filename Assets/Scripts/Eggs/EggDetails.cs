using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EggDetails 
{
    [ItemCodeDescription]
    public int eggItemCode;
    public int[] growthDays;
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite; //������ͼƬ

    [ItemCodeDescription]
    public int harvestedTransformItemCode;
    public bool hideEggBeforeHarvestAnimation;
    public bool disableEggColliderBeforeHarvsetAnimation;

    public bool isHarvestAnimation;
    public bool isHarvestActionEffect = false;
    public bool spawnEggProduceAtPlayerPosition;
    public HarvestEffect harvestActionEffect;
    public SoundName harvsestSound;

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // �ջ�Ĺ��ߵ�Item���
    public int[] requiredHarvestActions; //numbers of harvest actions required for corressponding tool in harvset tool item code array����Ҫ���ջ���Ϊ������harvestToolItemCode��Ӧ

    [ItemCodeDescription]
    public int[] eggsProducedItemCode;
    public int[] eggsProducedMinQuantity;
    public int[] eggsProducedMaxQuantity;
    public int daysToRegrow;

    public bool CanUseToolToHarvsetEggs(int toolItemCode)
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
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
