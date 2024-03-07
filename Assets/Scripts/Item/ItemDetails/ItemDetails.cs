using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode; //物品编号
    public ItemType itemType; //枚举类型，物品类型
    public string itemDescription; //物品描述
    public Sprite itemSprite; //物品精灵图
    public string itemLongDescription; //物品的长描述
    public short itemUseGridRadius; //物品在多少个Grid半径能使用
    public float itemUseRadius; //物品能使用的半径
    public bool isStartingItem; //是否是起始物品
    public bool canBePickUp; //可以被捡起来
    public bool canBeDropped;//可以被扔掉
    public bool canBeEaten; //可以被吃
    public bool canBeCarried; //可以被承载
}
