
namespace Obj
{
    using UnityEngine;
    using Player;
    using FNet;
    using static FNet.Fusion_Reliable;
    using static GmSystem.GS_GlobalValues;
    using static UIs.UI_System;
    public class Obj_Signboard : Obj_ActionObject
    {
        public string Name;
        [TextArea]
        public string MessageStr;
        public Texture Icon;
        public override void PlayAction(Player_State PSta)
        {
            SayName = Name;
            SayMessage = MessageStr;
            SayIcon = Icon;
            SayOpen();
            Fusion_Chat.LocalMessage(Enum_MesID.System, Name, MessageStr);
        }
    }
}
