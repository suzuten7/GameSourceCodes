namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_SaveValues;
    public class UI_Acive_Create : MonoBehaviour
    {
        public TextMeshProUGUI CountTx;
        public TMP_Dropdown LockDr;
        public TMP_Dropdown TypeDr;
        public List<UI_Acive_Single> SinUIs;
        private void LateUpdate()
        {
            var acvs = new List<string>();
            for (int i = 0; i < DB.Acives.DBL.Count; i++)
            {
                for (int k = 0; k < DB.Acives.DBL[i].Datas.Count; k++)
                {
                    acvs.Add(DB.Acives.DBL[i].Datas[k].AciveKey);
                }
            }

            int co = 0;
            for(int i = 0; i < acvs.Count; i++)
            {
                if (i >= SinUIs.Count) SinUIs.Add(Instantiate(SinUIs[0], SinUIs[0].transform.parent));
                var sui = SinUIs[i];
                var data = DB.Acives.KeyGetDataID(acvs[i]).Item1;
                var acv = AciveGet(acvs[i],false);
                if (acv != null && (acv.Get == 1 || acv.Get == 2)) co++;
                var view = true;
                switch (LockDr.value)
                {
                    case 1:
                        if (acv != null && (acv.Get == 1 || acv.Get == 2)) view = false;
                        break;
                    case 2:
                        if (acv == null || (acv.Get != 1 && acv.Get != 2)) view = false;
                        break;
                }
                if (TypeDr.value > 0 && (TypeDr.value - 1) != (int)data.Type) view = false;
                ChangeActive(sui.gameObject, view);
                if (!view) continue;
                sui.ID = acvs[i];
                ChangeText(sui.Name,data.Name);
                ChangeTexture(sui.Icon, data.Icon, true);
                switch (acv != null ? acv.Get : 0)
                {
                    default: ChangeColor(sui.OutIm, Color.red); break;
                    case 1: ChangeColor(sui.OutIm, Color.yellow); break;
                    case 2: ChangeColor(sui.OutIm, Color.blue); break;
                }
            }
            ChangeText(CountTx, co + "/" + acvs.Count);
        }
    }
}

