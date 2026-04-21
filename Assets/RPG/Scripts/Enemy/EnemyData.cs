using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("基本情報")]
    public string enemyName;
    public Sprite enemySprite;
    public int level = 1;

    [Header("ボス用")]
    public GameObject battlePrefab;

    [Header("ステータス")]
    public int maxHp;
    public int attack;
    public int defense;
    public int speed;

    [Header("報酬設定")]
    public int expValue;
    public ItemData dropItem;
}
