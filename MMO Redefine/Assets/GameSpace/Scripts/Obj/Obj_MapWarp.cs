namespace Obj
{
    using static UIs.UI_System;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using FNet;
    using static FNet.Fusion_Reliable;
    public class Obj_MapWarp : Obj_MapAction
    {
        public override void Action()
        {
            MyPlayer.SettingValues.Rig.position = transform.position;
            ui_system.MapOpenClose();
            LPlayerVal.RespawnePos = transform.position;
            Fusion_Chat.LocalMessage(Enum_MesID.System, "ワープしました","");

        }
    }
}
