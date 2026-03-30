using UnityEngine;

public class SceneMarker : MonoBehaviour
{
    [Header("識別用ID")]
    [SerializeField] private string markerId;

    [Header("遷移時のプレイヤーの向き")]
    [SerializeField] private Vector2 lookDirection = Vector2.down;

    //外部から参照できるようにする
    public string MarkerId => markerId;
    public Vector2 LookDirection => lookDirection;
    // Unityエディタ上で場所と向きを視覚的にわかりやすくする
    private void OnDrawGizmos()
    {
        // 到着地点に水色の球を表示
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.3f);

        // 向く方向に矢印（線）を表示
        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3(lookDirection.x, lookDirection.y, 0).normalized;
        Gizmos.DrawRay(transform.position, direction * 0.7f);
    }
}
