namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static UI_System;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    using static GmSystem.GS_EnumToJpString;
    using static GmSystem.GS_ChangeSet;
    public class UI_Craft_Adds_Main : MonoBehaviour
    {
        [SerializeField] UI_ItemSlotBase ItemSlotEquip;
        [SerializeField] TextMeshProUGUI NameTx;
        [SerializeField] TextMeshProUGUI AddCostTx;
        [SerializeField] TextMeshProUGUI ChangeCostTx;
        [SerializeField] List<UI_Craft_Adds_Single> AddUIs;
        private void LateUpdate()
        {
            ChangeText(NameTx,ItemSlotEquip.SlotGIDD != null ? ItemSlotEquip.SlotGIDD.Name : "無");
            var equVal = ItemSlotEquip.ItemEquipVal;
            if(equVal != null)
            {
                ChangeText(AddCostTx,ValueStrings(AddCost) + "ゴールド");
                ChangeText(ChangeCostTx,ValueStrings(ChangeCost) + "ゴールド");
                for (int i = 0; i < Mathf.Max(equVal.AddOps.Count, AddUIs.Count); i++)
                {
                    if (i >= AddUIs.Count)
                    {
                        AddUIs.Add(Instantiate(AddUIs[0], AddUIs[0].transform.parent));
                    }
                    var aui = AddUIs[i];
                    if (i >= equVal.AddOps.Count)
                    {
                        ChangeActive(aui.gameObject, false);
                        continue;
                    }

                    var addop = equVal.AddOps[i];
                    ChangeActive(aui.gameObject, true);
                    aui.ID = i;
                    ChangeText(aui.NumTx, "[" + (i + 1) + "]");
                    ChangeText(aui.StaTx,EnumToJp(addop.State));
                    ChangeText(aui.OpTx,EnumToJp(addop.Option));
                    ChangeText(aui.LvTx,addop.LV + "LV");
                    var vals = DB.OpValues.GetAddValues(addop.State, addop.Option);
                    var valstr = ValueStrings(DB.OpValues.GetValue(addop.State, addop.Option, addop.LV));
                    if(vals!=null)valstr +="(" + vals.Value.x + "+" + vals.Value.y + "×Lv)";
                    ChangeText(aui.ValTx, valstr);
                    ChangeText(aui.CostTx,ValueStrings(LvCost(i)) + "ゴールド");
                    ChangeOn(aui.LockTo,addop.Lock);
                }
            }
            else
            {
                for (int i = 0; i < AddUIs.Count; i++)
                {
                    ChangeActive(AddUIs[i].gameObject, false);
                }
                ChangeText(AddCostTx,"0ゴールド");
                ChangeText(ChangeCostTx,"0ゴールド");
            }
        }
        float AddCost
        {
            get
            {
                var equVal = ItemSlotEquip.ItemEquipVal;
                if (equVal == null) return 0;
                return equVal.AddOps.Count * 1000;
            }
        }
        float ChangeCost
        {
            get
            {
                var equVal = ItemSlotEquip.ItemEquipVal;
                if (equVal == null) return 0;
                var lco = 0;
                for(int i = 0; i < equVal.AddOps.Count; i++)
                {
                    if (equVal.AddOps[i].Lock) lco++;
                }
                return (1 + lco * 3) * 1000;
            }
        }
        float LvCost(int i)
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            if (equVal == null) return 0;
            return equVal.AddOps[i].LV * 250;
        }
        public void LvAdd(int i)
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            if (equVal == null) return;
            if (LPlayerVal.Gold < LvCost(i)) return;
            LPlayerVal.Gold -= LvCost(i);
            equVal.AddOps[i].LV++;
        }
        public void LockSet(int i,bool lk)
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            if (equVal == null) return;
            equVal.AddOps[i].Lock = lk;
        }
        public void Adds()
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            if (equVal == null) return;
            if (LPlayerVal.Gold < AddCost) return;
            LPlayerVal.Gold -= AddCost;
            RandOps(out var type, out var op);
            equVal.AddOps.Add(new Datas.Data_Equips.Class_EquipmentAddOp
            {
                LV = 1,
                State = type,
                Option = op,
            });
        }
        public void Change()
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            if (equVal == null) return;
            if (LPlayerVal.Gold < ChangeCost) return;
            LPlayerVal.Gold -= ChangeCost;
            for (int i = 0; i < equVal.AddOps.Count; i++)
            {
                var addop = equVal.AddOps[i];
                if (addop.Lock) continue;
                RandOps(out var type, out var op);
                addop.State = type;
                addop.Option = op;
            }
        }
        void RandOps(out Enum_StateAddsType type,out Enum_StateAddsOption op)
        {
            type = Enum_StateAddsType.MaxHP;
            op = Enum_StateAddsOption.BaseConst;
            switch (ItemGIDCategoryGet(ItemSlotEquip.ItemGID))
            {
                case Datas.Data_Items.Enum_ItemID.Wepon:
                    DB.WeponOpRand.RandOp(out type, out op);
                    break;
                case Datas.Data_Items.Enum_ItemID.Armor:
                    DB.ArmorOpRand.RandOp(out type, out op);
                    break;
                case Datas.Data_Items.Enum_ItemID.Akuse:
                    DB.AkuseOpRand.RandOp(out type, out op);
                    break;
            }
        }
    }
}
