using UnityEngine;
using TMPro;
using Photon.Pun;
using static GameInfos;
using System.Collections.Generic;

public class Player_UI_Pilot : MonoBehaviour
{
    #region 変数
    [SerializeField] Player_States PSta;
    [SerializeField] TextMeshProUGUI VisitTxs;
    [SerializeField] GameObject RampImage;

    #endregion
    void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        #region 取得
        var CRoom = PhotonNetwork.CurrentRoom;
        int VisitID = (int)CRoom.CustomProperties["Oni_Visit"];
        #endregion
        #region 相方テキスト設定
        var RoomHashs = CRoom.CustomProperties;
        if (RoomHashs.TryGetValue("GameOption1", out var Op1Val) && (bool)Op1Val)
        {
            VisitTxs.text = "W操縦者モード\n";
            if(RoomHashs.TryGetValue("Oni_Visit", out var GOni_Visit)&&(int)GOni_Visit == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                int PilotID = (int)CRoom.CustomProperties["Oni_Pilot"];
                VisitTxs.text += CRoom.Players.TryGetValue(PilotID, out var PilPlayer) ? PilPlayer.NickName : "<color=#FF0000><<不在>></color>";
            }
            else
            {
                VisitTxs.text += CRoom.Players.TryGetValue(VisitID, out var VisPlayer) ? VisPlayer.NickName : "<color=#FF0000><<不在>></color>";
            }
        }
        else
        {
            VisitTxs.text = "監視者\n";
            VisitTxs.text += CRoom.Players.TryGetValue(VisitID, out var VisPlayer) ? VisPlayer.NickName : "<color=#FF0000><<不在>></color>";
        }
        #endregion
        #region 暴走UI設定
        RampImage.SetActive(GInfo.Ramps);
        #endregion
    }
}
