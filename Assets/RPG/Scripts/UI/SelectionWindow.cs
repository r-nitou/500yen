using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SelectionWindow : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text[] menuTexts;

    [Header("カラー設定")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    private int currentIndex = 0;

    //UIManagerから呼ばれる表示処理
    public async UniTask<bool> OpenWindow(string message, PlayerInputAction inputActions)
    {
        gameObject.SetActive(true);
        messageText.text = message;
        currentIndex = 0;
        UpdateVisuals();

        bool? result = null;

        //プレイヤーが決定するまで待機
        while (result == null)
        {
            //入力判定
            if (inputActions.UI.Navigate.triggered)
            {
                Vector2 navigate = inputActions.UI.Navigate.ReadValue<Vector2>();
                if (navigate.x != 0 || navigate.y != 0) 
                {
                    //入力方向に応じてインデックス更新
                    int direction = (navigate.x > 0 || navigate.y < 0) ? 1 : -1;
                    currentIndex = (currentIndex + direction + menuTexts.Length) % menuTexts.Length;

                    //UI更新
                    UpdateVisuals();
                }
            }

            //決定ボタンの判定
            if (inputActions.UI.Submit.triggered)
            {
                //Yesならtureを返す
                result = (currentIndex == 0);
            }
            //入力待ち
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        gameObject.SetActive(false);
        return result.Value;
    }

    //選択状態に合わせて色を更新する
    private void UpdateVisuals()
    {
        for(int i = 0; i < menuTexts.Length; i++)
        {
            menuTexts[i].color = (i == currentIndex) ? selectedColor : defaultColor;
        }
    }
}
