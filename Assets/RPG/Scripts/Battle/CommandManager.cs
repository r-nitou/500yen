using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandManager : MonoBehaviour
{
    [Header("コマンド設定")]
    [SerializeField] private GameObject commandObj;
    [SerializeField] private TMP_Text[] commandtexts;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private int currentIndex = 0;
    private PlayerInputAction inputAction;
    private bool isActive = false;

    private void Awake()
    {
        //InputActionのインスタンス生成
        inputAction = new PlayerInputAction();
        commandObj.SetActive(false);
    }

    //コマンド入力を受け付けるようにする処理
    public void SetPlayerInputActive(bool active)
    {
        isActive = active;
        commandObj.SetActive(active);

        currentIndex = 0;

        if (active)
        {
            //InputActionの有効化とイベント紐づけ
            inputAction.Battle.Enable();
            inputAction.Battle.Navigate.performed += OnNavigate;
            inputAction.Battle.Decision.performed += OnDecision;

            UpdateSelectCommand();
        }
        else
        {
            //InputActionの無効化とイベント紐づけ
            inputAction.Battle.Disable();
            inputAction.Battle.Navigate.performed -= OnNavigate;
            inputAction.Battle.Decision.performed -= OnDecision;
        }
    }

    //コマンド選択処理
    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isActive)
        {
            return;
        }

        float moveY = context.ReadValue<Vector2>().y;

        if (moveY > 0.5f)
        {
            currentIndex = (currentIndex - 1 + commandtexts.Length) % commandtexts.Length;
        }
        else if (moveY < -0.5f) 
        {
            currentIndex = (currentIndex + 1) % commandtexts.Length;
        }

        UpdateSelectCommand();
    }

    //コマンド決定処理
    private void OnDecision(InputAction.CallbackContext context)
    {
        if (!isActive)
        {
            return;
        }

        string selectedCommand = commandtexts[currentIndex].text;

        //入力を止め、BattleManagerに行く
        SetPlayerInputActive(false);
        BattleManager.instance.OnPlayerActionSelected(selectedCommand).Forget();
    }

    //UI表示を更新する処理
    private void UpdateSelectCommand()
    {
        for (int i = 0; i < commandtexts.Length; i++) 
        {
            commandtexts[i].color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }
}
