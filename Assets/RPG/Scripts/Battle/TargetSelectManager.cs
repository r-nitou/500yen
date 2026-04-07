using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TargetSelectManager : MonoBehaviour
{
    public static TargetSelectManager instance { get; set; }

    [Header("UI参照")]
    [SerializeField] private RectTransform targetCursol;
    [SerializeField] private Vector2 cursolOffset = new Vector2(0, 100);

    private List<UnitStatus> targets = new List<UnitStatus>();
    private int currentIndex = 0;
    private bool isSelecting = false;
    private UnitStatus selectedTarget = null;

    private PlayerInputAction inputAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        inputAction = new PlayerInputAction();
        gameObject.SetActive(false);
    }

    //カーソルを動かす処理
    private void OnNavigate(InputAction.CallbackContext context)
    {
        float moveX = context.ReadValue<Vector2>().x;
        if (moveX > 0.5f)
        {
            currentIndex = (currentIndex + 1) % targets.Count;
        }
        else if (moveX < -0.5f) 
        {
            currentIndex = (currentIndex - 1 + targets.Count) % targets.Count;
        }

        UpdateSelectionVisual();
    }

    //対象を決定する処理
    private void OnDecision(InputAction.CallbackContext context)
    {
        selectedTarget = targets[currentIndex];
        isSelecting = false;
    }

    //対象選択をキャンセルする処理
    private void OnCancel(InputAction.CallbackContext context)
    {
        selectedTarget = null;
        isSelecting = false;
    }

    //ターゲットを選択する処理
    public async UniTask<UnitStatus> SelectTarget(List<UnitStatus> activeEnemies)
    {
        //生きている敵を取得
        targets = activeEnemies.FindAll(e => !e.isDead);

        if (targets.Count == 0)
        {
            return null;
        }

        //初期化
        currentIndex = 0;
        selectedTarget = null;
        isSelecting = true;
        gameObject.SetActive(true);

        //入力有効化
        inputAction.Battle.Enable();
        inputAction.Battle.Navigate.performed += OnNavigate;
        inputAction.Battle.Decision.performed += OnDecision;
        inputAction.Battle.Cancel.performed += OnCancel;

        UpdateSelectionVisual();

        //決定されるまで待機
        await UniTask.WaitUntil(() => !isSelecting);

        //色をリセット
        ResetEnemyColor();

        //入力無効化
        inputAction.Battle.Navigate.performed -= OnNavigate;
        inputAction.Battle.Decision.performed -= OnDecision;
        inputAction.Battle.Cancel.performed -= OnCancel;

        inputAction.Battle.Disable();

        gameObject.SetActive(false);

        return selectedTarget;
    }

    private void UpdateSelectionVisual()
    {
        if (targets == null || targets.Count == 0)
        {
            return;
        }

        //敵の色を暗くする
        foreach(var t in targets)
        {
            var img = t.transform.Find("Graphic").GetComponent<Image>();
            img.color = Color.gray;
        }

        UnitStatus currentTarget = targets[currentIndex];

        //選択中の敵を明るくする
        var selectedImg = currentTarget.transform.Find("Graphic").GetComponent<Image>();
        selectedImg.color = Color.white;

        //カーソル表示
        if (targetCursol != null)
        {
            targetCursol.gameObject.SetActive(true);
            //選択中の敵の座標を取得
            RectTransform targetRect=currentTarget.GetComponent<RectTransform>();

            //カーソル位置決定
            targetCursol.position = targetRect.position + (Vector3)cursolOffset;
        }
    }

    //敵の色を元に戻す処理
    private void ResetEnemyColor()
    {
        foreach(var t in targets)
        {
            if (t == null)
            {
                continue;
            }

            var img = t.transform.Find("Graphic").GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.white;
            }
        }

        //カーソル非表示
        if (targetCursol != null) 
        {
            targetCursol.gameObject.SetActive(false);
        }
    }
}
