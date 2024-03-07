
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// 自定义属性，通过重写OnGUI此方法，为属性创建自己的基于IMGUI的GUI。
/// </summary>
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))] //告诉系统自定义特性名叫ItemCodeDescriptionAttribute
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }
    /// <summary>
    /// 当改变Item的ItemCode的时候，同时也会改变对应ItemCode的ItemDescription
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


            //如果Item的ItemCode的值发生改变，则面板上的值也要发生变化

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }


        EditorGUI.EndProperty();
    }


    /// <summary>
    /// 通过AssetDatabase.LoadAssetAtPath寻找本地路径的唯一的SO_ItemList的所有登记Item的列表
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
