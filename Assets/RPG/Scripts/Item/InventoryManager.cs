using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public const int NOMAL_USE_ITEM_AMOUNT = 1;
    public static InventoryManager instance { get; set; }

    [Header("所持品リスト")]
    [SerializeField] private List<ItemSlot> playerInventory = new List<ItemSlot>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

    //アイテムを使用する処理
    public bool UseItem(ItemData item)
    {
        //消耗品のみ使用可能
        if (item.itemtype != ItemType.Consimable) return false;

        //効果の適用
        bool wasEffectApplied = ApplyEffect(item);
        //効果が適用されたら個数を１つ減らす
        if (wasEffectApplied)
        {
            RemoveItem(item, NOMAL_USE_ITEM_AMOUNT);
            return true;
        }

        return false;
    }

    //アイテムのデータをもとにステータスを操作する処理
    private bool ApplyEffect(ItemData item)
    {
        //アイテムの効果タイプに応じて処理を変える
        switch (item.effectType)
        {
            case ItemEffectType.HPRecover:
                return StatusManager.instance.RecoverHP(item.healAmount);
            default:
                return false;
        }
    }
}
