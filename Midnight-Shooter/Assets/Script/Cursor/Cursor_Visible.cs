using UnityEngine;

/* 内容
 * ・カーソルの可視化と透明化
 * ・Keyboard&Mouse時のみ表示(画面外の時を除く)
 */

public class Cursor_Visible : MonoBehaviour
{

    void Update()
    {
        Vector2 mousePos = Player_Input.PI.pi.actions["Target_Point"].ReadValue<Vector2>();

        bool isInside =
            mousePos.x >= 0 &&
            mousePos.x <= Screen.width &&
            mousePos.y >= 0 &&
            mousePos.y <= Screen.height;

        //画面外なら常に表示
        if (!isInside) { Cursor.visible = true; return; }

        //キーマウ操作時
        if (Player_Input.PI.pi.currentControlScheme == "Keyboard&Mouse")
        {
            var on = false;
            if (Obj_LocalObjects.LocalObjects == null || Obj_LocalObjects.MyPlayer == null) on = true;
            if(Obj_LocalObjects.UIOpenCheck) on = true;
            if (UI_OptionManager.OptionGetOnOff("KM_Option 01", false))on = true;
            Cursor.visible = on;
        }
        //キーマウ以外操作時
        else { Cursor.visible = false; }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) Cursor.visible = true;
    }
}
