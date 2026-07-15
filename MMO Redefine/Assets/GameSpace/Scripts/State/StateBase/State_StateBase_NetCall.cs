namespace State
{
    using Fusion;
    using System.Collections.Generic;
    using System.Data;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using FNet;
    using Datas;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    using static Datas.Data_Attack;
    public partial class State_StateBase
    {

        static int _dummyID;
        public void AddDamage(State_StateBase USta, float val, Struct_AtkValues atkval)
        {
            RPC_AddDamage(USta.Object.Id.Raw, val, atkval);
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_AddDamage(uint netid, float val, Struct_AtkValues atkval)
        {
            if (!CanControl(Object)) return;
            var tobj = Runner.FindObject(new NetworkId { Raw = netid });
            State_StateBase tsta = null;
            if (tobj != null) tsta = tobj.GetComponent<State_StateBase>();
            Event_AddDamage(tsta, val,atkval);
        }
        public void AddHeal(State_StateBase USta, float val, Struct_AtkValues atkval)
        {
            RPC_AddHeal(USta.Object.Id.Raw, val, atkval);
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_AddHeal(uint netid, float val, Struct_AtkValues atkval)
        {
            if (!CanControl(Object)) return;
            var tobj = Runner.FindObject(new NetworkId { Raw = netid });
            State_StateBase tsta = null;
            if (tobj != null) tsta = tobj.GetComponent<State_StateBase>();
            Event_AddHeal(tsta, val,atkval);
        }

        public void Damage(State_StateBase USta,Vector3 HitPos, float val, Struct_AtkValues atkval,byte RegHit = 1,byte RegEle = 1)
        {
            RPC_Damage(USta.Object.Id.Raw,HitPos, val, atkval,RegHit,RegEle);
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_Damage(uint netid, Vector3 HitPos, float val, Struct_AtkValues atkval, byte RegHit, byte RegEle)
        {
            if (!CanControl(Object)) return;
            var tobj = Runner.FindObject(new NetworkId { Raw = netid });
            State_StateBase tsta = null;
            if (tobj != null) tsta = tobj.GetComponent<State_StateBase>();
            if (float.IsNaN(val)) HP = float.NaN;
            else
            {
                if (val < 0)
                {
                    val *= ValGet(Enum_StateAddsType.AddHealMult) * 0.01f;
                    Event_TakeHeal(tsta, -val, atkval);
                    HP -= val;
                }
                else
                {
                    Event_TakeDamage(tsta, val, atkval);
                    var barrias = BufIndexs(Enum_Buf.Barria);
                    if (barrias.Count > 0)
                    {
                        BufPowChange(barrias[0], -1);
                        if (ChangeValues.Bufs[barrias[0]].CPow <= 0) ChangeValues.Bufs.RemoveAt(barrias[0]);
                        return;
                    }

                    EX += (val / F_MHP) * ValGet(Enum_StateAddsType.EXDamageCharge);
                    var dval = val;
                    var shilds = BufIndexs(Enum_Buf.Shild);
                    if (shilds.Count > 0)
                    {
                        for (int i = shilds.Count - 1; i >= 0; i--)
                        {
                            var rval = dval;
                            dval -= ChangeValues.Bufs[shilds[0]].CPow;
                            BufPowChange(shilds[0], -rval);
                            if (ChangeValues.Bufs[shilds[0]].CPow <= 0)
                            {
                                ChangeValues.Bufs.RemoveAt(shilds[0]);
                                shilds.RemoveAt(0);
                            }
                            else break;
                        }
                    }
                    if (dval > 0 && BufHas(Enum_Buf.MPShild) && MP > 0)
                    {
                        var rval = dval;
                        dval -= MP;
                        MP -= rval;
                    }
                    if (dval > 0) HP -= dval;
                }
            }

            Fusion_Reliable.DamageDisp(Object.Id.Raw,netid,HitPos, val, atkval.crit,atkval.element,RegHit,RegEle);
        }

        public void EXHitdd(float val)
        {
            RPC_EXHitAdd(val);
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_EXHitAdd(float val)
        {
            if (!CanControl(Object)) return;
            EX += val * ValGet(Enum_StateAddsType.EXHitCharge) * 0.01f;
        }

        public void BufSet(Class_BufAdd bufa)
        {
            BufSet(bufa.ID, bufa.Op, bufa.Index, bufa.Set, bufa.Times.x, bufa.Times.y, bufa.Pows.x, bufa.Pows.y);
        }
        public void BufSet(Class_AEvent_ShotBufAdd bufa, State_StateBase usta, Dictionary<Enum_ValueBase, float> otherValue = null)
        {
            var cpow = (bufa.CPow != null && bufa.CPow.Length > 0) ? Calc(bufa.CPow, usta, this, otherValue) : 0;
            var mpow = (bufa.MPow != null && bufa.MPow.Length > 0) ? Calc(bufa.MPow, usta, this, otherValue) : -1;
            BufSet(bufa.ID, bufa.Op, bufa.Index, bufa.Set, bufa.Times.x, bufa.Times.y, cpow, mpow);
            if (bufa.ID == Enum_Buf.Tyouhatu) RPC_TargetLockSet(usta.Object.Id.Raw);
        }
        public void BufSet(Enum_Buf bid, Enum_BufOp op, short index, Enum_BufSet setm, float ctime, float mtime, float cpow, float mpow)
        {
            var ctimeInt = Mathf.RoundToInt(ctime * 60);
            var mtimeInt = Mathf.RoundToInt(mtime * 60);
            RPC_BufSet((short)bid, (byte)op, index, (byte)setm, ctimeInt, mtimeInt, cpow, mpow);
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_BufSet(short bid, byte op, short Index, byte setm, int ctime, int mtime, float cpow, float mpow)
        {
            if (!CanControl(Object)) return;
            var set = (Enum_BufSet)setm;
            //Debug.Log(gameObject.name + ":" + (Enum_Buf)bid + "_" + (Enum_BufOp)op + "(" + Index + ")" + set + ",Time(" + ctime + "," + mtime + ")" + ",Pow(" + cpow + "," + mpow + ")");
            int slotNum = -1;
            slotNum = ChangeValues.Bufs.FindIndex(x => x.ID == bid && x.Index == Index && x.Op == op);

            Struct_Bufs bufd = slotNum >= 0 ? ChangeValues.Bufs[slotNum] :
            new Struct_Bufs { ID = bid, Op = op, Index = Index };
            if (set == Enum_BufSet.Remove)
            {
                if (slotNum >= 0) ChangeValues.Bufs.RemoveAt(slotNum);
            }
            else
            {
                if (set == Enum_BufSet.Add)
                {
                    bufd.CTime = Mathf.Max(bufd.CTime, ctime);
                    bufd.CPow = Mathf.Max(bufd.CPow, cpow);
                }
                if (set == Enum_BufSet.AddUp)
                {
                    mtime = Mathf.Max(ctime, mtime);
                    if (mpow >= 0) mpow = Mathf.Max(cpow, mpow);
                    bufd.CTime = Mathf.Max(bufd.CTime, Mathf.Min(bufd.CTime + ctime, mtime >= 0 ? mtime : int.MaxValue));
                    bufd.CPow = Mathf.Max(bufd.CPow, Mathf.Min(bufd.CPow + cpow, mpow >= 0 ? mpow : float.MaxValue));
                }
                bufd.MTime = Mathf.Max(bufd.MTime, bufd.CTime);
                bufd.MPow = Mathf.Max(bufd.MPow, bufd.CPow);
                if (slotNum >= 0) ChangeValues.Bufs[slotNum] = bufd;
                else ChangeValues.Bufs.Add(bufd);
            }
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_TargetLockSet(uint netid)
        {
            if (!CanControl(Object)) return;
            var tobj = Runner.FindObject(new NetworkId { Raw = netid });
            State_StateBase tsta = null;
            if(tsta!=null)ChangeValues.TyouhatuSta = tsta;
        }
        public void Shot(Class_AttackVal atv, Class_AEvent_ShotMain smain, Vector3 pos, Vector3 vect, Quaternion rot)
        {
            var eleride = EleRideGet(atv.Slot);
            if (!Runner.IsServer)
            {
                var objShot = smain.Shot.GetComponent<State_Shot_Base>();
                if (objShot != null)
                {
                    var dummyObj = Instantiate(smain.Shot, pos, rot);
                    dummyObj.name = "Dummy_" + dummyObj.name;
                    var dumRig = dummyObj.GetComponent<Rigidbody>();
                    if (dumRig != null)
                    {
                        dumRig.linearVelocity = vect;
                    }
                    var dumShot = dummyObj.GetComponent<State_Shot_Base>();
                    if (dumShot != null)
                    {
                        dumShot.LDummyID = _dummyID;
                        dumShot.LEleRideID = (byte)eleride;
                    }
                }
            }
            var shotIndex = atv.Attack.Events.IndexOf(smain);
            RPC_ShotRequest(atv.Attack.AddressableAddress,atv.Slot, shotIndex, _dummyID, pos, vect, rot, (byte)eleride);
            _dummyID++;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        void RPC_ShotRequest(NetworkString<_64> adress,int aslot, int shotIndex, int dummyID, Vector3 pos, Vector3 vect, Quaternion rot, byte eleride)
        {
            var handle = Addressables.LoadAssetAsync<Data_Attack>(adress.Value);
            Data_Attack datk = handle.WaitForCompletion();
            var smain = (Data_Attack.Class_AEvent_ShotMain)datk.Events[shotIndex];
            ShotSet(smain,aslot, dummyID, pos, vect, rot, eleride);
        }
        public void ShotSet(Class_AEvent_ShotMain ShotD, int aslot, int dummyID,Vector3 pos,Vector3 vect,Quaternion rot,byte eleride)
        {

            Runner.Spawn(ShotD.Shot, pos, rot, PlayerRef.None,
                onBeforeSpawned: (runner, obj) =>
                {
                    Fusion_RigSync.NStartSet(obj, pos, vect, rot);
                    var shot = obj.GetComponent<State_Shot_Base>();
                    if (shot != null)
                    {
                        shot.USta = this;
                        shot.NDummyID = dummyID;
                        shot.ShotData = ShotD;
                        shot.aslot = aslot;
                        shot.NEleRideID = eleride;
                    }
                });
        }


        public void Summon(Class_AttackVal atv, Class_AEvent_Summon summon, Vector3 pos, Vector3 vect, Quaternion rot)
        {
            var Index = atv.Attack.Events.IndexOf(summon);
            RPC_SummonRequest(atv.Attack.AddressableAddress, Index, pos, vect, rot);
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        void RPC_SummonRequest(NetworkString<_64> adress, int Index, Vector3 pos, Vector3 vect, Quaternion rot)
        {
            var handle = Addressables.LoadAssetAsync<Data_Attack>(adress.Value);
            Data_Attack datk = handle.WaitForCompletion();
            var summon = (Class_AEvent_Summon)datk.Events[Index];
            Runner.Spawn(summon.State, pos, rot, Object.InputAuthority,
                onBeforeSpawned: (runner, obj) =>
                {
                    Fusion_RigSync.NStartSet(obj, pos, vect, rot);
                    var state = obj.GetComponent<State_StateBase>();
                    if (state != null)
                    {
                        if(!summon.NoTeam)state.CommonValues.Team = CommonValues.Team;
                        state.CommonValues.LV = CommonValues.LV;
                        state.BaseValues.MHP = F_MHP * summon.HPPer * 0.01f;
                        state.BaseValues.HPRegene = ValGet(Enum_StateAddsType.HPRegene) * summon.HPPer * 0.01f;
                        state.BaseValues.PAtk = F_PAtk * summon.AtkPer * 0.01f;
                        state.BaseValues.MAtk = F_MAtk * summon.AtkPer * 0.01f;
                        state.BaseValues.PDef = F_PDef * summon.DefPer * 0.01f;
                        state.BaseValues.MDef = F_MDef * summon.DefPer * 0.01f;
                        state.ChangeValues.TimeLim = summon.TimeLimit;
                    }
                });
        }

        virtual public void HateAdd(State_StateBase USta,float dam,float tyouhatu)
        {

        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Dels()
        {
            Runner.Despawn(Object);
        }
    }
}
