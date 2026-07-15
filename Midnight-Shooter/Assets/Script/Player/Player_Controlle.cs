using UnityEngine;
using UnityEngine.InputSystem;
public partial class Player_Controlle : MonoBehaviour
{
    public Player_Manager pm;
    [System.Serializable]
    public class Inputs
    {
        public bool trigger;
        public bool press;
        public bool pressTo;
        public void InputClear()
        {
            trigger = false;
            press = false;
        }
        public void InputSetAction(InputAction action,bool pressToggle = false)
        {
            trigger = action.triggered;
            if(!pressToggle)press = action.IsPressed();
            else
            {
                if (action.triggered) pressTo = !pressTo;
                press = pressTo;
            }
        }
        public void InputSetMobile(Player_Moblie.MInput minput, bool pressToggle = false)
        {
            trigger = minput.trigger;
            if (!pressToggle) press = minput.press;
            else
            {
                if (minput.trigger) pressTo = !pressTo;
                press = pressTo;
            }
        }
    }
    public Vector2 move;
    public bool targetMoveMode;
    public Vector2 target_move;
    public Vector2 target_pos;
    public Inputs shot;
    public Inputs ads;
    public Inputs reload;
    public Inputs dash;
    public Inputs walk;
    public Inputs melee;
    public Inputs gadget;
    public Inputs ult;
    public Inputs marker;

    private void Update()
    {
        if (!Net_Connect.CanControl(pm.Object)) return;
        InputClear();
        if (pm == Obj_LocalObjects.MyPlayer)
        {
            if (!Obj_LocalObjects.UIOpenCheck)
            {
                if (Player_Moblie.PMoblie == null || !Player_Moblie.PMoblie.Active) InputPlayer();
                else InputPMoblie();
            }
        }
        else InputCPU();
    }
    void InputClear()
    {
        move = Vector2.zero;
        target_move = Vector2.zero;
        shot.InputClear();
        ads.InputClear();
        reload.InputClear();
        dash.InputClear();
        walk.InputClear();
        gadget.InputClear();
        ult.InputClear();
        melee.InputClear();
        marker.InputClear();
    }
    void InputPlayer()
    {
        var pi = Player_Input.PI.pi;
        move = pi.actions["Move"].ReadValue<Vector2>();
        ads.InputSetAction(pi.actions["ADS"], UI_OptionManager.OptionGetOnOff("GP_Option 03", false));
        switch (pi.currentControlScheme)
        {
            default:
                targetMoveMode = false;
                target_pos = pi.actions["Target_Point"].ReadValue<Vector2>();
                break;
            case "Controller":
                targetMoveMode = true;
                target_move = pi.actions["Target_Move"].ReadValue<Vector2>();
                if (pi.currentControlScheme == "Controller")
                {
                    if (target_move.magnitude <= UI_OptionManager.OptionGetFloat("C_Option 01", 1f) * 0.01f) target_move = Vector2.zero;
                    if (!ads.press) target_move *= UI_OptionManager.OptionGetFloat("C_Option 03", 100) * 0.01f;
                    else target_move *= UI_OptionManager.OptionGetFloat("C_Option 04", 50) * 0.01f;
                }
                switch (UI_OptionManager.OptionGetInt("GP_Option 09", 0))
                {
                    case 1:
                        target_move.x *= -1;
                        break;
                    case 2:
                        target_move.y *= -1;
                        break;
                    case 3:
                        target_move *= -1;
                        break;
                }
                break;
        }
        shot.InputSetAction(pi.actions["Shot"]);

        reload.InputSetAction(pi.actions["Reload"]);
        dash.InputSetAction(pi.actions["Dash"],UI_OptionManager.OptionGetOnOff("GP_Option 01",false));
        walk.InputSetAction(pi.actions["Walk"], UI_OptionManager.OptionGetOnOff("GP_Option 02", false));
        melee.InputSetAction(pi.actions["Use_Melee"]);
        gadget.InputSetAction(pi.actions["Use_Gadget"]);
        ult.InputSetAction(pi.actions["Use_Ult"]);
        marker.InputSetAction(pi.actions["Marker"]);
    }
    void InputPMoblie()
    {
        var moblie = Player_Moblie.PMoblie;
        move = moblie.move;
        ads.InputSetMobile(moblie.ads, UI_OptionManager.OptionGetOnOff("GP_Option 03", false));
        var tmode = UI_OptionManager.OptionGetInt("To_Option 02", 0);
        switch (tmode)
        {
            default:
                targetMoveMode = true;
                target_move = moblie.target;
                if (target_move.magnitude <= UI_OptionManager.OptionGetFloat("C_Option 01", 1f) * 0.01f) target_move = Vector2.zero;
                break;
            case 1:
                targetMoveMode = true;
                target_move = moblie.target;
                target_move *= 0.1f;
                if (target_move.magnitude <= UI_OptionManager.OptionGetFloat("C_Option 01", 1f) * 0.01f) target_move = Vector2.zero;
                break;
            case 2:
                targetMoveMode = false;
                target_pos = moblie.target;
                break;
        }
        if (targetMoveMode)
        {
            if (!ads.press) target_move *= UI_OptionManager.OptionGetFloat("To_Option 03", 100) * 0.01f;
            else target_move *= UI_OptionManager.OptionGetFloat("To_Option 04", 50) * 0.01f;
            switch (UI_OptionManager.OptionGetInt("GP_Option 09", 0))
            {
                case 1:
                    target_move.y *= -1;
                    break;
                case 2:
                    target_move.x *= -1;
                    break;
                case 3:
                    target_move *= -1;
                    break;
            }
        }
        shot.InputSetMobile(moblie.shot);

        reload.InputSetMobile(moblie.reload);
        dash.InputSetMobile(moblie.dash, UI_OptionManager.OptionGetOnOff("GP_Option 01", false));
        walk.InputSetMobile(moblie.walk, UI_OptionManager.OptionGetOnOff("GP_Option 02", false));
        melee.InputSetMobile(moblie.melee);
        gadget.InputSetMobile(moblie.gadget);
        ult.InputSetMobile(moblie.ult);
        marker.InputSetMobile(moblie.marker);
    }
}
