using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item List", menuName = "Data/Item List")]
public class ItemList_SO : ScriptableObject
{
    public List<Item> itemList;
    public Item Get_ItemInfo(string s)
    {
        Item item = itemList.Find(i => i.itemName == s || i.itemID == s);
        if (item == null) Debug.Log($"Can not find item {s} !");
        return item;
    }
}


[System.Serializable]
public class Item
{
    public string itemID;
    public string itemName;
    public string classifier;
    public Sprite itemSprite;
    [TextArea] public string decription;
}