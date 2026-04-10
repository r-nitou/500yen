using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class RepeatIntaraction : IInputInteraction<Vector2>
{
#if UNITY_EDITOR
    static RepeatIntaraction()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterInteraction<RepeatIntaraction>();
    }

    [Tooltip("リピート間隔")]
    public float repeatInterval = .03f;
    public float deadZone = .2f;

    public static bool IsIntuitiveControl(InputControl control) => control is DeltaControl;
    public void Process(ref InputInteractionContext context)
    {
        if (IsIntuitiveControl(context.control))
        {
            if (context.control.IsPressed()) context.Performed();
        }
        else
        {
            // performedの呼び出しタイミングを定期的にするため、呼びだしは以下の二例のみ
            // 最初(waiting => performed)
            // timerHasExpiredの時(performed => performed)
            switch (context.phase)
            {
                case InputActionPhase.Waiting:
                    // 最初の呼び出し
                    if (context.control.IsPressed(deadZone))
                    {
                        context.PerformedAndStayPerformed();
                        context.SetTimeout(repeatInterval);
                    }
                    break;
                case InputActionPhase.Performed:
                    // タイマーイベント中、IsPressed() => performed
                    if (context.timerHasExpired)
                    {
                        if (context.control.IsPressed(deadZone))
                        {
                            context.PerformedAndStayPerformed();
                            context.SetTimeout(repeatInterval);
                        }
                    }
                    // 値変化時、IsNotPressed() => canceled
                    else
                    {
                        if (!context.control.IsPressed(deadZone))
                        {
                            context.Canceled();
                        }
                    }
                    break;
            }
        }
    }
    public void Reset()
    {
    }
}
