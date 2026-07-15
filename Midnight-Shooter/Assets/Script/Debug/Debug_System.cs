using TMPro;
using UnityEngine;

public class Debug_System : MonoBehaviour
{

    void Update()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        if (pm == null) return;
        if (!Net_Value.NetCheck || !Net_Value.NetValue.options[2]) return;
        var pl_Input = Player_Input.PI.pi;
        if (pl_Input.actions["d_1"].triggered) D_FullHP();
        if (pl_Input.actions["d_2"].triggered) pm.values.gun_bullet = 999;
        if (pl_Input.actions["d_3"].triggered) pm.values.now_Retention++;
        if (pl_Input.actions["d_4"].triggered) pm.values.ultCharge = 999;

        if (pl_Input.actions["d_7"].triggered)
        {
            pm.values.lastAtkPm = null;
            pm.values.lastAtkID = -1;
            pm.values.hpNow = -999;
            pm.values.hpOver = -999;
            pm.values.hpShild = -999;

        }
        if (pl_Input.actions["d_8"].triggered) pm.values.gun_bullet = 0;
        if (pl_Input.actions["d_9"].triggered) D_AllHP();
        if (pl_Input.actions["d_0"].triggered) pm.Respawne();
    }

    public void D_FullHP()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        pm.values.hpInjury = pm.states.max_HP;
        pm.values.hpNow = pm.states.max_HP;
        pm.values.hpOver = 0;
        pm.values.hpShild = 0;
    }
    public void D_AllHP()
    {
        foreach(var hpm in Obj_LocalObjects.Players)
        {
            hpm.values.hpInjury = hpm.states.max_HP;
            hpm.values.hpNow = hpm.states.max_HP;
            hpm.values.hpOver = 0;
            hpm.values.hpShild = 0;
        }
    }

}
