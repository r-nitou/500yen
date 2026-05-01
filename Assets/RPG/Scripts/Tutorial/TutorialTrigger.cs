using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("表示したいチュートリアルID")]
    [SerializeField] private string tutorialId;

    private bool isProcessing = false;

    //チュートリアルを表示する処理
    private async UniTaskVoid CheckAndShowTutorial()
    {
        isProcessing = true;

        await UniTask.Yield(PlayerLoopTiming.Update);

        if (GameManager.instance == null || GlobalUIManager.instance == null)
        {
            isProcessing = false;
            Debug.Log("チュートリアルを開けない");
            return;
        }

        bool alreadyShown = false;

        if (tutorialId == "FastTravel") 
        {
            alreadyShown = GameManager.instance.hasShowFastTravelTutorial;
        }

        if (!alreadyShown)
        {
            await GlobalUIManager.instance.ShowTutorial(tutorialId);

            if (tutorialId == "FastTravel") 
            {
                GameManager.instance.hasShowFastTravelTutorial = true;
            }
        }

        isProcessing = false;

        //反応しないようオブジェクトを破棄
        if (alreadyShown || GameManager.instance.hasShowFastTravelTutorial)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isProcessing)
        {
            CheckAndShowTutorial().Forget();
        }
    }
}
