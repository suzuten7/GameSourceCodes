using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/* 内容
 * ・最後に使用したデバイスの取得メソッド置き場
 */

/// <summary>
/// 最後に入力したデバイス
/// </summary>
public enum InputMode_Type
{
    Keyboard_Mouse,
    Controller
}

public static class Library_InputMode
{
    //[Tooltip("現在の入力デバイス")] public static InputMode_Type im_Type;

    /// <summary>
    /// 入力デバイスの変更検知
    /// </summary>
    /// <remarks>
    /// コントローラー：ボタン入力 or スティックの入力
    /// キーマウ　　　：キー入力 or マウス移動/クリック
    /// </remarks>
    /// <param name="deadZone"> スティックのデッドゾーン </param>
    public static void UpdateInputMode(float deadZone = 0.1f)
    {
        #region コントローラーの入力判定
        if (Gamepad.current != null)
        {
            Gamepad pad = Gamepad.current;

            #region スティック入力判定
            bool stickInput =
                pad.leftStick.ReadValue().sqrMagnitude > deadZone * deadZone ||
                pad.rightStick.ReadValue().sqrMagnitude > deadZone * deadZone;
            #endregion
            #region ボタン入力判定
            bool buttonInput =
                pad.buttonSouth.wasPressedThisFrame ||
                pad.buttonNorth.wasPressedThisFrame ||
                pad.buttonEast.wasPressedThisFrame ||
                pad.buttonWest.wasPressedThisFrame ||

                pad.leftShoulder.wasPressedThisFrame ||
                pad.rightShoulder.wasPressedThisFrame ||

                pad.leftTrigger.ReadValue() > deadZone ||
                pad.rightTrigger.ReadValue() > deadZone ||

                pad.startButton.wasPressedThisFrame ||
                pad.selectButton.wasPressedThisFrame ||

                pad.dpad.up.wasPressedThisFrame ||
                pad.dpad.down.wasPressedThisFrame ||
                pad.dpad.left.wasPressedThisFrame ||
                pad.dpad.right.wasPressedThisFrame;
            #endregion

            //条件を満たしたら…
            //if (stickInput || buttonInput)
            //{ im_Type = InputMode_Type.Controller; return; }
        }
        #endregion

        #region キーボード＆マウスの入力判定
        #region キーボード入力判定
        bool keyboardInput = false;

        if (Keyboard.current != null)
        {
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (key.wasPressedThisFrame)
                { keyboardInput = true; break; }
            }
        }
        #endregion
        #region マウス入力判定
        bool mouseInput = false;

        if (Mouse.current != null)
        {
            Mouse mouse = Mouse.current;

            mouseInput =
                mouse.delta.ReadValue().sqrMagnitude > 0.01f ||
                mouse.leftButton.wasPressedThisFrame ||
                mouse.rightButton.wasPressedThisFrame ||
                mouse.middleButton.wasPressedThisFrame;
        }
        #endregion

        //条件を満たしたら…
        //if (keyboardInput || mouseInput)
        //{ im_Type = InputMode_Type.Keyboard_Mouse; return; }
        #endregion
    }
}

