using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode; //��Ʒ���
    public ItemType itemType; //ö�����ͣ���Ʒ����
    public string itemDescription; //��Ʒ����
    public Sprite itemSprite; //��Ʒ����ͼ
    public string itemLongDescription; //��Ʒ�ĳ�����
    public short itemUseGridRadius; //��Ʒ�ڶ��ٸ�Grid�뾶��ʹ��
    public float itemUseRadius; //��Ʒ��ʹ�õİ뾶
    public bool isStartingItem; //�Ƿ�����ʼ��Ʒ
    public bool canBePickUp; //���Ա�������
    public bool canBeDropped;//���Ա��ӵ�
    public bool canBeEaten; //���Ա���
    public bool canBeCarried; //���Ա�����
}
