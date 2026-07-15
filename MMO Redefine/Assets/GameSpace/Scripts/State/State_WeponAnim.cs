namespace State
{
    using System.Linq;
    using UnityEngine;
    using Fusion;
    using static State_StateBase;
    using Player;
    using static FNet.Fusion_Manager;
    public class State_WeponAnim : NetworkBehaviour
    {
        public State_StateBase Sta;
        public Player_WeponSet WepSet;
        public Animator Anim;
        public bool Wep2;
        public int ID;
        [Networked]public int net_ID { get; set; }
        int bid = -2;
        [SerializeField]
        Class_WeponAnimIDSet[] WepAnimSets;
        [System.Serializable]
        class Class_WeponAnimIDSet
        {
            public bool NAtks;
            public int BID;
            public int AnimID;
        }
        void Update()
        {
            if (Sta != null)
            {
                if (CanControl(Sta.Object))
                {
                    ID = -1;
                    for (int i = 0; i < WepAnimSets.Length; i++)
                    {
                        var wa = WepAnimSets[i];
                        if (!wa.NAtks)
                        {
                            var ats = Sta.AttackVals.Keys.ToArray();
                            for (int k = 0; k < ats.Length; k++)
                            {
                                var at = Sta.AttackVals[ats[k]];
                                if (at.Slot < 0) continue;
                                if (wa.BID == at.BID) ID = wa.AnimID;
                            }
                        }
                        else
                        {
                            var at = AtkGet(!Wep2 ? -1 : -2);
                            if (at != null && wa.BID == at.BID) ID = wa.AnimID;
                        }
                    }
                }
                else if (WepSet != null) ID = WepSet.AnimID;
                else ID = net_ID;

                if (CanControl(Sta.Object) && bid != ID)
                {
                    bid = ID;
                    if (WepSet != null) WepSet.AnimIDSet(ID);
                    else RPC_AnimIDSet(ID);
                }
            }

            if (Anim != null)
            {
                Anim.SetInteger("ID", ID);
            }
        }
        Class_AttackVal AtkGet(int ID)
        {
            return Sta.AttackVals.TryGetValue(ID,out var at) ? at : null;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_AnimIDSet(int ID)
        {
            net_ID = ID;
        }
    }
}

