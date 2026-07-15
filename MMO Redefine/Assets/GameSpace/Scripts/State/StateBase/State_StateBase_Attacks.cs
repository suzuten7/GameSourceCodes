namespace State
{
    using Fusion;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static GmSystem.GS_GlobalState;
    using Datas;
    using static Datas.Data_Attack;
    using static Datas.Data_Get;
    public partial class State_StateBase : NetworkBehaviour
    {
        public Dictionary<int, Class_AttackVal> AttackVals = new Dictionary<int, Class_AttackVal>();
        public Dictionary<int, float> SkillCTs = new Dictionary<int, float>();
        [System.Serializable]
        public class Class_AttackVal
        {
            public Data_Attack Attack;
            public int Slot;
            public Vector3 Pos;
            public Vector3 Rot;
            public int BID;
            public float ASpdAdd;
            public float FTime;
            public int ITime;
            public bool Input_E;
            public float StayTime;
            public float ExitTime;
            public Class_AttackVal()
            {
                BID = 0;
                Slot = 0;
                ASpdAdd = 0;
                FTime = 0;
                ITime = 0;
            }
        }
        void CTUpdate()
        {
            var SCTKey = SkillCTs.Keys.ToArray();
            var ctSpeed = ValGet(Enum_StateAddsType.SkillCT) * 0.01f;
            foreach (var SKey in SCTKey)
            {
                if (SkillCTs[SKey] <= 0) SkillCTs.Remove(SKey);
                else SkillCTs[SKey]-= ctSpeed;
            }
        }
        public bool AttackStart(int id, Data_Attack attacks, Vector3 bpos, Vector3 brot,bool noUse=false,float aspdAdd = 0)
        {
            if (AttackVals.ContainsKey(id))
            {
                if (AttackVals[id].Attack != attacks)
                {
                    AttackVals.Remove(id);
                    SkillCTs.Remove(id);
                }
                else return false;
            }
            if (!noUse && SkillCTs.ContainsKey(id)) return false;
            if (!noUse && !AttackCostCheck(attacks.Costs)) return false;
            ChangeValues.LastAtkTime = 0;
            if (!noUse)
            {
                SkillCTs.Add(id, Mathf.RoundToInt(attacks.CT * 60));
                AttackCostRem(attacks.Costs);
            }
            AttackVals.Add(id, new Class_AttackVal
            {
                Attack = attacks,
                Slot = id,
                ASpdAdd = aspdAdd,
                Pos = bpos,
                Rot = brot,
            });
            Event_Atk(id, 0);
            return true;
        }
        public void AttackInput(int id, bool stay, bool input)
        {
            if (!AttackVals.ContainsKey(id)) return;
            if (!stay)
            {
                AttackVals[id].Input_E = input;
            }
            else
            {
                if (input) AttackVals[id].StayTime += 1f + ValGet(Enum_StateAddsType.AtkSpeed) * 0.01f;
                else
                {
                    AttackVals[id].ExitTime = AttackVals[id].StayTime;
                    AttackVals[id].StayTime = 0;
                }
            }
        }
        public void AttackTrans(int id, Vector3 bpos, Vector3 brot)
        {
            if (!AttackVals.ContainsKey(id)) return;
            AttackVals[id].Pos = bpos;
            AttackVals[id].Rot = brot;
        }
        void AttackUpdate()
        {
            ChangeValues.LastAtkTime++;
            ChangeValues.LastLookTime++;
            if (HP <= 0) AttackVals.Clear();
            var atKeys = AttackVals.Keys.ToArray();
            var SAtkUse = false;
            var LAtkUse = false;
            var RAtkUse = false;
            foreach (var atKey in atKeys)
            {
                var atv = AttackVals[atKey];
                var ad = atv.Attack;
                float aspd = 1f + atv.ASpdAdd * 0.01f;
                if (ad.ASpdUse) aspd *= ValGet(Enum_StateAddsType.AtkSpeed) * 0.01f;
                atv.FTime += aspd;
                for (; atv.ITime < atv.FTime; atv.ITime++)
                {
                    int ind = -1;
                    foreach (var aEvent in ad.Events)
                    {
                        ind++;
                        var bcheck = false;
                        if (aEvent.IfBID == atv.BID) bcheck = true;
                        if(aEvent.IfBIDs!=null)for (int i = 0; i < aEvent.IfBIDs.Length; i++)
                        {
                            if (aEvent.IfBIDs[i] == atv.BID)
                            {
                                bcheck = true;
                                break;
                            }
                        }
                        if (!bcheck) continue;
                        if (!V3TimeCheck(atv.ITime, aEvent.Times)) continue;
                        //Debug.Log("AEv" + ind + "|" + aEvent.TypeName);
                        switch (aEvent)
                        {
                            case Class_AEvent_BChange:
                                Attack_BChange(atv, (Class_AEvent_BChange)aEvent);
                                break;
                            case Class_AEvent_ShotMain:
                                Attack_Shot(atv, (Class_AEvent_ShotMain)aEvent, 1f);
                                if (ad.Type == (int)Enum_AttackType.通常 && BufHas(Enum_Buf.Kakusei))
                                    Attack_Shot(atv, (Class_AEvent_ShotMain)aEvent, 0.5f);
                                break;
                            case Class_AEvent_MyMove:
                                Attack_MyMove(atv, (Class_AEvent_MyMove)aEvent);
                                break;
                            case Class_AEvent_MyLook:
                                Attack_MyLook(atv, (Class_AEvent_MyLook)aEvent);
                                break;
                            case Class_AEvent_MyValState:
                                Attack_MyValState(atv, (Class_AEvent_MyValState)aEvent);
                                break;
                            case Class_AEvent_MyBuf:
                                Attack_MyBuf(atv, (Class_AEvent_MyBuf)aEvent);
                                break;
                            case Class_AEvent_Summon:
                                Attack_Summon(atv,(Class_AEvent_Summon)aEvent);
                                break;
                            case Class_AEvent_Anim:
                                Attack_Anim(atv, (Class_AEvent_Anim)aEvent);
                                break;

                            case Class_AEvent_SE:
                                Attack_SE(atv, (Class_AEvent_SE)aEvent);
                                break;
                            case Class_AEvent_EX:
                                Attack_EX(atv, (Class_AEvent_EX)aEvent);
                                break;
                        }
                    }
                }
                var branch = ad.Branchs.Find(x => x.BID == atv.BID);
                if (branch == null || (!branch.NoEnd && atv.FTime >= branch.EndTime * 60))
                {
                    AttackVals.Remove(atKey);
                }
                else
                {
                    atv.FTime = Mathf.Min(atv.FTime, branch.EndTime* 60);
                    atv.ITime = Mathf.Min(atv.ITime,Mathf.RoundToInt(branch.EndTime * 60));
                    switch (atv.Slot)
                    {
                        default: SAtkUse = true; break;
                        case -1: LAtkUse = true; break;
                        case -2: RAtkUse = true; break;

                    }
                }
            }
            if (!SAtkUse)
            {
                AnimValues.SAtkID = 0;
                AnimValues.SAtkSpeed = 1;
            }
            if (!LAtkUse)
            {
                AnimValues.LAtkID = 0;
                AnimValues.LAtkSpeed = 1;
            }
            if (!RAtkUse)
            {
                AnimValues.RAtkID = 0;
                AnimValues.RAtkSpeed = 1;
            }

        }
        void AttackCostGets(Class_AttackData_Costs[] Costs, out float[] Uses)
        {
            Uses = new float[] { 0, 0, 0, 0 };
            foreach (var cs in Costs)
            {
                switch (cs.Type)
                {
                    case Enum_Cost.HP定数:
                    case Enum_Cost.HP上限割合:
                    case Enum_Cost.現在HP割合:
                        Uses[0] += AttackCostGet(cs);
                        break;
                    case Enum_Cost.MP定数:
                    case Enum_Cost.MP上限割合:
                    case Enum_Cost.現在MP割合:
                        Uses[1] += AttackCostGet(cs);
                        break;
                    case Enum_Cost.ST定数:
                    case Enum_Cost.ST上限割合:
                    case Enum_Cost.現在ST割合:
                        Uses[2] += AttackCostGet(cs);
                        break;
                    case Enum_Cost.EX定数:
                        Uses[3] += AttackCostGet(cs);
                        break;

                }
            }
        }
        public bool AttackCostCheck(Class_AttackData_Costs[] Costs)
        {
            if (Costs == null) return true;
            AttackCostGets(Costs, out var Uses);
            if (Uses[0] > 0 && HP <= Uses[0]) return false;
            if (Uses[1] > 0 && MP < Uses[1]) return false;
            if (Uses[2] > 0 && ChangeValues.LowST) return false;
            if (Uses[3] > 0 && EX < Uses[3]) return false;
            return true;
        }
        void AttackCostRem(Class_AttackData_Costs[] Costs)
        {
            if (Costs == null) return;
            AttackCostGets(Costs, out var Uses);
            HP -= Uses[0];
            MP -= Uses[1];
            ST -= Uses[2];
            if (Uses[2] > 0) ChangeValues.STUseTime = 0;
            EX -= Uses[3];
        }
        public float AttackCostGet(Class_AttackData_Costs cs)
        {
            switch (cs.Type)
            {
                case Enum_Cost.HP定数:
                case Enum_Cost.MP定数:
                case Enum_Cost.ST定数:
                case Enum_Cost.EX定数:
                    return cs.Val;
                case Enum_Cost.HP上限割合:
                    return F_MHP * cs.Val * 0.01f;
                case Enum_Cost.現在HP割合:
                    return HP * cs.Val * 0.01f;
                case Enum_Cost.MP上限割合:
                    return F_MMP * cs.Val * 0.01f;
                case Enum_Cost.現在MP割合:
                    return MP * cs.Val * 0.01f;
                case Enum_Cost.ST上限割合:
                    return F_MST * cs.Val * 0.01f;
                case Enum_Cost.現在ST割合:
                    return ST * cs.Val * 0.01f;
            }
            return 0;
        }
        Vector3 Atk_PosGet(Enum_PosBase BPos, Vector3 DPos)
        {
            switch (BPos)
            {
                case Enum_PosBase.TargetPositon:
                    if (TargetStaGet != null) return TargetStaGet.PosGet;
                    break;
            }
            return DPos;
        }
        Vector3 Atk_RotGet(Enum_RotBase BRot, Vector3 DRot, Vector3 Pos)
        {
            switch (BRot)
            {
                case Enum_RotBase.Fixed: return Vector3.zero;
                case Enum_RotBase.MyToTarget:
                    if (TargetStaGet != null)
                    {
                        var rot = Quaternion.LookRotation(TargetPosGet - Pos, Vector3.forward).eulerAngles;
                        rot.z = -180f;
                        return rot;
                    }
                    break;
                case Enum_RotBase.ShotToTarget:
                    if (TargetStaGet != null)
                    {
                        var rot = Quaternion.LookRotation(TargetPosGet - Pos, Vector3.forward).eulerAngles;
                        rot.z = -180f;
                        return rot;
                    }
                    break;
                case Enum_RotBase.MyLook: return RotGet;
                case Enum_RotBase.CameraLook:
                    if (CameraGet != null) return CameraRot;
                    else return Atk_RotGet(Enum_RotBase.MyToTarget, DRot, Pos);
            }
            return DRot;
        }

        void Attack_BChange(Class_AttackVal atv, Class_AEvent_BChange bch)
        {
            bool IfCheck = true;
            foreach (var bif in bch.Ifs)
            {
                switch (bif)
                {
                    case Enum_BranchIfs.SingleInput:
                        if (!atv.Input_E) IfCheck = false;
                        break;
                    case Enum_BranchIfs.LongInput:
                        if (atv.StayTime <= 0 || atv.StayTime < bch.IfStayTime * 60) IfCheck = false;
                        break;
                    case Enum_BranchIfs.OutInput:
                        if (atv.ExitTime <= 0) IfCheck = false;
                        break;
                    case Enum_BranchIfs.NonInput:
                        if (atv.Input_E || atv.StayTime > 0) IfCheck = false;
                        break;
                    case Enum_BranchIfs.AddDamage:
                        if (ChangeValues.LastAddDamageTime > 5) IfCheck = false;
                        break;
                    case Enum_BranchIfs.TakeDamage:
                        if (ChangeValues.LastTakeDamageTime > 5) IfCheck = false;
                        break;
                    case Enum_BranchIfs.AddHeal:
                        if (ChangeValues.LastAddHealTime > 5) IfCheck = false;
                        break;
                    case Enum_BranchIfs.TakeHeal:
                        if (ChangeValues.LastTakeHealTime > 5) IfCheck = false;
                        break;
                }
                if (!IfCheck) break;
            }
            if (!IfCheck) return;
            if (!AttackCostCheck(bch.Costs)) return;
            ChangeValues.LastAtkTime = 0;
            AttackCostRem(bch.Costs);
            atv.BID = bch.NBID;
            atv.ITime = Mathf.FloorToInt(bch.NTime);
            atv.FTime = bch.NTime;
            Event_Atk(atv.Slot, bch.NBID);
        }

        void Attack_Shot(Class_AttackVal atv, Class_AEvent_ShotMain smain, float smult)
        {
            var sfire = smain.Fire;
            for (int i = 0; i < sfire.Count; i++)
            {
                var pos = Atk_PosGet(sfire.PosBase, atv.Pos);
                foreach (var pch in sfire.PosChange)
                {
                    pos += Quaternion.Euler(atv.Rot) * Atk_TransChange(pch, i,smain.Fire.Count, atv.ITime);
                }
                var rot = Atk_RotGet(sfire.RotBase, atv.Rot, pos);
                foreach (var rch in sfire.RotChange)
                {
                    rot += Atk_TransChange(rch, i, smain.Fire.Count, atv.ITime);
                }
                if (atv.Slot == -1) rot.z += 180;
                var qrot = Quaternion.Euler(rot);
                var vect = qrot * Vector3.forward * Random.Range(sfire.Speed.x, sfire.Speed.y) * smult * 0.01f;
                Shot(atv, smain, pos, vect, qrot);
            }
        }

        void Attack_MyMove(Class_AttackVal atv, Class_AEvent_MyMove mmove)
        {
            var rot = Atk_RotGet(mmove.RotBase, atv.Rot, atv.Pos);
            rot += mmove.RotOffSet;
            var vect = Quaternion.Euler(rot) * mmove.Vect * 0.01f;
            SettingValues.Rig.linearVelocity += vect;
            LookVects(vect, mmove.RotPer);
        }

        void Attack_MyLook(Class_AttackVal atv, Class_AEvent_MyLook mlook)
        {
            var rot = Atk_RotGet(mlook.RotBase, atv.Rot, atv.Pos);
            rot += mlook.RotOffSet;
            LookVects(Quaternion.Euler(rot) * Vector3.forward, mlook.RotPer);
        }

        void Attack_MyValState(Class_AttackVal atv, Class_AEvent_MyValState mval)
        {
            var val = Calc(mval.Val, this, this, null);
            switch (mval.State)
            {
                case Enum_ValState.HP:
                    if (val > 0)
                    {
                        val *= ValGet(Enum_StateAddsType.TakeHealMult) * 0.01f;
                        Event_AddHeal(this, val,new Struct_AtkValues { });
                    }
                    else Event_AddDamage(this, -val, new Struct_AtkValues { });
                    Damage(this,PosGet, -val, new Struct_AtkValues { });
                    break;
                case Enum_ValState.MP:
                    MP += val;
                    break;
                case Enum_ValState.ST:
                    ST += val;
                    break;
                case Enum_ValState.EX:
                    EX += val;
                    break;
            }
        }

        void Attack_MyBuf(Class_AttackVal atv, Class_AEvent_MyBuf mbuf)
        {
            var cpow = mbuf.CPow.Length > 0 ? Calc(mbuf.CPow, this, this, null) : 0;
            var mpow = mbuf.MPow.Length > 0 ? Calc(mbuf.MPow, this, this, null) : -1;
            BufSet(mbuf.ID, mbuf.Op, mbuf.Index, mbuf.Set, mbuf.AddTimes.x, mbuf.AddTimes.y, cpow, mpow);
        }

        void Attack_Summon(Class_AttackVal atv, Class_AEvent_Summon summon)
        {
            var sfire = summon.Fire;
            for (int i = 0; i < sfire.Count; i++)
            {
                var pos = Atk_PosGet(sfire.PosBase, atv.Pos);
                foreach (var pch in sfire.PosChange)
                {
                    pos += Quaternion.Euler(atv.Rot) * Atk_TransChange(pch, i,summon.Fire.Count, atv.ITime);
                }
                var rot = Atk_RotGet(sfire.RotBase, atv.Rot, pos);
                foreach (var rch in sfire.RotChange)
                {
                    rot += Atk_TransChange(rch, i, summon.Fire.Count, atv.ITime);
                }
                if (atv.Slot == -1) rot.z += 180;
                var qrot = Quaternion.Euler(rot);
                var vect = qrot * Vector3.forward * Random.Range(sfire.Speed.x, sfire.Speed.y) * 0.01f;
                Summon(atv, summon, pos, vect, qrot);
            }
        }
        void Attack_Anim(Class_AttackVal atv, Class_AEvent_Anim anim)
        {
            var animID = -1;
            if (anim.AnimClip != null)
            {
                switch (atv.Slot)
                {
                    case -1:
                    case -2:
                        animID = DB.WeponAttackClips.IndexOf(anim.AnimClip);
                        break;
                    default:
                        animID = DB.SkillAttackClips.IndexOf(anim.AnimClip);
                        break;
                }
            }
            switch (atv.Slot)
            {
                default: AnimValues.SAtkID = animID; AnimValues.SAtkCo = anim.ComboID; AnimValues.SAtkSpeed = 1f + anim.AnimSpeed * 0.01f; break;
                case -1: AnimValues.LAtkID = animID; AnimValues.LAtkCo = anim.ComboID; AnimValues.LAtkSpeed = 1f + anim.AnimSpeed * 0.01f; break;
                case -2: AnimValues.RAtkID = animID; AnimValues.RAtkCo = anim.ComboID; AnimValues.RAtkSpeed = 1f + anim.AnimSpeed * 0.01f; break;
            }
        }
        void Attack_SE(Class_AttackVal atv, Class_AEvent_SE se)
        {

        }
        protected virtual void Attack_EX(Class_AttackVal atv, Class_AEvent_EX ex)
        {
        }
        void LookVects(Vector3 mvect, float RotPers)
        {
            var rotVect = (PosGet + mvect) - PosGet;
            rotVect.y = 0;
            var lerpVect = Vector3.Slerp(SettingValues.Rig.transform.forward, rotVect, RotPers * 0.01f);
            lerpVect.y = 0;
            SettingValues.Rig.transform.LookAt(SettingValues.Rig.transform.position + lerpVect);
            ChangeValues.LastLookTime = 0;
        }
    }
}
