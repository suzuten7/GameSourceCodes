
namespace Obj
{
    using UnityEngine;
    using Player;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    public class Obj_AreaView : MonoBehaviour
    {
        public string Name;
        public int Order;
        public Vector2Int LvRanges;
        public Vector2 MapOffSet;
        protected virtual void OnTriggerEnter(Collider other)
        {
            var pAction = other.GetComponent<Player_Action>();
            if (pAction != null && CanControl(pAction.PSta.Object) && pAction.PSta.BotID < 0) CurrentAreas.Add(this);
        }
        private void OnTriggerExit(Collider other)
        {
            var pAction = other.GetComponent<Player_Action>();
            if (pAction != null && CanControl(pAction.PSta.Object) && pAction.PSta.BotID < 0) CurrentAreas.Remove(this);
        }
    }
}

