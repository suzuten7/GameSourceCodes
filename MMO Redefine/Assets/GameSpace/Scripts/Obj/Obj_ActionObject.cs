namespace Obj
{
    using Fusion;
    using UnityEngine;
    using Player;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    public class Obj_ActionObject : NetworkBehaviour
    {
        public string ActionName;
        [Networked] public bool NoActiond { get; set; }
        public bool NoDisp
        {
            get
            {
                if (Object == null) return false;
                else return NoActiond;
            }
        }

        public virtual void PlayAction(Player_State PSta)
        {
            Debug.Log(ActionName + "実行:" + PSta.CommonValues.Name);
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            var pAction = other.GetComponent<Player_Action>();
            if (pAction != null && CanControl(pAction.PSta.Object) && pAction.PSta.BotID < 0) ActionObjs.Add(this);
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            var pAction = other.GetComponent<Player_Action>();
            if (pAction != null && CanControl(pAction.PSta.Object) && pAction.PSta.BotID < 0) ActionObjs.Remove(this);
        }
    }
}
