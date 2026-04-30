using TMPro;
using UnityEngine;

public class StatusWindowManager : MonoBehaviour
{
    [Header("メニュー全体")]
    [SerializeField] private GameObject statusMenu;

    [Header("プレイヤーステータス")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text speedText;

    [Header("装備")]
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private TMP_Text armorText;
    [SerializeField] private TMP_Text accessoryText;

    [Header("所持金")]
    [SerializeField] private TMP_Text moneyText;

    //[Header("ヒロイン")]
   
    //ステータスウィンドウを開く処理
    public void OpenStatus(PlayerData data)
    {
        statusMenu.SetActive(true);
        SetData(data);
    }

    //ステータスウィンドウを閉じる処理
    public void CloseStatus()
    {
        statusMenu.SetActive(false);
    }

    //各種データを反映する処理
    public void SetData(PlayerData data)
    {
        int currentHP = GameManager.instance.currentHp;
        int maxHP = data.maxHP;

        nameText.text = data.playerName;
        levelText.text = $"Lv：{data.level}";
        hpText.text = $"HP：{currentHP}/{maxHP}";
        attackText.text = $"攻撃：{data.TotalAttack}";
        defenseText.text = $"防御：{data.TotalDefense}";
        speedText.text = $"素早さ：{data.TotalSpeed}";

        weaponText.text = data.equippedWeapon != null ? data.equippedWeapon.itemName : "なし";
        armorText.text = data.equippedArmor != null ? data.equippedArmor.itemName : "なし";
        accessoryText.text = data.equippedAccessory != null ? data.equippedAccessory.itemName : "なし";

        moneyText.text = $"{GameManager.instance.gold.ToString()}G";
    }
}
