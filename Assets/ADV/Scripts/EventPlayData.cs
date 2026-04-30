using UnityEngine;
using UnityEngine.UI;

public enum EventCharacter
{
    Yuka = 0,   // 妹
    Mio,        // 道具屋
    Mei,        // 幼馴染
    Sontyo,     // 村長
}

[CreateAssetMenu(fileName = "EventPlayData", menuName = "Scriptable Objects/EventPlayData")]
public class EventPlayData : ScriptableObject
{
    public Sprite Still_ = null;
    public string NextSceneName_ = "VillageScene";
    public string HomeMarkerId_ = "HomeEntrance";
    public int PlayCount_ = 0;
    public EventCharacter Character_ = EventCharacter.Yuka;

    public string GetScenarioLabel()
    {
        // "Yuka_PlayeEvent_2" 等
        string chara = Character_.ToString();
        string count = PlayCount_.ToString();
        return chara + "_PlayeEvent_" + count;
    }
}
