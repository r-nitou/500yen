using UnityEngine;

public class PlayerHintDetector : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private GameObject hintIcon;
    [SerializeField] private float detectionDistance = 1.0f;
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] private LayerMask targetLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの向きを取得
        Vector3 detectionCenter = transform.position + (Vector3)PlayerMove.instance.LookDirection * detectionDistance;
        //周囲に対象があるか検索
        Collider2D hit = Physics2D.OverlapCircle(detectionCenter, detectionRadius, targetLayer);
        if (hit != null)
        {
            //アイコン表示
            if (!hintIcon.activeSelf)
            {
                hintIcon.SetActive(true);
            }
        }
        else
        {
            if (hintIcon.activeSelf)
            {
                hintIcon.SetActive(false);
            }
        }
    }

    // 検知範囲をシーンビューで可視化（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 detectionCenter = transform.position + (Vector3)PlayerMove.instance.LookDirection * detectionDistance;
        Gizmos.DrawWireSphere(detectionCenter, detectionRadius);
    }
}
