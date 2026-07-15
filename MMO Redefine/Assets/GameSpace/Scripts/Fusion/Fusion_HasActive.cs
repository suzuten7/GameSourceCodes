
namespace FNet
{
    using Fusion;
    using UnityEngine;
    using static GmSystem.GS_ChangeSet;
    public class Fusion_HasActive : NetworkBehaviour
    {
        [SerializeField] GameObject[] HasObjs;
        [SerializeField] GameObject[] OtherObjs;


        void Update()
        {
            bool has = Fusion_Manager.CanControl(Object);
            foreach (var obj in HasObjs)
            {
                if (obj != null)ChangeActive(obj,has);
            }
            foreach (var obj in OtherObjs)
            {
                if (obj != null)ChangeActive(obj, !has);
            }
        }
    }
}
