namespace UIs
{
    using FNet;
    using Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    public class UI_ServerOnly : MonoBehaviour
    {
        [SerializeField] GameObject CamOffUI;
        [SerializeField] Toggle CamTo;
        [SerializeField] TextMeshProUGUI ServerTime;
        private void LateUpdate()
        {
            ChangeActive(CamOffUI, !Fusion_ServerMove.DispMode);
            ChangeOn(CamTo, Fusion_ServerMove.DispMode);
            var serverTime = Fusion_Manager.FixServerTime();
            var serverSec = serverTime / 60;
            string timeStr = serverTime.ToString() + "(";
            timeStr += (serverSec / 3600).ToString("D2");
            timeStr += ":" + ((serverSec / 60) % 60).ToString("D2");
            timeStr += ":" + (serverSec % 60).ToString("D2") + ")";
            ChangeText(ServerTime, timeStr);
        }
        private void Update()
        {
            if (Player_Controle.PCont.In_Menu)
            {
                CamTo.isOn = !CamTo.isOn;
                CamToSet();
                Player_Controle.PCont.In_Menu = false;
            }
        }
        public void CamToSet()
        {
            Fusion_ServerMove.DispMode = CamTo.isOn;
        }
    }
}
