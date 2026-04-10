using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopInventory", menuName = "Scriptable Objects/ShopInventory")]
public class ShopInventory : ScriptableObject
{
    public string shopName;
    public List<ItemData> itemList = new List<ItemData>();
}
