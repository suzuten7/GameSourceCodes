
namespace Player
{
    using UnityEngine;
    using static Datas.Data_Equips;
    using static Datas.Data_Items;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    using System.Collections.Generic;
    using State;

    public partial class Player_State
    {
        public override float AddsGet(Enum_StateAddsType State, Enum_StateAddsOption Option, bool Rev)
        {
            var Val = Option != Enum_StateAddsOption.FinalRate ? 0f : 1f;
            var WepCount = 0;
            for (int i = 0; i < 2; i++)
            {
                var Wep = PlayerValues.SetWepons[i + (!PlayerValues.WepBack ? 0 : 2)];
                var WepD = DB.Wepons.GIDGetData(Wep.GID);
                if (WepD == null) continue;
                WepCount++;
                EquipAdd(ref Val,WepD.EquipmentAdds, State, Option,Rev, Wep.LV);
                AddOp(ref Val, Wep.AddOps, State, Option, Rev);
            }
            if (State == Enum_StateAddsType.AtkSpeed && Option == Enum_StateAddsOption.FinalRate && WepCount <= 1) Val *= 1.5f;

            for (int i = 0; i < PlayerValues.SetArmors.Length; i++)
            {
                var Arm = PlayerValues.SetArmors[i];
                var ArmD = DB.Equipments.GIDGetData(Arm.GID);
                if (ArmD == null) continue;
                EquipAdd(ref Val, ArmD.EquipmentAdds, State, Option, Rev, Arm.LV);
                AddOp(ref Val, Arm.AddOps, State, Option, Rev);
            }
            for (int i = 0; i < PlayerValues.SetAkuses.Length; i++)
            {
                var Aku = PlayerValues.SetAkuses[i];
                var AkuD = DB.Equipments.GIDGetData(Aku.GID);
                if (AkuD == null) continue;
                EquipAdd(ref Val, AkuD.EquipmentAdds, State, Option, Rev, Aku.LV);
                AddOp(ref Val, Aku.AddOps, State, Option, Rev);
            }
            for (int k = 0; k < PlayerValues.Jobs.Length; k++)
            {
                var Adds = DB.JobDatas[PlayerValues.Jobs[k].ID].Adds;
                EquipAdd(ref Val, Adds, State, Option, Rev, CommonValues.LV);

                var JTDGroups = DB.JobDatas[PlayerValues.Jobs[k].ID].JTGroupSet;
                for (int i = 0; i < JTDGroups.Count; i++)
                {
                    var JTDTrees = JTDGroups[i].JTreeGroup.JobTrees;
                    for (int m = 0; m < JTDTrees.Count; m++)
                    {
                        var JTD = JTDTrees[m];
                        if (PlayerValues.Jobs[k].Trees.Count <= i) continue;
                        var JLVs = PlayerValues.Jobs[k].Trees[i].LVs;
                        if (JLVs.Count <= m) continue;
                        if (JLVs[m] <= 0) continue;
                        EquipAdd(ref Val, JTD.CTree.StateAdds, State, Option, Rev, JLVs[m]);
                    }
                }
            }

            return Val;
        }
        bool EquipIfCheck(Class_EquipmentIf[] Ifs)
        {
            if (Ifs == null) return true;
            for (int i = 0; i < Ifs.Length; i++)
            {
                var ifd = Ifs[i];
                switch (ifd.If)
                {
                    case Enum_EquipIf.HPPer_xUp:
                        if ((HP / F_MHP * 100) < ifd.Val.x) return false;
                        break;
                    case Enum_EquipIf.HPPer_xDown:
                        if ((HP / F_MHP * 100) > ifd.Val.x) return false;
                        break;
                    case Enum_EquipIf.MPPer_x_Down:
                        if ((MP / F_MMP * 100) < ifd.Val.x) return false;
                        break;
                    case Enum_EquipIf.MPPer_x_Up:
                        if ((MP / F_MMP * 100) > ifd.Val.x) return false;
                        break;
                    case Enum_EquipIf.STPer_xUp:
                        if ((ST / F_MST * 100) < ifd.Val.x) return false;
                        break;
                    case Enum_EquipIf.STPer_xDown:
                        if ((ST / F_MST * 100) > ifd.Val.x) return false;
                        break;
                    case Enum_EquipIf.WHand:
                        for (int j = 0; j < 2; j++)
                        {
                            var GID = PlayerValues.SetWepons[j + (!PlayerValues.WepBack ? 0 : 2)].GID;
                            if (GID >= 0) return false;
                        }
                        break;
                    case Enum_EquipIf.SHand:
                        var nonCount = 0;
                        for (int j = 0; j < 2; j++)
                        {
                            var GID = PlayerValues.SetWepons[j + (!PlayerValues.WepBack ? 0 : 2)].GID;
                            if (GID < 0) nonCount++;
                        }
                        if (nonCount < 1) return false;
                        break;
                    case Enum_EquipIf.Shild:
                        var shildCount = 0;
                        for (int j = 0; j < 2; j++)
                        {
                            var GID = PlayerValues.SetWepons[j + (!PlayerValues.WepBack ? 0 : 2)].GID;
                            if (GID >= 0 && DB.Wepons.GIDGetData(GID).Type == (int)Enum_WeponType.Shild) shildCount++;
                        }
                        if (shildCount < 1) return false;
                        break;
                    case Enum_EquipIf.WWepon:
                        var wepCount = 0;
                        for (int j = 0; j < 2; j++)
                        {
                            var GID = PlayerValues.SetWepons[j + (!PlayerValues.WepBack ? 0 : 2)].GID;
                            if (GID >= 0 && DB.Wepons.GIDGetData(GID).Type != (int)Enum_WeponType.Shild) wepCount++;
                        }
                        if (wepCount < 2) return false;
                        break;
                }

            }
            return true;
        }
        void EquipAdd(ref float CVal,Class_EquipmentAdds[] EquipAdd, Enum_StateAddsType State, Enum_StateAddsOption Option, bool Rev, int LV)
        {
            for (int i = 0; i < EquipAdd.Length; i++)
            {
                Adds(ref CVal, State, Option, Rev,  EquipAdd[i].State, EquipAdd[i].Option, EquipAdd[i].EquipIfs, EquipAdd[i].Values.x + EquipAdd[i].Values.y * (LV - 1));
            }
        }
        void AddOp(ref float CVal,List<Class_EquipmentAddOp> Ops,Enum_StateAddsType State,Enum_StateAddsOption Option,bool Rev)
        {
            for (int i = 0; i < Ops.Count; i++)
            {
                Adds(ref CVal, State, Option, Rev,  Ops[i].State, Ops[i].Option,null, 1 + Ops[i].LV * 0.1f);
            }
        }
        void Adds(ref float CVal, Enum_StateAddsType CState, Enum_StateAddsOption COption, bool Rev, Enum_StateAddsType AState, Enum_StateAddsOption AOption, Class_EquipmentIf[] Ifs, float AVal)
        {
            if (AState != CState) return;
            if (AOption != COption) return;
            if (Ifs!=null && !EquipIfCheck(Ifs)) return;
            if (Rev) AVal *= -1;
            if (COption == Enum_StateAddsOption.FinalRate) CVal *= 1f + AVal * 0.01f;
            else CVal += AVal;
        }
        public override Transform CameraGet => PlCamera.transform;
        public override Vector3 CameraRot
        {
            get
            {
                Vector3? pos = null;
                var dis = 100f;
                var hit = Physics.RaycastAll(PlCamera.transform.position, PlCamera.transform.forward, dis);
                for(int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider.TryGetComponent<State_StateHit>(out var shit))
                    {
                        if (TeamCheck(shit.State.CommonValues.Team) != Enum_TeamCheck.Enemy) continue;
                    }
                    else if (hit[i].collider.isTrigger) continue;
                    
                    if (dis <= hit[i].distance) continue;
                    dis = hit[i].distance;
                    pos = hit[i].point;
                }
                if (pos != null) return Quaternion.LookRotation(pos.Value - PosGet, Vector3.forward).eulerAngles;
                return PlCamera.transform.eulerAngles;
            }
        }

        public override GameObject TargetObjGet
        {
            get
            {
                if (Cont as Player_BotInput)
                {
                    var tar = ((Player_BotInput)Cont).Target;
                    if(tar!=null)return tar;
                }
                if (TargetHit != null) return TargetHit.gameObject;
                if (ChangeValues.LastHitSta != null && ChangeValues.LastAddDamageTime <= 600) return ChangeValues.LastHitSta.gameObject;
                return null;
            }
        }
        public override State_StateBase TargetStaGet
        {
            get
            {
                if (Cont as Player_BotInput)
                {
                    var tar = ((Player_BotInput)Cont).Target;
                    if (tar != null)
                    {
                        if (tar.TryGetComponent<State_StateBase>(out var sta)) return sta;
                        if (tar.TryGetComponent<State_StateHit>(out var shit)) return shit.State;
                    }
                }
                if (TargetHit != null) return TargetHit.State;
                if (ChangeValues.LastHitSta != null && ChangeValues.LastAddDamageTime <= 600) return ChangeValues.LastHitSta;
                return null;
            }
        }
        public override Vector3 TargetPosGet
        {
            get
            {
                if (Cont as Player_BotInput)
                {
                    var tar = ((Player_BotInput)Cont).Target;
                    if (tar != null)
                    {
                        if (tar.TryGetComponent<State_StateBase>(out var sta)) return sta.PosGet;
                        if (tar.TryGetComponent<State_StateHit>(out var shit)) return shit.PosGet;
                    }
                }
                if (TargetHit != null) return TargetHit.PosGet;
                if (ChangeValues.LastHitSta != null && ChangeValues.LastAddDamageTime <= 600) return ChangeValues.LastHitSta.PosGet;
                return PosGet;
            }
        }
    }
}
