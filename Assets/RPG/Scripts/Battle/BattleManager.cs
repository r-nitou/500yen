using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleManager : MonoBehaviour
{
    private bool isExiting = false;
    private PlayerInputAction inputAction;

    private void Awake()
    {
        //InputActionのインスタンスを取得
        inputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        //バトル用のInputActionを有効化
        inputAction.Battle.Enable();

        //イベントの紐づけ
        inputAction.Battle.ReturnScene.performed += OnDicision;
    }

    private void OnDisable()
    {
        //バトル用のInputActionを無効化
        inputAction.Battle.Disable();
        //イベントの紐づけ解除
        inputAction.Battle.ReturnScene.performed -= OnDicision;
    }

    private void OnDicision(InputAction.CallbackContext context)
    {
        if (isExiting)
        {
            return;
        }

        ReturnToDungeon().Forget();
    }

    private async UniTaskVoid ReturnToDungeon()
    {
        isExiting = true;

        await SceneLoader.instance.ExcuteSceneTransition("CaveScene", "CaveEntrance", PlayerMove.instance);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
