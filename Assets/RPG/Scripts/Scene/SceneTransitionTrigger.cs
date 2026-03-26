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

    //外部から参照できるようにする
    public EntranceType Type => entranceType;
    public string TargetSceneName => targetSceneName;
    public string DestinationText => destinationText;
    
}
