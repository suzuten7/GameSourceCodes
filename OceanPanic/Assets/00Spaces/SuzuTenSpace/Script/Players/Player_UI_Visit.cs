using Photon.Pun;
using TMPro;
using UnityEngine;

public class Player_UI_Visit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PilotTxs;
    void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        #region 取得
        var CRoom = PhotonNetwork.CurrentRoom;
        int PilotID = (int)CRoom.CustomProperties["Oni_Pilot"];
        #endregion
        #region 相方テキスト設定
        PilotTxs.text = "操縦者\n";
        PilotTxs.text += CRoom.Players.TryGetValue(PilotID, out var PilPlayer) ? PilPlayer.NickName : "<color=#FF0000><<不在>></color>";
        #endregion
    }
}
