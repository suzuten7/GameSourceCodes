namespace UIs
{
    using Datas;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_SaveValues;
    using static UI_System;
    public class UI_Craft_Upgrade_Main : MonoBehaviour
    {
        [SerializeField] UI_ItemSlotBase ItemSlotEquip;
        [SerializeField] TextMeshProUGUI LvNameTx;
        [SerializeField] Image ExpFill;
        [SerializeField] TextMeshProUGUI ExpTx;
        [SerializeField] TextMeshProUGUI EquipStateTx;
        [SerializeField] List<UI_Craft_Upgrade_Adds> UpgradeAddUIs;
        [SerializeField] UI_ItemSlotBase ItemSlotMaterial;
        [SerializeField] TextMeshProUGUI CountExpTx;
        [SerializeField] Slider CountSlider;
        [SerializeField] TMP_InputField CountIn;
        int count = 1;
        private void LateUpdate()
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            var ItemD = ItemSlotEquip.SlotGIDD;
            string lvnamestr;
            if (ItemSlotEquip.SlotGIDD != null) lvnamestr = "「" + ItemD.Name + "」";
            else lvnamestr = "無";
            if (equVal != null)
            {
                lvnamestr = "Lv" + equVal.LV + LvNameTx.text;
                ChangeFill(ExpFill,equVal.Exp / Mathf.Max(1, equVal.NextExp));
                ChangeText(ExpTx,ValueStrings(equVal.Exp) + "/" + ValueStrings(equVal.NextExp));
            }
            else
            {
                ChangeFill(ExpFill,0);
                ChangeText(ExpTx,"-");
            }
            ChangeText(LvNameTx,lvnamestr);
            Class_EquipmentAdds[] EquAdds = null;
            Class_UpgradeBoost[] Boosts = null;
            switch (ItemGIDCategoryGet(ItemSlotEquip.ItemGID))
            {
                case Data_Items.Enum_ItemID.Wepon:
                    EquAdds = ((Data_Wepon)ItemD).EquipmentAdds;
                    Boosts = ((Data_Wepon)ItemD).UpgradeBoosts;
                    break;
                case Data_Items.Enum_ItemID.Armor:
                case Data_Items.Enum_ItemID.Akuse:
                    EquAdds = ((Data_Equipment)ItemD).EquipmentAdds;
                    Boosts = ((Data_Equipment)ItemD).UpgradeBoosts;
                    break;
            }
            var eqtx = "";
            if (EquAdds != null)
            {
                var EquLists = EquAdds.ToList();
                EquipSort(EquLists);
                for (int i = 0; i < EquLists.Count; i++)
                {
                    EquipStrs(EquLists[i],equVal != null ? equVal.LV : 0, out var oName, out var oOp, out var oVal, out var oIf);
                    eqtx += "\n";
                    if (oIf != "") eqtx += oIf + "\n";
                    eqtx += oName + oOp + oVal;
                }
            }
            ChangeText(EquipStateTx, eqtx);
            for(int i = 0;i<Mathf.Max(Boosts != null ? Boosts.Length : 0, UpgradeAddUIs.Count); i++)
            {
                if (i >= UpgradeAddUIs.Count)
                {
                    UpgradeAddUIs.Add(Instantiate(UpgradeAddUIs[0], UpgradeAddUIs[0].transform.parent));
                }
                var uaui = UpgradeAddUIs[i];
                if (Boosts == null || i >= Boosts.Length)
                {
                    ChangeActive(uaui.gameObject,false);
                    continue;
                }
                var boost = Boosts[i];
                ChangeActive(uaui.gameObject, true);
                ChangeTexture(uaui.Icon,boost.ItemD.Icon, true);
                ChangeText(uaui.Name, boost.ItemD.Name);
                ChangeText(uaui.BoostTx,"+" + boost.ExpAddPer + "%");

            }
            var MaterialD = ItemSlotMaterial.SlotGIDD;
            if (MaterialD != null)
            {
                var boost = BoostGet;
                var exptx = "Exp+" + ValueStrings(AddExp * boost);
                exptx += "\nExp+" + ValueStrings(((Data_Item)MaterialD).UpgradeExp * boost) + "/個";
                ChangeText(CountExpTx, exptx);
                ChangeColor(CountExpTx,boost > 1f ? Color.yellow : Color.white);
            }
            else
            {
                ChangeText(CountExpTx,"Exp+0\nExp+0/個");
                ChangeColor(CountExpTx,Color.white);
            }
            ChangeMax(CountSlider,Mathf.Max(1, ItemSlotMaterial.ItemCount));
            ChangeValue(CountSlider,count);
            ChangeText(CountIn,count.ToString(),true);
        }
        float AddExp
        {
            get
            {
                var MaterialD = ItemSlotMaterial.SlotGIDD;
                if(MaterialD != null)
                {
                    return ((Data_Item)MaterialD).UpgradeExp * count;
                }
                return 0;
            }
        }
        float BoostGet
        {
            get
            {
                var MaterialD = ItemSlotMaterial.SlotGIDD;
                if (MaterialD != null)
                {
                    var ItemD = ItemSlotEquip.SlotGIDD;
                    Class_UpgradeBoost[] Boosts = null;
                    switch (ItemGIDCategoryGet(ItemSlotEquip.ItemGID))
                    {
                        case Data_Items.Enum_ItemID.Wepon:
                            Boosts = ((Data_Wepon)ItemD).UpgradeBoosts;
                            break;
                        case Data_Items.Enum_ItemID.Armor:
                        case Data_Items.Enum_ItemID.Akuse:
                            Boosts = ((Data_Equipment)ItemD).UpgradeBoosts;
                            break;
                    }
                    if (Boosts != null)
                        for (int i = 0; i < Boosts.Length; i++)
                        {
                            if (Boosts[i].ItemD == MaterialD)
                            {
                                return 1f + Boosts[i].ExpAddPer * 0.01f;
                            }
                        }
                }
                return 1f;
            }
        }
        public void Upgrades()
        {
            var equVal = ItemSlotEquip.ItemEquipVal;
            if (equVal != null)
            {
                equVal.Exp += AddExp * BoostGet;
                for (int i = 0; i < 100; i++)
                {
                    if (equVal.Exp < equVal.NextExp) break;
                    equVal.Exp -= equVal.NextExp;
                    equVal.LV++;
                }
            }
            switch (ItemGIDCategoryGet(ItemSlotMaterial.ItemGID))
            {
                case Data_Items.Enum_ItemID.Wepon:
                case Data_Items.Enum_ItemID.Armor:
                case Data_Items.Enum_ItemID.Akuse:
                    LPlayerVal.ItemRem(ItemSlotMaterial.ItemGID, ItemSlotMaterial.SlotNum);
                    break;
                default:
                    LPlayerVal.ItemRem(ItemSlotMaterial.ItemGID, count);
                    break;
            }
            ItemSlotMaterial.USID = -1;
        }
        public void CountSliderChange()
        {
            count = (int)CountSlider.value;
        }
        public void CountInChange()
        {
            count = int.TryParse(CountIn.text, out var val) ? val : count;
            count = Mathf.Clamp(count,1, ItemSlotMaterial.ItemCount);
        }
    }
}

