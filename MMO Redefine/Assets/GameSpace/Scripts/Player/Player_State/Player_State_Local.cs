namespace Player
{
    using Datas;
    using Fusion;
    using Obj;
    using State;
    using System.Threading.Tasks;
    using UnityEngine;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static FNet.Fusion_Chat;
    using static FNet.Fusion_Manager;
    using static FNet.Fusion_Reliable;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_FireBaseSet;
    using static Datas.Data_Attack;

    public partial class Player_State
    {
        #region イベント
        protected override void Event_AddDamage(State_StateBase Sta, float val,Struct_AtkValues atkval)
        {
            base.Event_AddDamage(Sta, val, atkval);
            GraphAdd("AddDamage", val, atkval);
            EquipTriggerStart(Enum_EquipTrigger.AddDamage, Sta);
            for(int i = 0; i <= 3; i++)
            {
                AciveProgressAdd("TDam_" + i, val);
            }
            for (int i = 0; i <= 3; i++)
            {
                AciveProgressSetM("SDam_" + i, val);
            }
        }
        protected override void Event_TakeDamage(State_StateBase Sta, float val, Struct_AtkValues atkval)
        {
            base.Event_TakeDamage(Sta, val, atkval);
            GraphAdd("TakeDamage", val, atkval);
            EquipTriggerStart(Enum_EquipTrigger.TakeDamage, Sta);
        }
        protected override void Event_AddHeal(State_StateBase Sta, float val, Struct_AtkValues atkval)
        {
            base.Event_AddHeal(Sta, val, atkval);
            GraphAdd("AddHeal", val, atkval);
            EquipTriggerStart(Enum_EquipTrigger.AddHeal, Sta);
        }
        protected override void Event_TakeHeal(State_StateBase Sta, float val, Struct_AtkValues atkval)
        {
            base.Event_TakeHeal(Sta, val, atkval);
            GraphAdd("TakeHeal", val, atkval);
            EquipTriggerStart(Enum_EquipTrigger.TakeHeal, Sta);
        }
        protected override void Event_Death()
        {
            EquipTriggerStart(Enum_EquipTrigger.Death, ChangeValues.LastAttackSta);
        }
        protected override void Event_Atk(int id, int bid)
        {
            if (id < 0)
            {
                EquipTriggerStart(Enum_EquipTrigger.NormalAtk, this);
            }
            else
            {
                switch (ItemGIDCategoryGet(id))
                {
                    case Enum_ItemID.Consumables:
                        EquipTriggerStart(Enum_EquipTrigger.Consum, this);
                        break;
                    case Enum_ItemID.Skill:
                        EquipTriggerStart(Enum_EquipTrigger.Skill, this);
                        break;
                }
            }
        }
        #endregion
        void GraphAdd(string types,float val, Struct_AtkValues atkval)
        {
            if (ValGraph == null) return;
            if (!ValGraph.gameObject.activeInHierarchy) return;
            ValGraph.Adds(types + "_All", val);
                ValGraph.Adds(types + "_Hit", 1);
                if (atkval.crit) ValGraph.Adds(types + "_Crit", 1);
                if (atkval.aslot == -1) ValGraph.Adds(types+ "_SWR", val);
                else if (atkval.aslot == -2) ValGraph.Adds(types+ "_SWL", val);
                else ValGraph.Adds(types+ "_SOther", val);
                ValGraph.Adds(types+ "_A" + (Enum_AtkType)atkval.atktype, val);
                ValGraph.Adds(types+ "_" + (Enum_Element)atkval.element, val);
                ValGraph.Adds(types+ "_R" + (Enum_RangeType)atkval.rangetype, val);
            
        }
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Debug.Log("削除");
            if (!CanControl(Object)) return;
            if (BotID < 0) SaveSet();
        }
        void ObjectNameSet()
        {
            gameObject.name = "Player[" + Object.InputAuthority.PlayerId + "](" + CommonValues.Name + ")";
        }
        public void Respawne(bool NPoint)
        {
            HP = F_MHP;
            MP = F_MMP;
            ST = F_MST;
            if(!NPoint)EX = EXMax;
            SkillCTs.Clear();
            ChangeValues.Bufs.Clear();
            if (NPoint) return;

            var pos = LPlayerVal.RespawnePos;
            var ndis = float.MaxValue;
            foreach (var rp in FindObjectsByType<Obj_RespawnePoint>(FindObjectsSortMode.None))
            {
                if (rp == null) continue;
                var dis = Vector3.Distance(LPlayerVal.RespawnePos, rp.transform.position);
                if (ndis > dis)
                {
                    ndis = dis;
                    pos = rp.transform.position;
                }
            }
            pos += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized * Random.Range(SpawneRange.x, SpawneRange.y);

            SettingValues.Rig.transform.position = pos;
            SettingValues.Rig.transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);


        }
        public void EquipTriggerStart(Enum_EquipTrigger Trigger, State_StateBase TSta)
        {
            for (int i = 0; i < 2; i++)
            {
                var Wep = PlayerValues.SetWepons[i + (!PlayerValues.WepBack ? 0 : 2)];
                var WepD = DB.Wepons.GIDGetData(Wep.GID);
                if (WepD == null) continue;
                EquipTriggerEv((int)Enum_ItemID.Wepon + (int)Enum_ItemID.Category / 10 * i, Trigger, WepD.TriggerAttacks, TSta);
            }
            for (int i = 0; i < PlayerValues.SetArmors.Length; i++)
            {
                var Arm = PlayerValues.SetArmors[i];
                var ArmD = DB.Equipments.GIDGetData(Arm.GID);
                if (ArmD == null) continue;
                EquipTriggerEv((int)Enum_ItemID.Armor + (int)Enum_ItemID.Category / 10 * i, Trigger, ArmD.TriggerAttacks, TSta);
            }
            for (int i = 0; i < PlayerValues.SetAkuses.Length; i++)
            {
                var Aku = PlayerValues.SetAkuses[i];
                var AkuD = DB.Equipments.GIDGetData(Aku.GID);
                if (AkuD == null) continue;
                EquipTriggerEv((int)Enum_ItemID.Akuse + (int)Enum_ItemID.Category / 10 * i, Trigger, AkuD.TriggerAttacks, TSta);
            }
            for (int k = 0; k < PlayerValues.Jobs.Length; k++)
            {
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
                        EquipTriggerEv((int)Enum_ItemID.JobTree + (int)Enum_ItemID.Category / 10 * k + (int)Enum_ItemID.Category / 100 * i + m, Trigger, JTD.CTree.TriggerAttacks, TSta);
                    }
                }
            }
        }
        void EquipTriggerEv(int ID, Enum_EquipTrigger Trigger, Class_EquipmentTrigger[] EquipTrigs, State_StateBase TSta)
        {
            for (int i = 0; i < EquipTrigs.Length; i++)
            {
                if (EquipTrigs[i].Trigger != Trigger) continue;
                if (!EquipIfCheck(EquipTrigs[i].EquipIfs)) continue;
                var Pos = PosGet;
                var Rot = RotGet;
                if (TSta != null && EquipTrigs[i].TriggerStaTransUse)
                {
                    Pos = TSta.PosGet;
                    Rot = TSta.RotGet;
                }
                AttackStart(ID + i, EquipTrigs[i].Attack, Pos, Rot);
                AttackInput(ID + i, false, true);
                AttackInput(ID + i, true, true);
                AttackTrans(ID + i, Pos, Rot);
            }
        }

        public void ItemGIDUse(int GID, int Option = 0, int Index = -1)
        {
            var lchara = LPlayerCharas[CharaID];
            switch (ItemGIDCategoryGet(GID))
            {
                case Enum_ItemID.Consumables:
                    var ConsAtk = DB.Consumables.GIDGetData(GID).Attack;
                    if (!LPlayerVal.ConsumablesDic.ContainsKey(GID)) return;
                    if (AttackStart(GID, ConsAtk, PosGet, SettingValues.Rig.transform.eulerAngles))
                    {
                        LPlayerVal.ItemRem(GID, 1);
                    }
                    break;
                case Enum_ItemID.Skill:
                    var AttackD = (Data_Attack)ItemGIDDataGet(GID);
                    if (!SkillCheck(AttackD, lchara)) return;
                    AttackStart(GID, AttackD, PosGet, SettingValues.Rig.transform.eulerAngles);
                    break;
                case Enum_ItemID.Wepon:
                    var wepIndex = Index;
                    if (Index < 0)
                    {
                        for (int i = 0; i < LPlayerVal.Wepons.Count; i++)
                        {
                            if (LPlayerVal.Wepons[i].GID == GID)
                            {
                                wepIndex = i;
                                break;
                            }
                        }
                    }
                    if (wepIndex >= 0)
                    {
                        if (Option < 0) Option = !PlayerValues.WepBack ? 0 : 2;
                        var setBWep = lchara.SetWepons[Option];
                        lchara.SetWepons[Option] = LPlayerVal.Wepons[wepIndex];
                        LPlayerVal.Wepons[wepIndex] = setBWep;
                    }
                    break;
                case Enum_ItemID.Armor:
                    var armIndex = Index;
                    if (Index < 0)
                    {
                        for (int i = 0; i < LPlayerVal.Armors.Count; i++)
                        {
                            if (LPlayerVal.Armors[i].GID == GID)
                            {
                                armIndex = i;
                                break;
                            }
                        }
                    }
                    if (armIndex >= 0)
                    {
                        if (Option < 0) Option = 0;
                        var setBArm = lchara.SetArmors[Option];
                        lchara.SetArmors[Option] = LPlayerVal.Armors[armIndex];
                        LPlayerVal.Armors[armIndex] = setBArm;
                    }
                    break;
                case Enum_ItemID.Akuse:
                    var akuIndex = Index;
                    if (Index < 0)
                    {
                        for (int i = 0; i < LPlayerVal.Akuses.Count; i++)
                        {
                            if (LPlayerVal.Akuses[i].GID == GID)
                            {
                                akuIndex = i;
                                break;
                            }
                        }
                    }
                    if (akuIndex >= 0)
                    {
                        if (Option < 0) Option = 0;
                        var setBAku = lchara.SetAkuses[Option];
                        lchara.SetAkuses[Option] = LPlayerVal.Akuses[akuIndex];
                        LPlayerVal.Akuses[akuIndex] = setBAku;
                    }
                    break;

            }
        }
        public void ItemGIDDrop(int GID, int Index = -1)
        {
            var equipData = new Class_State_EquipmentValues();
            var equip = false;
            switch (ItemGIDCategoryGet(GID))
            {
                case Enum_ItemID.Material:
                case Enum_ItemID.Consumables:
                    LPlayerVal.ItemRem(GID, 1);
                    break;
                case Enum_ItemID.Wepon:
                    equipData = LPlayerVal.Wepons[Index];
                    LPlayerVal.Wepons.RemoveAt(Index);
                    equip = true;
                    break;
                case Enum_ItemID.Armor:
                    equipData = LPlayerVal.Armors[Index];
                    LPlayerVal.Armors.RemoveAt(Index);
                    equip = true;
                    break;
                case Enum_ItemID.Akuse:
                    equipData = LPlayerVal.Akuses[Index];
                    LPlayerVal.Akuses.RemoveAt(Index);
                    equip = true;
                    break;

            }
            if (!equip) RPC_ItemDrop(GID, "");
            else
            {
                if (Runner.GameMode == GameMode.Single) OffItemDrop(GID, equipData);
                else _ = FBSet_EquipItemDrop(GID, JsonUtility.ToJson(equipData));
            }


        }

        public void PStateSet()
        {
            CommonValues.Name = PlayerName + "「" + LPlayerCharas[CharaID].Name + "」";
            if (BotID > 0) CommonValues.Name += "(Bot" + BotID + ")";
            else if (BotID == 0) CommonValues.Name += "(不動)";
            BaseValues.MHP = 0;
            BaseValues.HPRegene = 0;
            BaseValues.MMP = 0;
            BaseValues.MPRegene = 0;
            BaseValues.MST = 0;
            BaseValues.STRegene = 0;
            BaseValues.PAtk = 0;
            BaseValues.MAtk = 0;
            BaseValues.PDef = 0;
            BaseValues.MDef = 0;
            foreach (var job in PlayerValues.Jobs)
            {
                var JobData = DB.JobDatas[job.ID];
                BaseValues.MHP += JobData.MHP;
                BaseValues.HPRegene += JobData.HPRegene;
                BaseValues.MMP += JobData.MMP;
                BaseValues.MPRegene += JobData.MPRegene;
                BaseValues.MST += JobData.MST;
                BaseValues.STRegene += JobData.STRegene;
                BaseValues.PAtk += JobData.PAtk;
                BaseValues.MAtk += JobData.MAtk;
                BaseValues.PDef += JobData.PDef;
                BaseValues.MDef += JobData.MDef;
                var LChara = LPlayerCharas[CharaID];
                if (job.ID < LChara.JobTrees.Count) job.Trees = LChara.JobTrees[job.ID].Groups;

            }
            var lvAdd = 1f + (CommonValues.LV - 1) * 0.01f;
            BaseValues.MHP *= lvAdd;
            BaseValues.HPRegene *= lvAdd;
            BaseValues.PAtk *= lvAdd;
            BaseValues.MAtk *= lvAdd;
            BaseValues.PDef *= lvAdd;
            BaseValues.MDef *= lvAdd;
        }
        public void LocalSetChara()
        {
            if (BotID < 0) CharaID = LPlayerVal.UseChara;
            var LChara = LPlayerCharas[CharaID];
            CommonValues.LV = Mathf.Clamp(LChara.SetLV > 0 ? LChara.SetLV : LPlayerVal.LV, 1, LPlayerVal.LV);
            PlayerValues.ModelMode = LChara.ModelMode;
            PlayerValues.ModelID = LChara.ModelID;
            switch (LChara.ModelMode)
            {
                default:
                    PlayerValues.ModelExID = "";
                    break;
                case 1:
                    PlayerValues.ModelExID = LChara.ModelVrm;
                    PlayerValues.ModelExScale.x = LChara.ScaleVrm;
                    break;
                case 2:
                    PlayerValues.ModelExScale = LChara.Scale2D;
                    var Model2Ds = GetSave_2DImages[LChara.Model2DID];
                    PlayerValues.Model2DSet = Model2Ds;
                    break;
            }

            for (int i = 0; i < PlayerValues.SetWepons.Length; i++) PlayerValues.SetWepons[i] = LChara.SetWepons[i];
            for (int i = 0; i < PlayerValues.WeponSkin.Length; i++) PlayerValues.WeponSkin[i] = LChara.WeponSkin[i];
            for (int i = 0; i < PlayerValues.SetArmors.Length; i++) PlayerValues.SetArmors[i] = LChara.SetArmors[i];
            for (int i = 0; i < PlayerValues.SetAkuses.Length; i++) PlayerValues.SetAkuses[i] = LChara.SetAkuses[i];
            for (int i = 0; i < PlayerValues.Jobs.Length; i++) PlayerValues.Jobs[i].ID = LChara.JobIDs[i];

            for (int i = 0; i < DB.JobDatas.Length; i++)
            {
                if (LChara.JobTrees.Count <= i) LChara.JobTrees.Add(new Class_Save_JTreeBase());
                var LJTrees = LChara.JobTrees[i];
                for (int k = 0; k < DB.JobDatas[i].JTGroupSet.Count; k++)
                {
                    if (LJTrees.Groups.Count <= k) LJTrees.Groups.Add(new Class_JobTrees());
                    var LJTGroups = LJTrees.Groups[k];
                    for (int m = LJTGroups.LVs.Count; m < DB.JobDatas[i].JTGroupSet[k].JTreeGroup.JobTrees.Count; m++) LJTGroups.LVs.Add(0);
                }
            }

            //
            for(int i = 0; i <= 3; i++)
            {
                AciveProgressSetM("Lv_" + i, CommonValues.LV);
            }
        }
        public void SaveSet()
        {
            Debug.Log("セーブ開始");
            Save();
            Debug.Log("セーブ完了");
        }

        static public void NStartSet(NetworkObject baseObj, int CharaID, int BotID)
        {
            var psta = baseObj.GetComponent<Player_State>();
            psta._net_BotID = BotID;
            psta._net_CharaID = CharaID;
        }
        public override void Spawned()
        {
            BotID = _net_BotID;
            CharaID = _net_CharaID;
        }
        protected override void Attack_EX(Class_AttackVal atv, Data_Attack.Class_AEvent_EX ex)
        {
            switch (ex.EXs)
            {
                case Data_Attack.Enum_AEventEXs.Bot_Battle:
                    if (BotID >= 0) return;
                    foreach (var pl in PStaList)
                    {
                        if (CanControl(pl.Object) && pl.BotID > 0)
                        {
                            var BotCont = (Player_BotInput)pl.Cont;
                            BotCont.Mode = 0;
                        }
                    }
                    break;
                case Data_Attack.Enum_AEventEXs.Bot_Follow:
                    if (BotID >= 0) return;
                    foreach (var pl in PStaList)
                    {
                        if (CanControl(pl.Object) && pl.BotID > 0)
                        {
                            var BotCont = (Player_BotInput)pl.Cont;
                            BotCont.Mode = 1;
                        }
                    }
                    break;
                case Data_Attack.Enum_AEventEXs.Bot_Scat:
                    if (BotID >= 0) return;
                    foreach (var pl in PStaList)
                    {
                        if (CanControl(pl.Object) && pl.BotID > 0)
                        {
                            var BotCont = (Player_BotInput)pl.Cont;
                            BotCont.Mode = 2;
                        }
                    }
                    break;
                case Data_Attack.Enum_AEventEXs.Bot_Tp:
                    if (BotID >= 0) return;
                    foreach (var pl in PStaList)
                    {
                        if (CanControl(pl.Object) && pl.BotID > 0)
                        {
                            var BotCont = (Player_BotInput)pl.Cont;
                            BotCont.Mode = 1;
                            pl.SettingValues.Rig.position = PosGet;
                        }
                    }
                    break;
                case Data_Attack.Enum_AEventEXs.ShortWarp:
                    var dis = 20f;
                    var rotvect = Vector3.up;
                    if (Cont.V2_Move.magnitude > 0.1f)
                    {
                        rotvect = CameraGet.forward * Cont.V2_Move.y + CameraGet.right * Cont.V2_Move.x;
                        rotvect.y = 0;
                        rotvect = rotvect.normalized;
                    }
                    foreach (var hit in Physics.RaycastAll(SettingValues.Rig.transform.position, rotvect, dis))
                    {
                        if (hit.rigidbody != null) continue;
                        if (hit.collider.isTrigger) continue;
                        if(dis > hit.distance)dis = hit.distance;
                    }
                    SettingValues.Rig.transform.position += rotvect * dis;
                    break;
                case Data_Attack.Enum_AEventEXs.RandomHeal:
                    RandomHeal();
                    break;
                case Data_Attack.Enum_AEventEXs.RandomBuf:
                    RandomBuf();
                    break;
                case Data_Attack.Enum_AEventEXs.EventMake:
                    EventMake();
                    break;
                case Data_Attack.Enum_AEventEXs.HalfHalf:
                    if (Random.value >= 0.5f)
                    {
                        LocalMessage(Enum_MesID.System, "ハーフハーフ","成功!!!");
                        EX += 20;
                    }
                    else
                    {
                        LocalMessage(Enum_MesID.System, "ハーフハーフ", "失敗???");
                        ChangeValues.Bufs.Clear();
                        HP = -F_MHP;
                    }
                        break;
            }
        }
        void RandomHeal()
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    LocalMessage(Enum_MesID.System,"ランダムヒール", "HPが回復!!!");
                    Damage(this,PosGet,-F_MHP * 0.3f, new Struct_AtkValues {element = (byte)Enum_Element.Wind });
                    break;
                case 1:
                    LocalMessage(Enum_MesID.System, "ランダムヒール", "MPが回復!!!」");
                    MP += F_MMP * 0.5f;
                    break;
                case 2:
                    LocalMessage(Enum_MesID.System, "ランダムヒール", "スタミナが回復!!!");
                    ST += F_MST * 0.8f;
                    break;
                case 3:
                    LocalMessage(Enum_MesID.System, "ランダムヒール", "EXが回復!!!");
                    EX += 10;
                    break;
            }
        }
        void RandomBuf()
        {
            switch (Random.Range(0, 6))
            {
                case 0:
                    LocalMessage(Enum_MesID.System,"ランダムバフ", "最大HP増加バフが付いた!!!");
                    BufSet(Enum_Buf.MaxHPChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 20, 20);
                    break;
                case 1:
                    LocalMessage(Enum_MesID.System, "ランダムバフ", "HP回復速度増加バフが付いた!!!");
                    BufSet(Enum_Buf.HPRegeneChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 40, 40);
                    break;
                case 2:
                    LocalMessage(Enum_MesID.System, "ランダムバフ", "物理攻撃力増加バフが付いた!!!");
                    BufSet(Enum_Buf.PAtkChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 20, 20);
                    break;
                case 3:
                    LocalMessage(Enum_MesID.System, "ランダムバフ", "魔法攻撃力増加バフが付いた!!!");
                    BufSet(Enum_Buf.MAtkChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 20, 20);
                    break;
                case 4:
                    LocalMessage(Enum_MesID.System, "ランダムバフ", "物理防御力増加バフが付いた!!!");
                    BufSet(Enum_Buf.PDefChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 20, 20);
                    break;
                case 5:
                    LocalMessage(Enum_MesID.System, "ランダムバフ", "魔法防御力増加バフが付いた!!!");
                    BufSet(Enum_Buf.MDefChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 20, 20);
                    break;

            }
        }
        void EventMake()
        {
            switch (Random.Range(0, 12))
            {
                case 0:
                case 1:
                    for (int i = 0; i < Random.Range(1, 4); i++) RandomHeal();
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < Random.Range(1, 4); i++) RandomBuf();
                    break;
                case 4:
                    LocalMessage(Enum_MesID.System,"イベントメイク", "何も起きなかった???");
                    break;
                case 5:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "大外れHPが1に!!!");
                    HP = 1;
                    break;
                case 6:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "毒薬を獲得???");
                    LPlayerVal.ItemAdd(GIDMake(Enum_ItemID.Consumables, 0, 1), 5);
                    break;
                case 7:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "保険を獲得した!!!");
                    BufSet(Enum_Buf.Revive, Enum_BufOp.Non, 777, Enum_BufSet.Add, 30, 30,50,50);
                    break;
                case 8:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "確定会心やったね!!!");
                    BufSet(Enum_Buf.CritPerChange, Enum_BufOp.Add_FinalConst, 777, Enum_BufSet.Add, 10, 10, 7777, 7777);
                    break;
                case 9:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "攻撃速度が加速した!!!");
                    BufSet(Enum_Buf.AtkSpeedChange, Enum_BufOp.Add_FinalRate, 777, Enum_BufSet.Add, 10, 10, 77, 77);
                    break;
                case 10:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "クールタイムが長く感じられる???");
                    BufSet(Enum_Buf.SkillCTChange, Enum_BufOp.Rem_FinalRate, 777, Enum_BufSet.Add, 10, 10, 77, 77);
                    break;
                case 11:
                    LocalMessage(Enum_MesID.System, "イベントメイク", "足が動かない???");
                    BufSet(Enum_Buf.MoveSpeedChange, Enum_BufOp.Rem_FinalRate, 777, Enum_BufSet.Add, 10, 10, 77, 77);
                    break;

            }
        }
    }
}
