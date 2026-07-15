
namespace State
{
    using Datas;
    using Fusion;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    public class State_Shot_AddShot : NetworkBehaviour
    {
        [SerializeField] State_Shot_Base Shot;
        [SerializeField] ShotAdds[] Adds;
        [System.Serializable]
        class ShotAdds
        {
            public Vector3 Times;
            public Data_AddShot AddShot;
        }
        public void FixedUpdate()
        {
            if (!CanControl(Object)) return;
            for (int i = 0; i < Adds.Length; i++)
            {
                if (!V3TimeCheck(Shot._times, Adds[i].Times)) continue;
                Shot.AddShot(Adds[i].AddShot,Shot._times);
            }
        }
    }
}

