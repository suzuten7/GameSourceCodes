namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_ChangeSet;
    using Unity.AppUI.UI;
    using static UIs.UI_System;
    public class UI_DebugItem_Create : MonoBehaviour
    {
        [SerializeField] Enum_ItemID ItemType;
        [SerializeField] List<UI_DebugItem_Single> SingleUIs;
        [SerializeField] TMP_InputField StrData;
        [SerializeField] TextMeshProUGUI CountTxs;
        [SerializeField] TMP_InputField SerchTx;
        public int SelectGID;
        public void TypeSet(int i)
        {
            ItemType = (Enum_ItemID)(i * (int)Enum_ItemID.Category);
        }
        public void ItemAdd()
        {
            LPlayerVal.ItemAdd(SelectGID, StrData.text);
        }
        private void LateUpdate()
        {
            var GIDs = new List<int>();
            switch (ItemType)
            {
                case Enum_ItemID.Material:
                    for (int i = 0; i < DB.Items.DBL.Count; i++)
                    {
                        var DBL = DB.Items.DBL[i];
                        for (int j = 0; j < DBL.Datas.Count; j++)
                        {
                            if (DBL.Datas[j] == null) continue;
                            GIDs.Add(GIDMake(Enum_ItemID.Material, (int)DBL.Type, j));
                        }
                    }
                    break;
                case Enum_ItemID.Consumables:
                    for (int i = 0; i < DB.Consumables.DBL.Count; i++)
                    {
                        var DBL = DB.Consumables.DBL[i];
                        for (int j = 0; j < DBL.Datas.Count; j++)
                        {
                            if (DBL.Datas[j] == null) continue;
                            GIDs.Add(GIDMake(Enum_ItemID.Consumables, (int)DBL.Type, j));
                        }
                    }
                    break;
                case Enum_ItemID.Wepon:
                    for (int i = 0; i < DB.Wepons.DBL.Count; i++)
                    {
                        var DBL = DB.Wepons.DBL[i];
                        for (int j = 0; j < DBL.Datas.Count; j++)
                        {
                            if (DBL.Datas[j] == null) continue;
                            GIDs.Add(GIDMake(Enum_ItemID.Wepon, (int)DBL.Type, j));
                        }
                    }
                    break;
                case Enum_ItemID.Armor:
                    for (int i = 0; i < DB.Equipments.DBL.Count; i++)
                    {
                        var DBL = DB.Equipments.DBL[i];
                        for (int j = 0; j < DBL.Datas.Count; j++)
                        {
                            if (DBL.Datas[j] == null) continue;
                            if (DBL.Datas[j].Type >= (int)Enum_EquipType.Earrings) continue;
                            GIDs.Add(GIDMake(Enum_ItemID.Armor, (int)DBL.Type, j));
                        }
                    }
                    break;
                case Enum_ItemID.Akuse:
                    for (int i = 0; i < DB.Equipments.DBL.Count; i++)
                    {
                        var DBL = DB.Equipments.DBL[i];
                        for (int j = 0; j < DBL.Datas.Count; j++)
                        {
                            if (DBL.Datas[j] == null) continue;
                            if (DBL.Datas[j].Type < (int)Enum_EquipType.Earrings) continue;
                            GIDs.Add(GIDMake(Enum_ItemID.Akuse, (int)DBL.Type - (int)Enum_EquipType.Earrings, j));
                        }
                    }
                    break;

            }
            var dispCount = 0;
            for (int i = 0; i < Mathf.Max(SingleUIs.Count, GIDs.Count); i++)
            {
                if (SingleUIs.Count <= i) SingleUIs.Add(Instantiate(SingleUIs[0], SingleUIs[0].transform.parent));
                var sui = SingleUIs[i];
                if (i >= GIDs.Count)
                {
                    ChangeActive(sui.gameObject, false);
                    continue;
                }

                var ItemD = ItemGIDDataGet(GIDs[i]);
                if (SerchTx.text != "")
                {
                    if (!ItemD.Name.Contains(SerchTx.text) && !ItemD.Info.Contains(SerchTx.text))
                    {
                        ChangeActive(sui.gameObject, false);
                        continue;
                    }
                }
                ChangeActive(sui.gameObject, true);
                sui.GID = GIDs[i];
                ChangeTexture(sui.Icon, ItemD.Icon, true);
                ChangeText(sui.Name, ItemD.Icon != null ? "" : ItemD.Name);
                ChangeColor(sui.Flame,OnColors(GIDs[i] == SelectGID));
                dispCount++;
            }
            ChangeText(CountTxs, "検索数:" + dispCount + "/" + GIDs.Count);
        }
    }
}

