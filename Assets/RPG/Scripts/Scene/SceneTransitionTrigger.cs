using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    //入り口のタイプ
    public enum EntranceType
    {
        None,
        Manual,
        Auto
    }

    [Header("入り口のタイプ")]
    [SerializeField] private EntranceType entranceType = EntranceType.None;
    [Header("移動先のシーン")]
    [SerializeField] private string targetSceneName;
    [Header("表示するTextの内容")]
    [SerializeField] private string destinationText;

    [Header("出現位置の指定")]
    [SerializeField] private string targetMarkerId;

    [Header("侵入可能な時間帯")]
    [SerializeField] private List<DayPhase> allowblePhase = new List<DayPhase>();

    public bool CanEnter()
    {
        if (GameManager.instance == null) return true;

        //現在の時間帯が侵入可能な時間帯に含まれているかを確認
        return allowblePhase.Contains(GameManager.instance.currentPhase);
    }

    //外部から参照できるようにする
    public EntranceType Type => entranceType;
    public string TargetSceneName => targetSceneName;
    public string DestinationText => destinationText;
    public string TargetMarkerId => targetMarkerId;
    
}
