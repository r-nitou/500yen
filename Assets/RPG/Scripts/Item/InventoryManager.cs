using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("所持品リスト")]
    [SerializeField] private List<ItemSlot> playerInventory = new List<ItemSlot>();

    //外部参照用プロパティ
    public List<ItemSlot> PlayerInventory => playerInventory;

    //アイテムをインベントリに追加する処理
    public void AddItem(ItemData item, int amount)
    {
        //アイテムがすでにインベントリに存在するか確認
        ItemSlot slot = playerInventory.Find(s => s.item == item);

        if (slot != null)
        {
            //アイテムが存在する場合、数量を増やす
            slot.count += amount;
        }
        else
        {
            //アイテムが存在しない場合、新しいスロットを作成して追加
            playerInventory.Add(new ItemSlot(item, amount));
        }
    }

    //アイテムを消費する処理
    public bool RemoveItem(ItemData item,int amount)
    {
        ItemSlot slot = playerInventory.Find(s => s.item == item);

        if (slot != null && slot.count >= amount) 
        {
            slot.count -= amount;
            //所持数が0個以下になったらインベントリから削除
            if (slot.count <= 0)
            {
                playerInventory.Remove(slot);
            }
            return true;
        }
        return false;
    }

    //アイテムを所持しているか確認する処理
    public int GetItemCount(ItemData item)
    {
        ItemSlot slot = playerInventory.Find(s => s.item == item);
        return slot != null ? slot.count : 0;
    }
}
