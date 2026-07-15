namespace UIs
{
    using System.Collections.Generic;
    using UnityEngine;
    using static UI_System;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static Datas.Data_Get;
    using TMPro;
    using static GmSystem.GS_ChangeSet;
    public class UI_InventryCreate : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        public Enum_ItemSlotType ItemType;
        [SerializeField] List<UI_ItemSlotBase> ItemSlots;
        [SerializeField] TextMeshProUGUI CountTxs;
        [SerializeField] TMP_InputField SerchTx;
        private void Update()
        {
            if (MyPlayer == null) return;
            var count = 0;
            var sta = CharaUI.Sta;
            switch (ItemType)
            {
                case Enum_ItemSlotType.Shop:count = CurrentShop != null ? CurrentShop.ShopD.Buys.Count : 0;break;
                case Enum_ItemSlotType.WeponUI: count = 2; break;
                case Enum_ItemSlotType.ShortCut: count = 10; break;
                case Enum_ItemSlotType.BotSet: count = 20; break;
                case Enum_ItemSlotType.Material: count = LPlayerVal.ItemsDic.Count; break;
                case Enum_ItemSlotType.Consumables: count = LPlayerVal.ConsumablesDic.Count; break;
                case Enum_ItemSlotType.Has_Wepon: count = LPlayerVal.Wepons.Count; break;
                case Enum_ItemSlotType.Has_Armor: count = LPlayerVal.Armors.Count; break;
                case Enum_ItemSlotType.Has_Akuse: count = LPlayerVal.Akuses.Count; break;
                case Enum_ItemSlotType.Skill_All:
                case Enum_ItemSlotType.Skill_Common:
                case Enum_ItemSlotType.Skill_Job1:
                case Enum_ItemSlotType.Skill_Job2:
                    count = GetAttackList(ItemType, CharaUI.LChara).Count;
                    break;
            }
            var dispCount = 0;
            for (int i = 0; i < Mathf.Max(ItemSlots.Count, count); i++)
            {
                if (ItemSlots.Count <= i) ItemSlots.Add(Instantiate(ItemSlots[0], ItemSlots[0].transform.parent));
                var isl = ItemSlots[i];
                if (i >= count)
                {
                    ChangeActive(isl.gameObject, false);
                    continue;
                }

                isl.SlotType = ItemType;
                switch (ItemType)
                {
                    default:
                        isl.SlotNum = i;
                        break;
                    case Enum_ItemSlotType.WeponUI:
                        isl.SlotNum = (!sta.PlayerValues.WepBack ? 0 : 2) + i;
                        isl.SlotType = Enum_ItemSlotType.Set_Wepon;
                        break;
                    case Enum_ItemSlotType.ShortCut:
                        isl.SlotNum = (!CharaUI.Sta.PlayerValues.ShortCutBack ? 0 : 10) + i;
                        break;
                }

                if (SerchTx != null && SerchTx.text != "")
                {
                    var ItemData = ItemGIDDataGet(isl.ItemGID);
                    if (!ItemData.Name.Contains(SerchTx.text) && !ItemData.Info.Contains(SerchTx.text))
                    {
                        ChangeActive(isl.gameObject, false);
                        continue;
                    }
                }
                ChangeActive(isl.gameObject, true);

                dispCount++;
            }
            if (CountTxs != null)ChangeText(CountTxs,"検索数:" + dispCount + "/" + count) ;
        }
        public void SlotChange(int Type)
        {
            ItemType = (Enum_ItemSlotType)Type;
        }

    }
}
