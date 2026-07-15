namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    using static UIs.UI_System;
    public class UI_MyState_Sets_Job_Make : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        public int SelectID = 0;
        public TextMeshProUGUI Job1Tx;
        public TextMeshProUGUI Job2Tx;
        public List<UI_MyState_Sets_Job_Single> Singles;

        public TextMeshProUGUI JobInfoTx;
        void Update()
        {
            ChangeText(Job1Tx,"ジョブ1:" + DB.JobDatas[CharaUI.LChara.JobIDs[0]].Name);
            ChangeText(Job2Tx,"ジョブ2:" + DB.JobDatas[CharaUI.LChara.JobIDs[1]].Name);
            for (int i = 0; i < DB.JobDatas.Length; i++)
            {
                if (i >= Singles.Count) Singles.Add(Instantiate(Singles[0], Singles[0].transform.parent));
                var sui = Singles[i];
                sui.ID = i;
                ChangeColor(sui.BackImage,OnColors(i == SelectID));
                ChangeText(sui.NameTx, DB.JobDatas[i].Name);
            }
            var SJobD = DB.JobDatas[SelectID];
            var infostr = SJobD.Name + "\n" + SJobD.Info;

            infostr += "\n<size=150%>=== 基礎 ===</size>";
            infostr += "\n最大HP:" + SJobD.MHP;
            infostr += "\nHP回復速度" + SJobD.HPRegene;
            infostr += "\n最大MP" + SJobD.MMP;
            infostr += "\nMP回復速度" + SJobD.MPRegene;
            infostr += "\n最大スタミナ" + SJobD.MST;
            infostr += "\nスタミナ回復速度" + SJobD.STRegene;
            infostr += "\n物理攻撃力:" + SJobD.PAtk;
            infostr += "\n魔法攻撃力:" + SJobD.MAtk;
            infostr += "\n物理防御力:" + SJobD.PDef;
            infostr += "\n魔法防御力:" + SJobD.MDef;

            infostr += "\n<size=150%>=== 補正 ===</size>";
            if (SJobD.Adds.Length <= 0) infostr += "\n無し";
            for (int i = 0; i < SJobD.Adds.Length; i++)
            {
                EquipStrs(SJobD.Adds[i],CharaUI.LChara.LV, out var oName, out var oOP, out var oVal, out var oIf);
                if (oIf != "") infostr += "\n条件" + oIf;
                infostr += "\n" + oName + ":" + oOP + ":" + oVal;
            }

            ChangeText(JobInfoTx, infostr);

        }
        public void SetJob(int i)
        {
            CharaUI.LChara.JobIDs[i] = (byte)SelectID;
        }
    }
}
