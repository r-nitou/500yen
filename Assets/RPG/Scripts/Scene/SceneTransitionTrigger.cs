using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("移動先のシーン")]
    [SerializeField] private string targetSceneName;
    [Header("表示するTextの内容")]
    [SerializeField] private string destinationText;

    //外部からシーン名と表示内容を参照できるようにする
    public string TargetSceneName => targetSceneName;
    public string DestinationText => destinationText;
    
}
