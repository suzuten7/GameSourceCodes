namespace FNet
{
    using Fusion;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Fusion_Manager;
    using static Fusion_Reliable;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    public class Fusion_Chat : MonoBehaviour
    {
        [SerializeField] TMP_InputField TextIn;
        [SerializeField] TextMeshProUGUI BaseTx;
        [SerializeField] TextMeshProUGUI AddTx;
        List<TextMeshProUGUI> InsBaseTxs = new ();
        List<TextMeshProUGUI> InsAddTxs = new();
        static public List<Class_MessageData> Messages = new ();
        int _mesMode = 0;
        
        private void Start()
        {
            BaseTx.gameObject.SetActive(false);
            AddTx.gameObject.SetActive(false);
        }
        public void SendChat()
        {
            if (TextIn.text == "") return;
            Fusion_Reliable.ChatMessage(Enum_MesID.Chat,InsRunner.GameMode != GameMode.Server ? PlayerName : "サーバー", TextIn.text);
            TextIn.text = "";
        }
        static public void LocalMessage(Enum_MesID AddType,string Base, string Add, int Time = -1)
        {
            if (Time < 0) Time = FixServerTime();
            Time /= 60;
            var TStr = (Time / 3600).ToString("D2") + ":";
            TStr += (Time / 60 % 60).ToString("D2") + ":";
            TStr += (Time % 60).ToString("D2");
            Messages.Add(new Class_MessageData
            {
                Type = AddType,
                Base = "(" + TStr + ")"+Base,
                Add = Add
            });
        }
        private void LateUpdate()
        {
            for (int i = 0; i < Mathf.Max(InsBaseTxs.Count, Messages.Count); i++)
            {
                if (InsBaseTxs.Count <= i) InsBaseTxs.Add(Instantiate(BaseTx, BaseTx.transform.parent));
                if (InsAddTxs.Count <= i) InsAddTxs.Add(Instantiate(AddTx, AddTx.transform.parent));
                var btx = InsBaseTxs[i];
                var atx = InsAddTxs[i];
                if (i >= Messages.Count)
                {
                    ChangeActive(btx.gameObject, false);
                    ChangeActive(atx.gameObject, false);
                    continue;
                }
                if (_mesMode == 0 || _mesMode - 1 == (int)Messages[i].Type)
                {
                    ChangeActive(btx.gameObject, true);
                    ChangeText(btx, Messages[i].Base);
                    ChangeActive(atx.gameObject, Messages[i].Add != "");
                    ChangeText(atx, Messages[i].Add);
                }
                else
                {
                    ChangeActive(btx.gameObject, false);
                    ChangeActive(atx.gameObject, false);
                }
            }
        }
        public void ModeChange(int mode)
        {
            _mesMode = mode;
        }
    }
}

