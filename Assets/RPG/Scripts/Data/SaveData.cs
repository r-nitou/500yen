using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    //プレイヤー基本情報
    public string playerName;
    public int level;
    public int currentHP;
    public int currentExp;
    public int nextLevelExp;
    public int gold;

    //プレイヤーステータス
    public int maxHP;
    public int attack;
    public int defense;
    public int speed;

    //装備品
    public string weaponName;
    public string armorName;
    public string accessoryName;

    //ヒロインパラメータ
    public int affection;
    public int heroineAttack;
    public int heroineDefense;
    public int heroineSpeed;

    //進行フラグ
    public DayPhase currentPhase;
    public bool hasShownBulletinTutorial;
    public bool hasShownFastTravelTutorial;
    public bool isNewGame;
    public bool islastBossDefeated;

    //到達済み階層リスト
    public List<FloorData> visitedFloorData = new List<FloorData>();

    //倒した敵IDリスト
    public List<string> defeatedSymbolIds = new List<string>();

    //インベントリ
    public List<ItemSaveSlot> inventory = new List<ItemSaveSlot>();

    //位置情報
    public string currentSceneName;
    public float posX, posY, posZ;
    public string currentObjective;
}

[Serializable]
public class ItemSaveSlot
{
    public string itemName;
    public int count;
}
