namespace UIs
{
    using Datas;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    using static UIs.UI_System;
    using static GmSystem.GS_SaveValues;

    public class UI_Enemys_Make : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI Counts;
        [SerializeField] TextMeshProUGUI Kills;
        public int ID;
        [SerializeField] TextMeshProUGUI Name;
        [SerializeField] TextMeshProUGUI Info;
        [SerializeField] RawImage Icon;
        [SerializeField] TMP_Dropdown LockDr;
        [SerializeField] TMP_Dropdown TypeDr;
        [SerializeField] List<UI_Enemys_Single> SinUIs;
        private void LateUpdate()
        {
            var Enemys = new List<int>();
            for (int i = 0; i < DB.Enemys.DBL.Count; i++)
            {
                for (int k = 0; k < DB.Enemys.DBL[i].Datas.Count; k++)
                {
                    Enemys.Add((int)DB.Enemys.DBL[i].Type * Data_Enemy_DBL.TypeSize + k);
                }
            }
            ChangeText(Counts,GetSave_Enemy.Enemys.Count + "/" + Enemys.Count);
            ChangeText(Kills, "総撃破数" + GetSave_Enemy.Kills.ToString());
            var data = DB.Enemys.IDGetData(ID);
            var svs = GetSave_Enemy.Enemys.Find(x => x.ID == ID);
            ChangeTexture(Icon, data.Icon, false);
            if (svs != null)
            {
                ChangeText(Name, data.Name);
                var infos = data.Info;
                infos += "\n撃破数" + svs.Kills;
                ChangeText(Info, infos);
                ChangeColor(Icon, Color.white);
            }
            else
            {
                ChangeText(Name, "???");
                ChangeText(Info, "");
                ChangeColor(Icon, Color.black);
            }
            for(int i = 0; i < Enemys.Count; i++)
            {
                if (i >= SinUIs.Count) SinUIs.Add(Instantiate(SinUIs[0], SinUIs[0].transform.parent));
                var sui = SinUIs[i];
                var ed = DB.Enemys.IDGetData(Enemys[i]);
                var sv = GetSave_Enemy.Enemys.Find(x => x.ID == Enemys[i]);
                var view = true;
                switch (LockDr.value)
                {
                    case 1: if (sv != null) view = false; break;
                    case 2: if (sv == null) view = false; break;
                }
                if (TypeDr.value > 0 && (int)ed.Type != (TypeDr.value - 1)) view = false;

                ChangeActive(sui.gameObject, view);
                if (!view) continue;

                sui.ID = Enemys[i];
                ChangeColor(sui.SelIm, OnColors(ID == Enemys[i]));
                ChangeTexture(sui.Icon, ed.Icon, false);
                if (sv != null)
                {
                    ChangeText(sui.Name, ed.Name);
                    ChangeColor(sui.Icon, Color.white);
                }
                else
                {
                    ChangeText(sui.Name, "???");
                    ChangeColor(sui.Icon, Color.black);
                }
            }
        }
    }
}

