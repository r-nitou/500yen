using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnviromentContoroller : MonoBehaviour
{
    [Header("ライト")]
    [SerializeField] private Light2D mainLight;

    [Header("朝の設定")]
    [SerializeField] private Color mornigColor = Color.white;
    [SerializeField] private float morningIntensity = 1f;

    [Header("夜の設定")]
    [SerializeField] private Color nightColor = new Color(0.2f, 0.2f, 0.5f);
    [SerializeField] private float nightIntensity = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyEnviroment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyEnviroment()
    {
        if (mainLight == null) return;

        //時間に応じてライトを切り替える
        switch (GameManager.instance.currentPhase)
        {
            case DayPhase.Morning:
                SetLight(mornigColor, morningIntensity);
                break;
            case DayPhase.Night:
                SetLight(nightColor, nightIntensity);
                break;
        }
    }

    private void SetLight(Color color,float intensity)
    {
        mainLight.color = color;
        mainLight.intensity = intensity;
    }
}
