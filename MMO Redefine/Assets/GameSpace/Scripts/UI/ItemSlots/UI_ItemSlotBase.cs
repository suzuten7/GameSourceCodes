namespace UIs
{
    using Datas;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static Player.Player_Controle;
    using static UI_System;
    public class UI_ItemSlotBase : MonoBehaviour
    {
        public UI_CharaBase CharaUI;
        public Enum_ItemSlotType SlotType;
        public int SlotNum;
        public int USID;
        public int SetEquip;
        public Image Flame;
        [SerializeField] RawImage Icon;
        [SerializeField] TextMeshProUGUI NameTx;
        [SerializeField] TextMeshProUGUI CountTx;
        [SerializeField] TextMeshProUGUI NumTx;
        [SerializeField] TextMeshProUGUI InfoTx;
        int bGID = int.MinValue;
        public Data_Attack GetSkillAtk
        {
            get
            {


                Data_Attack AttackD = null;
                var ComAtks = GetAttackList(Enum_ItemSlotType.Skill_Common, CharaUI.LChara);
                var J1Atks = GetAttackList(Enum_ItemSlotType.Skill_Job1, CharaUI.LChara);
                var J2Atks = GetAttackList(Enum_ItemSlotType.Skill_Job2, CharaUI.LChara);
                switch (SlotType)
                {
                    case Enum_ItemSlotType.Skill_All:
                        int OffSet = 0;

                        if (SlotNum < ComAtks.Count + OffSet)
                        {
                            AttackD = ComAtks[SlotNum - OffSet];
                            break;
                        }
                        OffSet += ComAtks.Count;
                        if (SlotNum < J1Atks.Count + OffSet)
                        {
                            AttackD = J1Atks[SlotNum - OffSet];
                            break;
                        }
                        OffSet += J1Atks.Count;
                        if (SlotNum < J2Atks.Count + OffSet)
                        {
                            AttackD = J2Atks[SlotNum - OffSet];
                            break;
                        }
                        break;
                    case Enum_ItemSlotType.Skill_Common:
                        if (SlotNum < ComAtks.Count) AttackD = ComAtks[SlotNum];

                        break;
                    case Enum_ItemSlotType.Skill_Job1:
                        if (SlotNum < J1Atks.Count) AttackD = J1Atks[SlotNum];
                        break;
                    case Enum_ItemSlotType.Skill_Job2:
                        if (SlotNum < J2Atks.Count) AttackD = J2Atks[SlotNum];
                        break;
                }
                return AttackD;
            }
        }
        public int ItemGID
        {
            get
            {
                var lchara = CharaUI.LChara;
                if (lchara == null) return -1;
                switch (SlotType)
                {
                    case Enum_ItemSlotType.UISlot:return USID;
                    case Enum_ItemSlotType.Shop:
                        if (CurrentShop.ShopD.Buys.Count <= SlotNum) return -1;
                        return ItemDataFindGID(CurrentShop.ShopD.Buys[SlotNum].ItemD);
                    case Enum_ItemSlotType.Craft: return CraftSets.Count > SlotNum ? CraftSets[SlotNum].x : -1;
                    case Enum_ItemSlotType.ShortCut: return lchara.ShortCutSets[SlotNum];
                    case Enum_ItemSlotType.BotSet:return lchara.BotSets[SlotNum];
                    case Enum_ItemSlotType.Material:
                        var ItemKeys = LPlayerVal.ItemsDic.Keys.ToArray();
                        return ItemKeys[SlotNum];
                    case Enum_ItemSlotType.Consumables:
                        var ConsKeys = LPlayerVal.ConsumablesDic.Keys.ToArray();
                        return ConsKeys[SlotNum];
                    case Enum_ItemSlotType.Has_Wepon: return LPlayerVal.Wepons[SlotNum].GID;
                    case Enum_ItemSlotType.Has_Armor: return LPlayerVal.Armors[SlotNum].GID;
                    case Enum_ItemSlotType.Has_Akuse: return LPlayerVal.Akuses[SlotNum].GID;
                    case Enum_ItemSlotType.Set_Wepon: return lchara.SetWepons[SlotNum].GID;
                    case Enum_ItemSlotType.Set_Armor: return lchara.SetArmors[SlotNum].GID;
                    case Enum_ItemSlotType.Set_Akuse: return lchara.SetAkuses[SlotNum].GID;
                    case Enum_ItemSlotType.Wepon_Skin:return lchara.WeponSkin[SlotNum];

                    case Enum_ItemSlotType.Skill_All:
                    case Enum_ItemSlotType.Skill_Common:
                    case Enum_ItemSlotType.Skill_Job1:
                    case Enum_ItemSlotType.Skill_Job2:
                        var AttackD = GetSkillAtk;
                        var SkillIndex = GetAttackList(Enum_ItemSlotType.Skill_Common, CharaUI.LChara).IndexOf(AttackD);
                        if (SkillIndex >= 0)return GIDMake(Enum_ItemID.Skill, 0, SkillIndex);
                        for (int i = 0; i < DB.JobDatas.Length; i++)
                        {
                            SkillIndex = GetJobAttackList(i, false).IndexOf(AttackD);
                            if (SkillIndex >= 0)return GIDMake(Enum_ItemID.Skill, i + 1, SkillIndex);
                        }
                        break;
                }
                return -1;
            }
        }
        public string ItemCountLVStr
        {
            get
            {
                var lchara = CharaUI.LChara;
                switch (SlotType)
                {
                    case Enum_ItemSlotType.UISlot:
                        switch (ItemGIDCategoryGet(ItemGID))
                        {
                            case Enum_ItemID.Material:
                            case Enum_ItemID.Consumables:
                                return "×" + ItemCount;
                        }
                        return ItemGIDDataGet(USID).Name;
                    case Enum_ItemSlotType.Shop:
                        return CurrentShop != null ? CurrentShop.ShopD.Buys[SlotNum].ItemD.Name : "Null";
                    case Enum_ItemSlotType.ShortCut:
                    case Enum_ItemSlotType.BotSet:
                        switch (ItemGIDCategoryGet(ItemGID))
                        {
                            case Enum_ItemID.Material:
                            case Enum_ItemID.Consumables:
                                return "×" + ItemCount;
                        }
                        return "";
                    case Enum_ItemSlotType.Material:
                        return "×" + ItemCount;
                    case Enum_ItemSlotType.Consumables:
                        return "×" + ItemCount;
                    case Enum_ItemSlotType.Has_Wepon: return "LV" + LPlayerVal.Wepons[SlotNum].LV;
                    case Enum_ItemSlotType.Has_Armor: return "LV" + LPlayerVal.Armors[SlotNum].LV;
                    case Enum_ItemSlotType.Has_Akuse: return "LV" + LPlayerVal.Akuses[SlotNum].LV;
                    case Enum_ItemSlotType.Set_Wepon: return "LV" + lchara.SetWepons[SlotNum].LV;
                    case Enum_ItemSlotType.Set_Armor: return "LV" + lchara.SetArmors[SlotNum].LV;
                    case Enum_ItemSlotType.Set_Akuse: return "LV" + lchara.SetAkuses[SlotNum].LV;

                    case Enum_ItemSlotType.Skill_All:
                    case Enum_ItemSlotType.Skill_Common:
                    case Enum_ItemSlotType.Skill_Job1:
                    case Enum_ItemSlotType.Skill_Job2:
                        return SlotGIDD.Name;
                }
                return "";
            }
        }

        public Data_SlotGID SlotGIDD
        {
            get
            {
                return ItemGIDDataGet(ItemGID);
            }
        }
        public int ItemCount
        {
            get
            {
                switch (SlotType)
                {
                    case Enum_ItemSlotType.UISlot:
                    case Enum_ItemSlotType.ShortCut:
                    case Enum_ItemSlotType.BotSet:
                        switch (ItemGIDCategoryGet(ItemGID))
                        {
                            case Enum_ItemID.Material:
                                if (!LPlayerVal.ItemsDic.ContainsKey(ItemGID)) return 0;
                                else return LPlayerVal.ItemsDic[ItemGID];
                            case Enum_ItemID.Consumables:
                                if (!LPlayerVal.ConsumablesDic.ContainsKey(ItemGID)) return 0;
                                else return LPlayerVal.ConsumablesDic[ItemGID];
                        }
                        break;
                    case Enum_ItemSlotType.Material:
                        if (!LPlayerVal.ItemsDic.ContainsKey(ItemGID)) return 0;
                        else return LPlayerVal.ItemsDic[ItemGID];
                    case Enum_ItemSlotType.Consumables:
                        if (!LPlayerVal.ConsumablesDic.ContainsKey(ItemGID)) return 0;
                        else return LPlayerVal.ConsumablesDic[ItemGID];
                }
                return 1;
            }
        }
        public Class_State_EquipmentValues ItemEquipVal
        {
            get
            {
                var lchara = CharaUI.LChara;
                switch (SlotType)
                {
                    case Enum_ItemSlotType.UISlot:
                        switch (ItemGIDCategoryGet(ItemGID))
                        {
                            case Enum_ItemID.Wepon: return SetEquip < 0 ? LPlayerVal.Wepons[SlotNum] : LPlayerCharas[SetEquip].SetWepons[SlotNum];
                            case Enum_ItemID.Armor: return SetEquip < 0 ? LPlayerVal.Armors[SlotNum] : LPlayerCharas[SetEquip].SetArmors[SlotNum];
                            case Enum_ItemID.Akuse: return SetEquip < 0 ? LPlayerVal.Akuses[SlotNum] : LPlayerCharas[SetEquip].SetAkuses[SlotNum];
                        }
                        break;
                    case Enum_ItemSlotType.Has_Wepon: return LPlayerVal.Wepons[SlotNum];
                    case Enum_ItemSlotType.Has_Armor: return LPlayerVal.Armors[SlotNum];
                    case Enum_ItemSlotType.Has_Akuse: return LPlayerVal.Akuses[SlotNum];
                    case Enum_ItemSlotType.Set_Wepon: return lchara.SetWepons[SlotNum];
                    case Enum_ItemSlotType.Set_Armor: return lchara.SetArmors[SlotNum];
                    case Enum_ItemSlotType.Set_Akuse: return lchara.SetAkuses[SlotNum];
                }
                return null;
            }
        }
        public void ItemChange(UI_ItemSlotBase citem)
        {
            var lchara = CharaUI.LChara;
            switch (SlotType)
            {
                case Enum_ItemSlotType.UISlot:
                    SlotNum = citem.SlotNum;
                    USID = citem.ItemGID;
                    switch (citem.SlotType)
                    {
                        default:
                            SetEquip = -1;
                            break;
                        case Enum_ItemSlotType.Set_Wepon:
                        case Enum_ItemSlotType.Set_Armor:
                        case Enum_ItemSlotType.Set_Akuse:
                            SetEquip = citem.CharaUI.CharaID >= 0 ? citem.CharaUI.CharaID : LPlayerVal.UseChara;
                            break;
                    }
                    break;
                case Enum_ItemSlotType.Craft:
                    CraftSets[SlotNum] = new Vector2Int(citem.ItemGID, citem.SlotNum);

                    break;
                case Enum_ItemSlotType.ShortCut:
                    if (citem.SlotType == Enum_ItemSlotType.ShortCut)
                    {
                        var bgid = lchara.ShortCutSets[SlotNum];
                        lchara.ShortCutSets[SlotNum] = citem.ItemGID;
                        lchara.ShortCutSets[citem.SlotNum] = bgid;
                    }
                    else lchara.ShortCutSets[SlotNum] = citem.ItemGID;
                    break;
                case Enum_ItemSlotType.BotSet:
                    if (citem.SlotType == Enum_ItemSlotType.BotSet)
                    {
                        var bgid = lchara.BotSets[SlotNum];
                        lchara.BotSets[SlotNum] = citem.ItemGID;
                        lchara.BotSets[citem.SlotNum] = bgid;
                    }
                    else lchara.BotSets[SlotNum] = citem.ItemGID;
                    break;
                case Enum_ItemSlotType.Has_Wepon:
                    if (citem.SlotType == Enum_ItemSlotType.Has_Wepon)
                    {
                        var wep = LPlayerVal.Wepons[citem.SlotNum];
                        LPlayerVal.Wepons[citem.SlotNum] = LPlayerVal.Wepons[SlotNum];
                        LPlayerVal.Wepons[SlotNum] = wep;
                    }
                    else if (citem.SlotType == Enum_ItemSlotType.Set_Wepon)
                    {
                        var wep = citem.CharaUI.LChara.SetWepons[citem.SlotNum];
                        citem.CharaUI.LChara.SetWepons[citem.SlotNum] = LPlayerVal.Wepons[SlotNum];
                        LPlayerVal.Wepons[SlotNum] = wep;
                        if (LPlayerVal.Wepons[SlotNum].GID < 0) LPlayerVal.Wepons.RemoveAt(SlotNum);
                    }
                    else Debug.LogWarning("武器じゃない!!!");
                    break;
                case Enum_ItemSlotType.Set_Wepon:
                    if (citem.SlotType == Enum_ItemSlotType.Has_Wepon)
                    {
                        citem.ItemChange(this);
                    }
                    else if (citem.SlotType == Enum_ItemSlotType.Set_Wepon)
                    {
                        var wep = citem.CharaUI.LChara.SetWepons[citem.SlotNum];
                        citem.CharaUI.LChara.SetWepons[citem.SlotNum] = lchara.SetWepons[SlotNum];
                        lchara.SetWepons[SlotNum] = wep;
                    }
                    else Debug.LogWarning("武器じゃない!!!");
                    break;
                case Enum_ItemSlotType.Has_Armor:
                    if (citem.SlotType == Enum_ItemSlotType.Has_Armor)
                    {
                        var arm = LPlayerVal.Armors[citem.SlotNum];
                        LPlayerVal.Armors[citem.SlotNum] = LPlayerVal.Armors[SlotNum];
                        LPlayerVal.Armors[SlotNum] = arm;
                    }
                    else if (citem.SlotType == Enum_ItemSlotType.Set_Armor)
                    {
                        GIDBrake(ItemGID,out var categ,out var type);
                        var check = false;
                        switch (citem.SlotNum)
                        {
                            case 0:
                                if (type == (int)Enum_EquipType.Head) check = true;
                                break;
                            case 1:
                                if (type == (int)Enum_EquipType.Body) check = true;
                                break;
                            case 2:
                                if (type == (int)Enum_EquipType.Hand) check = true;
                                break;
                            case 3:
                                if (type == (int)Enum_EquipType.Foot) check = true;
                                break;
                        }
                        if (!check)
                        {
                            Debug.LogWarning("種類が違う!!!");
                            break;
                        }

                        var arm = citem.CharaUI.LChara.SetArmors[citem.SlotNum];
                        citem.CharaUI.LChara.SetArmors[citem.SlotNum] = LPlayerVal.Armors[SlotNum];
                        LPlayerVal.Armors[SlotNum] = arm;
                        if (LPlayerVal.Armors[SlotNum].GID < 0) LPlayerVal.Armors.RemoveAt(SlotNum);
                    }
                    else Debug.LogWarning("防具じゃない!!!");
                    break;
                case Enum_ItemSlotType.Set_Armor:
                    if (citem.SlotType == Enum_ItemSlotType.Has_Armor)
                    {
                        citem.ItemChange(this);
                    }
                    else if (citem.SlotType == Enum_ItemSlotType.Set_Armor)
                    {
                        GIDBrake(ItemGID, out var categ, out var type);
                        var check = false;
                        switch (citem.SlotNum)
                        {
                            case 0:
                                if (type == (int)Enum_EquipType.Head) check = true;
                                break;
                            case 1:
                                if (type == (int)Enum_EquipType.Body) check = true;
                                break;
                            case 2:
                                if (type == (int)Enum_EquipType.Hand) check = true;
                                break;
                            case 3:
                                if (type == (int)Enum_EquipType.Foot) check = true;
                                break;
                        }
                        if (!check)
                        {
                            Debug.LogWarning("種類が違う!!!");
                            break;
                        }

                        var arm = citem.CharaUI.LChara.SetArmors[citem.SlotNum];
                        citem.CharaUI.LChara.SetArmors[citem.SlotNum] = lchara.SetArmors[SlotNum];
                        lchara.SetArmors[SlotNum] = arm;
                    }
                    else Debug.LogWarning("防具じゃない!!!");
                    break;
                case Enum_ItemSlotType.Has_Akuse:
                    if (citem.SlotType == Enum_ItemSlotType.Has_Akuse)
                    {
                        var aku = LPlayerVal.Akuses[citem.SlotNum];
                        LPlayerVal.Akuses[citem.SlotNum] = LPlayerVal.Akuses[SlotNum];
                        LPlayerVal.Akuses[SlotNum] = aku;
                    }
                    else if (citem.SlotType == Enum_ItemSlotType.Set_Akuse)
                    {
                        GIDBrake(ItemGID, out var categ, out var type);
                        var check = false;
                        switch (citem.SlotNum)
                        {
                            case 0:
                                if (type == (int)Enum_EquipType.Earrings) check = true;
                                break;
                            case 1:
                                if (type == (int)Enum_EquipType.Necklace) check = true;
                                break;
                            case 2:
                            case 3:
                                if (type == (int)Enum_EquipType.Ring) check = true;
                                break;
                        }
                        if (!check)
                        {
                            Debug.LogWarning("種類が違う!!!" + citem.SlotNum + "|" + type);
                            break;
                        }

                        var aku = lchara.SetAkuses[citem.SlotNum];
                        lchara.SetAkuses[citem.SlotNum] = LPlayerVal.Akuses[SlotNum];
                        LPlayerVal.Akuses[SlotNum] = aku;
                        if (LPlayerVal.Akuses[SlotNum].GID < 0) LPlayerVal.Akuses.RemoveAt(SlotNum);
                    }
                    else Debug.LogWarning("アクセサリーじゃない!!!");
                    break;
                case Enum_ItemSlotType.Set_Akuse:
                    if (citem.SlotType == Enum_ItemSlotType.Has_Akuse)
                    {
                        citem.ItemChange(this);
                    }
                    else if (citem.SlotType == Enum_ItemSlotType.Set_Akuse)
                    {
                        GIDBrake(ItemGID, out var categ, out var type);
                        var check = false;
                        switch (citem.SlotNum)
                        {
                            case 0:
                                if (type == (int)Enum_EquipType.Earrings) check = true;
                                break;
                            case 1:
                                if (type == (int)Enum_EquipType.Necklace) check = true;
                                break;
                            case 2:
                            case 3:
                                if (type == (int)Enum_EquipType.Ring) check = true;
                                break;
                        }
                        if (!check)
                        {
                            Debug.LogWarning("種類が違う!!!" + citem.SlotNum + "|" + type);
                            break;
                        }

                        var aku = lchara.SetAkuses[citem.SlotNum];
                        lchara.SetAkuses[citem.SlotNum] = lchara.SetAkuses[SlotNum];
                        lchara.SetAkuses[SlotNum] = aku;
                    }
                    else Debug.LogWarning("アクセサリーじゃない!!!");
                    break;
                case Enum_ItemSlotType.Wepon_Skin:
                    lchara.WeponSkin[SlotNum] = citem.ItemGID;
                    break;
            }

        }
        public void OtherDrop()
        {
            if (ItemGID < 0) return;
            var lchara = CharaUI.LChara;
            switch (SlotType)
            {
                case Enum_ItemSlotType.UISlot:
                    SlotNum = -1;
                    break;
                case Enum_ItemSlotType.ShortCut:
                    lchara.ShortCutSets[SlotNum] = -1;
                    break;
                case Enum_ItemSlotType.BotSet:
                    lchara.BotSets[SlotNum] = -1;
                    break;
                case Enum_ItemSlotType.Skill_All:
                case Enum_ItemSlotType.Skill_Common:
                case Enum_ItemSlotType.Skill_Job1:
                case Enum_ItemSlotType.Skill_Job2:
                    break;
                case Enum_ItemSlotType.Set_Wepon:
                    LPlayerVal.Wepons.Add(lchara.SetWepons[SlotNum]);
                    lchara.SetWepons[SlotNum] = new Class_State_EquipmentValues { GID = -1 };
                    break;
                case Enum_ItemSlotType.Set_Armor:
                    LPlayerVal.Armors.Add(lchara.SetArmors[SlotNum]);
                    lchara.SetArmors[SlotNum] = new Class_State_EquipmentValues { GID = -1 };
                    break;
                case Enum_ItemSlotType.Set_Akuse:
                    LPlayerVal.Akuses.Add(lchara.SetAkuses[SlotNum]);
                    lchara.SetAkuses[SlotNum] = new Class_State_EquipmentValues { GID = -1 };
                    break;
                case Enum_ItemSlotType.Wepon_Skin:
                    lchara.WeponSkin[SlotNum] = -1;
                    break;
                default:
                    MyPlayer.ItemGIDDrop(ItemGID, SlotNum);
                    break;
            }
        }

        public void LateUpdate()
        {
            if (ItemGID >= 0 && CountTx != null) ChangeText(CountTx,ItemCountLVStr);
            switch (SlotType)
            {
                case Enum_ItemSlotType.ShortCut:
                    ChangeColor(Flame,OnColors(PCont.Sel_ShortCutSet == (SlotNum % 10)));
                    break;
                case Enum_ItemSlotType.Skill_All:
                case Enum_ItemSlotType.Skill_Common:
                case Enum_ItemSlotType.Skill_Job1:
                case Enum_ItemSlotType.Skill_Job2:
                    var SCheck = SkillCheck(GetSkillAtk, CharaUI.LChara);
                    ChangeColor(Flame, SCheck ? Color.white : new Color(1, 0.5f, 0.5f));
                    break;
            }

            if (bGID == ItemGID) return;
            bGID = ItemGID;
            if (NumTx != null)
            {
                switch (SlotType)
                {
                    default:
                        ChangeText(NumTx, SlotNum.ToString());
                        break;
                    case Enum_ItemSlotType.ShortCut:
                        ChangeText(NumTx, ((SlotNum + 1) % 10).ToString());
                        break;
                    case Enum_ItemSlotType.BotSet:
                        ChangeText(NumTx, SlotNum.ToString("D2"));
                        break;
                }

            }
            if (ItemGID >= 0)
            {
                ChangeTexture(Icon, SlotGIDD.Icon, true);
                ChangeText(NameTx, SlotGIDD.Icon != null ? "" : SlotGIDD.Name);

                if (InfoTx != null)ChangeText( InfoTx,SlotGIDD.Info);
            }
            else
            {
                ChangeColor(Icon,Color.clear);
                ChangeText(NameTx,"");

                if (CountTx != null) ChangeText(CountTx,"");
                if (InfoTx != null) ChangeText(InfoTx,"");
            }
        }
        public void SkillInput()
        {
            PCont.In_ShortCutSet = (SlotNum % 10)+ 1;
            PCont.Sel_ShortCutSet = SlotNum % 10;
        }
    }
}
