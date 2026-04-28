using Cysharp.Threading.Tasks;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [Header("どの敵のドロップか")]
    [SerializeField] private string mySymbolId;

    [Header("中身のアイテム")]
    [SerializeField] private ItemData contentItem;

    [Header("表示設定")]
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isOpened = false;

    //IDと状態のリセットを行う処理
    public void Initialize(string id,ItemData item,bool alreadyOpened)
    {
        mySymbolId = id;
        contentItem = item;
        isOpened = alreadyOpened;

        //開封済みの宝箱の見た目にする
        if (isOpened && spriteRenderer != null && openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }
    }

    //プレイヤーから呼ばれるアクション
    public async UniTask Intaract()
    {
        if (isOpened || contentItem == null)
        {
            await GlobalUIManager.instance.ShowMessage("この宝箱はすでに開けている");
            return;
        }
        else
        {
            isOpened = true;
            //インベントリに追加
            InventoryManager.instance.AddItem(contentItem, 1);

            //開封をSymbolEncountManagerに知らせる
            SymbolEncounterManager.instance.RegisterOpenedChest(mySymbolId);
            //見た目の変化
            if (spriteRenderer != null && openedSprite != null)
            {
                spriteRenderer.sprite = openedSprite;
            }

            //メッセージ表示
            await GlobalUIManager.instance.ShowMessage($"{contentItem.itemName}を入手した!");
        }
    }

    //アイテムを設定留守
    public void SetContent(ItemData item)
    {
        contentItem = item;
    }

}
