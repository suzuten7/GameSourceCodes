namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    public class UI_Skill : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] TextMeshProUGUI[] JobTxs;
        void Update()
        {
            for(int i = 0; i < JobTxs.Length; i++)
            {
                ChangeText(JobTxs[i], DB.JobDatas[CharaUI.LChara.JobIDs[i]].Name);
            }
        }
    }
}

