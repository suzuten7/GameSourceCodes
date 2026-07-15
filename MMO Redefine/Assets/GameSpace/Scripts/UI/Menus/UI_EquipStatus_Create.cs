namespace UIs
{
    using Datas;
    using System.Collections.Generic;
    using UnityEngine;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static GmSystem.GS_GlobalState;
    using static UI_System;
    using static GmSystem.GS_ChangeSet;
    public class UI_EquipStatus_Create : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] List<UI_EquipStatus_Single> StatusUIs;
        List<Class_EquipmentAdds> EquLists = new ();
        private void LateUpdate()
        {
            EquLists.Clear();
            var PVal = CharaUI.LChara;
            for (int i = 0; i < PVal.SetWepons.Length; i++)
            {
                var WepEqVal = PVal.SetWepons[i];
                var WepData = (Data_Wepon)ItemGIDDataGet(WepEqVal.GID);
                if (WepData == null) continue;
                ListAddAdds(WepData.EquipmentAdds, WepEqVal.LV);
                ListAddOps(WepEqVal.AddOps);
            }
            for (int i = 0; i < PVal.SetArmors.Length; i++)
            {
                var ArmEqVal = PVal.SetArmors[i];
                var ArmData = (Data_Equipment)ItemGIDDataGet(ArmEqVal.GID);
                if (ArmData == null) continue;
                ListAddAdds(ArmData.EquipmentAdds, ArmEqVal.LV);
                ListAddOps(ArmEqVal.AddOps);
            }
            for (int i = 0; i < PVal.SetAkuses.Length; i++)
            {
                var AkuEqVal = PVal.SetAkuses[i];
                var AkuData = (Data_Equipment)ItemGIDDataGet(AkuEqVal.GID);
                if (AkuData == null) continue;
                ListAddAdds(AkuData.EquipmentAdds, AkuEqVal.LV);
                ListAddOps(AkuEqVal.AddOps);
            }

            EquipSort(EquLists);
            for (int i = 0; i < Mathf.Max(StatusUIs.Count, EquLists.Count); i++)
            {
                if (StatusUIs.Count <= i) StatusUIs.Add(Instantiate(StatusUIs[0], StatusUIs[0].transform.parent));
                var sui = StatusUIs[i];
                if (i >= EquLists.Count)
                {
                    ChangeActive(sui.gameObject, false);
                    continue;
                }
                ChangeActive(sui.gameObject, true);
                EquipStrs(EquLists[i],-1, out var oName, out var oOp, out var oVal, out var oIf);
                ChangeText(sui.Name,oName);
                ChangeText(sui.OP,oOp);
                ChangeText(sui.Val,oVal);
                ChangeText(sui.IFs,oIf);
            }
        }

        void ListAddAdds(Class_EquipmentAdds[] Equips, int LV)
        {
            for (int i = 0; i < Equips.Length; i++)
            {
                Add(Equips[i].State, Equips[i].Option, Equips[i].EquipIfs, Equips[i].Values.x + Equips[i].Values.y * (LV - 1));
            }
        }
        void ListAddOps(List<Class_EquipmentAddOp> Ops)
        {
            for (int i = 0; i < Ops.Count; i++)
            {
                Add(Ops[i].State, Ops[i].Option, null, 1f + Ops[i].LV * 0.1f);
            }
        }
        void Add(Enum_StateAddsType state,Enum_StateAddsOption option, Class_EquipmentIf[] ifs,float val)
        {
            if (option != Enum_StateAddsOption.FinalRate && (ifs==null || ifs.Length <= 0))
            {
                var FindList = EquLists.Find(x => x.State == state && x.Option == option && x.EquipIfs.Length <= 0);
                if (FindList != null)
                {
                    FindList.Values.x += val;
                    return;
                }
            }
            EquLists.Add(
                new Class_EquipmentAdds
                {
                    State = state,
                    Option = option,
                    EquipIfs = ifs ?? (new Class_EquipmentIf[0]),
                    Values = new Vector2(val, 0),
                }
                );
        }
    }
}
