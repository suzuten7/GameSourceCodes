namespace Datas
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static Data_Equips;
    using static Data_Items;
    using static GmSystem.GS_EnumToJpString;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_SaveValues;
    using static UIs.UI_System;
    public class Data_Get : MonoBehaviour
    {
        static public Data_Base DB => DBd;
        static Data_Base DBd;
        static public void DBSet(Data_Base DBs)
        {
            DBd = DBs;
        }
        static public int GIDBrake(int GID, out Enum_ItemID ItemCategory, out int ItemType)
        {
            ItemCategory = (Enum_ItemID)(GID / (int)Enum_ItemID.Category * (int)Enum_ItemID.Category);
            ItemType = GID % (int)Enum_ItemID.Category / (int)Enum_ItemID.Type;
            if (ItemCategory == Enum_ItemID.Akuse) ItemType += 100;
            return GID % (int)Enum_ItemID.Type;
        }
        static public int GIDMake(Enum_ItemID Category, int Type, int Index)
        {
            return (int)Category + Type * (int)Enum_ItemID.Type + Index;
        }

        static public int ItemDataFindGID(Data_Item ItemData)
        {
            if (ItemData == null) return -1;
            int Index = -1;
            switch (ItemData)
            {
                case Data_Consumables:
                    Index = DB.Consumables.DataGetGID((Data_Consumables)ItemData);
                    if (Index >= 0) return Index;
                    break;
                case Data_Wepon:
                    Index = DB.Wepons.DataGetGID((Data_Wepon)ItemData);
                    if (Index >= 0) return Index;
                    break;
                case Data_Equipment:
                    Index = DB.Equipments.DataGetGID((Data_Equipment)ItemData);
                    if (Index >= 0) return Index;
                    break;
            }
            Index = DB.Items.DataGetGID(ItemData);
            if (Index >= 0) return Index;
            return -1;
        }
        static public Enum_ItemID ItemGIDCategoryGet(int GID)
        {
            if (GID < (int)Enum_ItemID.Material) return Enum_ItemID.Non;
            else if (GID >= (int)Enum_ItemID.Material && GID < (int)Enum_ItemID.Material + (int)Enum_ItemID.Category) return Enum_ItemID.Material;
            else if (GID >= (int)Enum_ItemID.Consumables && GID < (int)Enum_ItemID.Consumables + (int)Enum_ItemID.Category) return Enum_ItemID.Consumables;
            else if (GID >= (int)Enum_ItemID.Skill && GID < (int)Enum_ItemID.Skill + (int)Enum_ItemID.Category) return Enum_ItemID.Skill;
            else if (GID >= (int)Enum_ItemID.Wepon && GID < (int)Enum_ItemID.Wepon + (int)Enum_ItemID.Category) return Enum_ItemID.Wepon;
            else if (GID >= (int)Enum_ItemID.Armor && GID < (int)Enum_ItemID.Armor + (int)Enum_ItemID.Category) return Enum_ItemID.Armor;
            else if (GID >= (int)Enum_ItemID.Akuse && GID < (int)Enum_ItemID.Akuse + (int)Enum_ItemID.Category) return Enum_ItemID.Akuse;
            return Enum_ItemID.Other;
        }
        static public Data_SlotGID ItemGIDDataGet(int GID)
        {
            if (GID < 0) return null;
            var index = GIDBrake(GID, out var Category, out var Type);
            switch (Category)
            {
                case Enum_ItemID.Material: return DB.Items.GIDGetData(GID);
                case Enum_ItemID.Consumables: return DB.Consumables.GIDGetData(GID);
                case Enum_ItemID.Skill:
                    if (Type == 0)
                    {
                        var Attacks = DB.ALLSkills.ToList();
                        Attacks.AddRange(GetJobAttackList(0, true));
                        return Attacks[index];
                    } 
                    else return GetJobAttackList(Type - 1,false)[index];
                case Enum_ItemID.Wepon: return DB.Wepons.GIDGetData(GID);
                case Enum_ItemID.Armor:
                case Enum_ItemID.Akuse:
                    return DB.Equipments.GIDGetData(GID);
            }
            return null;
        }

        static public List<Data_Attack> GetAttackList(Enum_ItemSlotType slotType, Class_Local_CharaSet lchara)
        {

            List<Data_Attack> Attacks = null;
            switch (slotType)
            {
                case Enum_ItemSlotType.Skill_All:
                    Attacks = GetAttackList(Enum_ItemSlotType.Skill_Common, lchara);
                    Attacks.AddRange(GetAttackList(Enum_ItemSlotType.Skill_Job1, lchara));
                    Attacks.AddRange(GetAttackList(Enum_ItemSlotType.Skill_Job2, lchara));
                    break;
                case Enum_ItemSlotType.Skill_Common:
                    Attacks = DB.ALLSkills.ToList();
                    Attacks.AddRange(GetJobAttackList(0, true));
                    break;
                case Enum_ItemSlotType.Skill_Job1:
                    Attacks = GetJobAttackList(lchara.JobIDs[0],false);
                    break;
                case Enum_ItemSlotType.Skill_Job2:
                    Attacks = GetJobAttackList(lchara.JobIDs[1], false);
                    break;
            }
            return Attacks;
        }
        static public List<Data_Attack> GetJobAttackList(int JID,bool All)
        {
            var JD = DB.JobDatas[JID];
            var Attacks = !All ? JD.SkillAttacks.ToList() : new List<Data_Attack>();
            foreach (var JG in JD.JTGroupSet)
            {
                if (All != JG.JTreeGroup.Alls) continue;
                foreach (var JT in JG.JTreeGroup.JobTrees)
                {
                    foreach (var JTSA in JT.CTree.SkillAdds)
                    {
                        Attacks.Add(JTSA);
                    }
                }
            }
            return Attacks;
        }
        static public bool SkillCheck(Data_Attack AtkD, Class_Local_CharaSet LChara)
        {
            if (DB.ALLSkills.Contains(AtkD)) return true;
            var Trees = LChara.JobTrees;
            for (int i = 0; i < LChara.JobIDs.Length; i++)
            {
                var JID = LChara.JobIDs[i];
                var JD = DB.JobDatas[JID];
                if (JD.SkillAttacks.Contains(AtkD)) return true;
                for (int k = 0; k < JD.JTGroupSet.Count; k++)
                {
                    if (Trees[JID].Groups.Count <= k) continue;
                    for (int m = 0; m < JD.JTGroupSet[k].JTreeGroup.JobTrees.Count; m++)
                    {
                        var JTD = JD.JTGroupSet[k].JTreeGroup.JobTrees[m].CTree;
                        if (Trees[JID].Groups[k].LVs.Count <= m) continue;
                        if (Trees[JID].Groups[k].LVs[m] <= 0) continue;
                        if (JTD.SkillAdds.Contains(AtkD)) return true;
                    }
                }
            }
            return false;
        }

        static public void EquipSort(List<Class_EquipmentAdds> EquLists)
        {
            EquLists.Sort((a, b) =>
            {
                int result;
                result = a.EquipIfs.Length.CompareTo(b.EquipIfs.Length); // 要素1の昇順
                if (result != 0) return result;

                result = a.State.CompareTo(b.State); // 要素2の昇順
                if (result != 0) return result;

                result = a.Option.CompareTo(b.Option); // 要素3の昇順
                if (result != 0) return result;

                result = a.Values.x.CompareTo(b.Values.x); // 要素4の昇順
                return result;
            });
        }
        static public void EquipStrs( Class_EquipmentAdds EquAdd, int LV, out string OutName, out string OutOP, out string OutVal, out string OutIf)
        {
            OutName = EnumToJp(EquAdd.State);
            OutOP = "";
            OutVal = "";
            OutIf = "";
            switch (EquAdd.Option)
            {
                case Enum_StateAddsOption.BaseConst:
                    OutOP = "基礎";
                    break;
                case Enum_StateAddsOption.AddRate:
                    OutOP = "加算";
                    break;
                case Enum_StateAddsOption.BackConst:
                    OutOP = "後";
                    break;
                default:
                    OutOP = "最終";
                    break;
            }
            var Val = EquAdd.Values.x;
            if (LV > 0) Val += EquAdd.Values.y * (LV - 1);
            OutVal = Val >= 0 ? "+" : "";
            OutVal += ValueStrings(Val);
            if (LV >= 0 && EquAdd.Values.y != 0) OutVal += "(" + ValueStrings(EquAdd.Values.x) + "+" + ValueStrings(EquAdd.Values.y) + "×LV)";
            if (EquAdd.Option == Enum_StateAddsOption.AddRate || EquAdd.Option == Enum_StateAddsOption.FinalRate) OutVal += "%";

            if (EquAdd.EquipIfs.Length <= 0) OutIf = "";
            else
            {
                OutIf = "";
                for (int k = 0; k < EquAdd.EquipIfs.Length; k++)
                {
                    OutIf += "\n◆";
                    var EqIf = EquAdd.EquipIfs[k];
                    switch (EqIf.If)
                    {
                        case Enum_EquipIf.HPPer_xUp:
                            OutIf += "HP割合" + EqIf.Val.x.ToString("F1") + "%以上";
                            break;
                        case Enum_EquipIf.HPPer_xDown:
                            OutIf += "HP割合" + EqIf.Val.x.ToString("F1") + "%以下";
                            break;
                        case Enum_EquipIf.MPPer_x_Up:
                            OutIf += "MP割合" + EqIf.Val.x.ToString("F1") + "%以上";
                            break;
                        case Enum_EquipIf.MPPer_x_Down:
                            OutIf += "MP割合" + EqIf.Val.x.ToString("F1") + "%以下";
                            break;
                        case Enum_EquipIf.STPer_xUp:
                            OutIf += "ST割合" + EqIf.Val.x.ToString("F1") + "%以上";
                            break;
                        case Enum_EquipIf.STPer_xDown:
                            OutIf += "ST割合" + EqIf.Val.x.ToString("F1") + "%以下";
                            break;
                        case Enum_EquipIf.EX_xUp:
                            OutIf += "EX" + EqIf.Val.x.ToString("F1") + "以上";
                            break;
                        case Enum_EquipIf.EX_xDown:
                            OutIf += "EX" + EqIf.Val.x.ToString("F1") + "以下";
                            break;
                        default:
                            OutIf += EqIf.If.ToString();
                            break;
                    }
                }
            }
        }

    }
}
