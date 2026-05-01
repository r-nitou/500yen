using UnityEngine;
using Utage;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string saveFilePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //保存先指定
            saveFilePath = Path.Combine(Application.persistentDataPath, "savedata.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //セーブの実行
    [ContextMenu("Save Game")]
    public void Save()
    {
        //インスタンス生成
        SaveData data = new SaveData();
        var gameManager = GameManager.instance;
        var player = gameManager.playerData;
        var sister = player.sisterParameter;

        //データ詰込み(プレイヤー)
        //基本情報
        data.playerName = player.playerName;
        data.level = player.level;
        data.currentHP = gameManager.currentHp;
        data.currentExp = player.currentExp;
        data.nextLevelExp = player.nextLevelExp;
        data.gold = gameManager.gold;
        //ステータス
        data.maxHP = player.maxHP;
        data.attack = player.attack;
        data.defense = player.defense;
        data.speed = player.speed;
        //装備
        data.weaponName = player.equippedWeapon != null ? player.equippedWeapon.itemName : "";
        data.armorName = player.equippedArmor != null ? player.equippedArmor.itemName : "";
        data.accessoryName = player.equippedAccessory != null ? player.equippedAccessory.itemName : "";

        //ヒロイン
        if (sister != null)
        {
            data.affection = sister.affection;
            data.heroineAttack = sister.attack;
            data.heroineDefense = sister.defense;
            data.heroineSpeed = sister.speed;
        }

        //フラグ
        data.currentPhase = gameManager.currentPhase;

        data.isNewGame = gameManager.isNewGame;


        data.hasShownBulletinTutorial = gameManager.hasShownBulletinTutorial;
        data.hasShownFastTravelTutorial = gameManager.hasShownBulletinTutorial;
        data.islastBossDefeated = gameManager.IslastBossDefated;
        data.visitedFloorData = new List<FloorData>(gameManager.visitedFloorData);

        //撃破情報
        if (gameManager.symbolEncounterManager != null)
        {
            data.defeatedSymbolIds = new List<string>(gameManager.symbolEncounterManager.defeatedSymbolId);
        }

        //インベントリ
        foreach(var slot in gameManager.inventoryManager.PlayerInventory)
        {
            data.inventory.Add(new ItemSaveSlot { itemName = slot.item.itemName, count = slot.count });
        }

        //位置
        data.currentSceneName = SceneManager.GetActiveScene().name;
        if (PlayerMove.instance != null)
        {
            Vector3 playerPos = PlayerMove.instance.transform.position;
            data.posX = playerPos.x;
            data.posY = playerPos.y;
            data.posZ = playerPos.z;
        }

        //現在の目標
        if (ObjectiveUIManager.instance != null)
        {
            data.currentObjective = ObjectiveUIManager.instance.currentObjectiveString;
        }

        //保存
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"<color=green>Save Complete:{saveFilePath}</color>");
    }

    [ContextMenu("Load Game")]
    public void Load()
    {
        //フォルダの有無を確認
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        var gameManager = GameManager.instance;
        var playerData = gameManager.playerData;
        var sisterData = playerData.sisterParameter;

        //値の復元
        //プレイヤー
        playerData.playerName = data.playerName;
        playerData.level = data.level;
        gameManager.currentHp = data.currentHP;
        playerData.currentExp = data.currentExp;
        playerData.nextLevelExp = data.nextLevelExp;
        gameManager.gold = data.gold;

        playerData.maxHP = data.maxHP;
        playerData.attack = data.attack;
        playerData.defense = data.defense;
        playerData.speed = data.speed;

        //ヒロイン
        if (sisterData != null)
        {
            sisterData.affection = data.affection;
            sisterData.attack = data.heroineAttack;
            sisterData.defense = data.heroineDefense;
            sisterData.speed = data.heroineSpeed;
        }

        //フラグ
        gameManager.currentPhase = data.currentPhase;
        gameManager.isNewGame = data.isNewGame;
        gameManager.hasShownBulletinTutorial = data.hasShownBulletinTutorial;
        gameManager.hasShowFastTravelTutorial = data.hasShownFastTravelTutorial;
        gameManager.IslastBossDefated = data.islastBossDefeated;
        gameManager.visitedFloorData = new List<FloorData>(data.visitedFloorData);

        //撃破情報
        if (gameManager.symbolEncounterManager != null)
        {
            gameManager.symbolEncounterManager.defeatedSymbolId = new List<string>(data.defeatedSymbolIds);
        }

        //インベントリ・装備
        RestoreInventoryAndEquipment(data);

        //シーン遷移・位置復元
        RestorePosition(data).Forget();
    }

    //インベントリ・装備の復元処理
    private void RestoreInventoryAndEquipment(SaveData data)
    {
        var imventory = GameManager.instance.inventoryManager;
        var playerData = GameManager.instance.playerData;

        //ItemDataの取得
        ItemData[] allItems = Resources.LoadAll<ItemData>("");

        //名前で検索できるように辞書化
        Dictionary<string, ItemData> itemDataBase = new Dictionary<string, ItemData>();
        foreach(var item in allItems)
        {
            if (itemDataBase.ContainsKey(item.itemName)) 
            {
                itemDataBase.Add(item.itemName, item);
            }
        }

        //インベントリーの復元
        imventory.PlayerInventory.Clear();
        foreach(var slot in data.inventory)
        {
            if(itemDataBase.TryGetValue(slot.itemName,out ItemData foundItem))
            {
                imventory.AddItem(foundItem, slot.count);
            }
        }

        //装備の復元
        playerData.equippedWeapon = itemDataBase.ContainsKey(data.weaponName) ? itemDataBase[data.weaponName] : null;
        playerData.equippedArmor = itemDataBase.ContainsKey(data.armorName) ? itemDataBase[data.armorName] : null;
        playerData.equippedAccessory = itemDataBase.ContainsKey(data.accessoryName) ? itemDataBase[data.accessoryName] : null;
    }

    private async Cysharp.Threading.Tasks.UniTaskVoid RestorePosition(SaveData data)
    {
        await SceneLoader.instance.LoadSceneForReload(data.currentSceneName);

        //ロード完了後位置をセット
        if (PlayerMove.instance != null)
        {
            PlayerMove.instance.gameObject.SetActive(true);
            PlayerMove.instance.transform.position = new Vector3(data.posX, data.posY, data.posZ);

            //カメラのセット
            var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Follow = PlayerMove.instance.transform;
                vcam.ForceCameraPosition(PlayerMove.instance.transform.position, Quaternion.identity);
            }

            //操作を有効化
            PlayerMove.instance.InputAction.Player.Enable();

            // 仲間の位置も同期させる
            if (FollowerMove.instance != null)
            {
                FollowerMove.instance.gameObject.SetActive(true);
                FollowerMove.instance.transform.position = PlayerMove.instance.transform.position;
            }
        }

        //目的地UIの復元
        if (ObjectiveUIManager.instance != null && !string.IsNullOrEmpty(data.currentObjective)) 
        {
            ObjectiveUIManager.instance.SetObjective(data.currentObjective).Forget();
        }
    }

    [ContextMenu("Deleate Game")]
    //セーブデータの削除
    public void DeleateSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("セーブデータを破棄しました");
        }
    }
}
