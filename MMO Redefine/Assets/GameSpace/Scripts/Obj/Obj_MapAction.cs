namespace Obj
{
    using UnityEngine;
    using FNet;
    using static FNet.Fusion_Reliable;
    public class Obj_MapAction : MonoBehaviour
    {
        public string ActionName;
        virtual public void Action()
        {
            Fusion_Chat.LocalMessage(Enum_MesID.System, "マップアクション", "");
        }
    }
}

