namespace UIs
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using static UIs.UI_System;
    using System.Collections.Generic;
    using Datas;

    public class UI_DebugEnemy_Create : MonoBehaviour
    {
        public int ID;
        [SerializeField]TextMeshProUGUI NameTx;
        [SerializeField] TextMeshProUGUI InfoTx;
        [SerializeField] RawImage Icon;
        [SerializeField] TMP_Dropdown TypeDr;
        [SerializeField] List<UI_DebugEnemy_Single> SinUIs;
        [SerializeField] TMP_InputField LvIn;
        private void LateUpdate()
        {
            var Enemys = new List<int>();
            for(int i = 0; i < DB.Enemys.DBL.Count; i++)
            {
                for(int k = 0; k < DB.Enemys.DBL[i].Datas.Count; k++)
                {
                    Enemys.Add((int)DB.Enemys.DBL[i].Type * Data_Enemy_DBL.TypeSize + k);
                }
            }

            for(int i = 0; i < Mathf.Max(Enemys.Count, SinUIs.Count); i++)
            {
                if (i >= SinUIs.Count) SinUIs.Add(Instantiate(SinUIs[0], SinUIs[0].transform.parent));
                var uis = SinUIs[i];
                var ed = DB.Enemys.IDGetData(Enemys[i]);
                var view = TypeDr.value == 0 || ((int)ed.Type == TypeDr.value - 1);
                ChangeActive(uis.gameObject, view);
                if (!view) continue;

                uis.ID = Enemys[i];
                ChangeColor(uis.SelUI, OnColors(uis.ID == ID));
                ChangeText(uis.NameTx, ed.Name);
                ChangeTexture(uis.Icon, ed.Icon,true);
            }
            var edata = DB.Enemys.IDGetData(ID);
            ChangeText(NameTx, edata.Name);
            ChangeText(InfoTx, edata.Info);
            ChangeTexture(Icon, edata.Icon,true);
        }
        public void Summons()
        {
            NetValue.Rpc_Summon(ID, int.TryParse(LvIn.text, out var olv) ? olv : 0, MyPlayer.PosGet, MyPlayer.RotGet);
        }
    }
}

