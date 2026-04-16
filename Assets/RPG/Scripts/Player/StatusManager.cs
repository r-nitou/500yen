using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager instance { get; set; }

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

    //HPを回復する処理
    public bool RecoverHP(int amount)
    {
        if (GameManager.instance == null) return false;

        //すでに満タンなら回復しない
        if (GameManager.instance.currentHp >= GameManager.instance.playerData.maxHP)
        {
            return false;
        }
        //回復処理
        GameManager.instance.currentHp += amount;
        //最大HPを超えないように制限
        if (GameManager.instance.currentHp > GameManager.instance.playerData.maxHP)
        {
            GameManager.instance.currentHp = GameManager.instance.playerData.maxHP;
        }
        return true;
    }
}
