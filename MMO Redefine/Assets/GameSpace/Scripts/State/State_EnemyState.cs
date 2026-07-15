

namespace State
{
    using Datas;
    using FNet;
    using Fusion;
    using Obj;
    using UnityEngine;
    using static Datas.Data_Get;
    using static FNet.Fusion_NetValue;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using System.Collections.Generic;
    using Player;

    public class State_EnemyState : State_StateBase
    {
        public Data_Enemy EnemyD;
        [System.Serializable]
        public class Class_State_DropValues
        {
            public byte ExpCount;
            public int EXP;
            public int Lv1Exp;
            public Data_DropTable[] Drops;
        }

        List<State_StateBase> TakeList = new ();
        public Class_State_DropValues DropValues;
        public State_EnemyAI EAI;
        protected override void LVChangeOther(bool Lv1Val, float Mult)
        {
            if (Lv1Val)
            {
                DropValues.Lv1Exp = Mathf.RoundToInt(DropValues.EXP * Mult);
            }
            else
            {
                DropValues.EXP = Mathf.RoundToInt(DropValues.EXP * Mult);
            }
        }
        protected override void Event_TakeDamage(State_StateBase TSta, float val, Struct_AtkValues atkval)
        {
            base.Event_TakeDamage(TSta, val, atkval);
            if(!TakeList.Contains(TSta))TakeList.Add(TSta);
            if (EnemyD != null && TSta.TryGetComponent<Player_State>(out var psta))
                psta.RPC_EnemyAdd(DB.Enemys.DataGetID(EnemyD), false);
        }
        protected override void Event_Death()
        {
            var pr = new List<PlayerRef>();
            for (int i = 0; i < TakeList.Count; i++)
            {
                if (TakeList[i] == null) continue;
                var ip = TakeList[i].Object.InputAuthority;
                if (ip != null && !pr.Contains(ip)) pr.Add(ip);
            }
            if(DB.Enemys!=null)
            for(int i=0;i< pr.Count; i++)
            {
                for(int k = 0; k < PStaList.Count; k++)
                {
                    if (PStaList[k] == null) continue;
                    if (PStaList[k].Object.InputAuthority != pr[i]) continue;
                    PStaList[k].RPC_EnemyAdd(DB.Enemys.DataGetID(EnemyD), true);
                    break;
                }
            }
            if (pr.Count <= 0) return;
            var exp = Mathf.RoundToInt(DropValues.EXP * DificChange((int)CommonValues.Team, Enum_DifcVal.EXP));
            if (exp > 0)
            {
                Fusion_Reliable.EXPDrop(PosGet, DropValues.ExpCount, exp,pr);
            }
            if (DropValues.Drops != null && DropValues.Drops.Length > 0)
            {
                foreach(var Drd in DropValues.Drops)
                {
                    foreach(var DItem in Drd.DropGet((int)CommonValues.Team))
                    {
                        Runner.Spawn(DB.ItemObj, PosGet, Quaternion.Euler(RotGet), PlayerRef.None,
                        onBeforeSpawned: (runner, obj) =>
                        {
                        var ItemVect = new Vector3(Random.value - 0.5f, Random.value / 2f, Random.value - 0.5f).normalized
                       * Random.Range(200f, 1000f) * 0.01f;
                        Fusion_RigSync.NStartSet(obj, PosGet, ItemVect, Quaternion.Euler(RotGet));
                        var iobj = obj.GetComponent<Obj_ItemObj>();
                        iobj.ItemGID = DItem.Item1;
                        var CountMult = 1f + (CommonValues.LV - 1) * 0.05f;
                        iobj.ItemDataStr = Mathf.RoundToInt(DItem.Item2 * CountMult).ToString();
                        });
                    }
                }

            }
        }
        public override void HateAdd(State_StateBase USta,float dam, float tyouhatu)
        {
            if (EAI != null) RPC_HateAdd(dam, tyouhatu, USta.Object.Id.Raw);
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_HateAdd(float dam,float tyouhatu,uint netid)
        {
            if (!CanControl(Object)) return;
            var tobj = Runner.FindObject(new NetworkId { Raw = netid });
            State_StateBase tsta = null;
            if (tobj != null) tsta = tobj.GetComponent<State_StateBase>();
            if(dam>0) EAI.TargetAdds(1, tsta, dam);
            if(tyouhatu > 0) EAI.TargetAdds(2, tsta, tyouhatu);
        }
        public override GameObject TargetObjGet
        {
            get
            {
                if (EAI != null && EAI.Target != null) return EAI.Target.gameObject;
                return base.TargetObjGet;
            }
        }
        public override State_StateBase TargetStaGet
        {
            get
            {
                if(EAI != null && EAI.Target != null)return EAI.Target;
                return base.TargetStaGet;
            }
        }
        public override Vector3 TargetPosGet
        {
            get
            {
                if (EAI != null && EAI.Target != null) return EAI.Target.PosGet;
                return base.TargetPosGet;
            }
        }
    }
}
