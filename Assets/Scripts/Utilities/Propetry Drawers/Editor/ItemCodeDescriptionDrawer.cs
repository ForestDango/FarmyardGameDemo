
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// �Զ������ԣ�ͨ����дOnGUI�˷�����Ϊ���Դ����Լ��Ļ���IMGUI��GUI��
/// </summary>
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))] //����ϵͳ�Զ�����������ItemCodeDescriptionAttribute
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }
    /// <summary>
    /// ���ı�Item��ItemCode��ʱ��ͬʱҲ��ı��ӦItemCode��ItemDescription
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if(property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();

            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2),label,property.intValue);

            EditorGUI.LabelField(new Rect(position.x, position.y + position.height /2 , position.width, position.height / 2), "Item Description",
                GetItemDescription(property.intValue));


            //���Item��ItemCode��ֵ�����ı䣬������ϵ�ֵҲҪ�����仯

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }


        EditorGUI.EndProperty();
    }


    /// <summary>
    /// ͨ��AssetDatabase.LoadAssetAtPathѰ�ұ���·����Ψһ��SO_ItemList�����еǼ�Item���б�
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_itemList;

        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Items/so_Itemlist.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailLists = so_itemList.itemDetails;

        ItemDetails itemDetail = itemDetailLists.Find(x => x.itemCode == itemCode);

        if(itemDetail!= null)
        {
            return itemDetail.itemDescription;
        }
        else
        {
            return "";
        }
    }
}
