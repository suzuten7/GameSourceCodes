namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static UI_System;
    using static GmSystem.GS_SaveValues;
    using System.Linq;
    using System;

    public class UI_InventrySort : MonoBehaviour
    {
        [SerializeField] UI_InventryCreate Inv;
        [SerializeField] TMP_Dropdown SortDr;
        Enum_ItemSlotType itemType;
        private void Start()
        {
            DrSet();
        }
        private void Update()
        {
            if (itemType != Inv.ItemType)
            {
                itemType = Inv.ItemType;
                DrSet();
            }
        }
        void DrSet()
        {
            var Options = new List<string> { "ソート", "ID" };
            switch (Inv.ItemType)
            {
                case Enum_ItemSlotType.Material:
                case Enum_ItemSlotType.Consumables:
                    Options.Add("所持数");
                    break;
                case Enum_ItemSlotType.Has_Wepon:
                case Enum_ItemSlotType.Has_Armor:
                case Enum_ItemSlotType.Has_Akuse:
                    Options.Add("LV");
                    Options.Add("入手順");
                    break;

            }
            SortDr.ClearOptions();
            for (int i = 0; i < Options.Count; i++)
            {
                SortDr.AddOptions(new List<TMP_Dropdown.OptionData> { new (Options[i]) });
            }
            SortDr.value = 0;
        }
        public void DrChange()
        {
            switch (Inv.ItemType)
            {
                case Enum_ItemSlotType.Material: SortItems(ref LPlayerVal.ItemsDic); break;
                case Enum_ItemSlotType.Consumables: SortItems(ref LPlayerVal.ConsumablesDic); break;
                case Enum_ItemSlotType.Has_Wepon: SortEquips(ref LPlayerVal.Wepons); break;
                case Enum_ItemSlotType.Has_Armor: SortEquips(ref LPlayerVal.Armors); break;
                case Enum_ItemSlotType.Has_Akuse: SortEquips(ref LPlayerVal.Akuses); break;
            }
        }
        void SortItems(ref Dictionary<int, int> Dic)
        {
            switch (SortDr.value)
            {
                case 1:
                    var LKDic = Dic.ToList().OrderBy(x => x.Key).ToList();
                    Dic = LKDic.ToDictionary(x => x.Key, x => x.Value);
                    break;
                case 2:
                    var LVDic = Dic.ToList().OrderByDescending(x => x.Value).ToList();
                    Dic = LVDic.ToDictionary(x => x.Key, x => x.Value);
                    break;
            }
        }
        void SortEquips(ref List<Datas.Data_Equips.Class_State_EquipmentValues> Equips)
        {
            switch (SortDr.value)
            {
                case 1: Equips = Equips.OrderBy(x => x.GID).ToList(); break;
                case 2: Equips = Equips.OrderByDescending(x => x.LV).ToList(); break;
                case 3: Equips = Equips.OrderBy(x => x.GetDateTime).ToList(); break;
            }
        }
    }
}

