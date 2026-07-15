using UnityEngine;
using Fusion;
using TMPro;
public class Net_InfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LobbyTx;
    [SerializeField] TextMeshProUGUI RoomTx;

    void Update()
    {
        if (Net_Connect.InsRunner == null)
        {
            if(LobbyTx!=null)LobbyTx.text = "NoConnect";
            RoomTx.text = "NoConnect";
            return;
        }
        if (LobbyTx != null) LobbyTx.text = Net_Connect.InsRunner.LobbyInfo != null ? ("Lobby" + Net_Connect.InsRunner.LobbyInfo.Name) : "NoLobby";
        var sinfo = Net_Connect.InsRunner.SessionInfo;
        if (sinfo == null) RoomTx.text = "NoRoom";
        else
        {
            if(Net_Connect.InsRunner.GameMode == GameMode.Single)
            {
                RoomTx.text = LocalizSystem.LocailzSCInfo("オフライン");
            }
            else
            {
                RoomTx.text = sinfo.Name + "(" + sinfo.PlayerCount + "/" + sinfo.MaxPlayers + ")";
            }

        }
    }
    async public void Lerve()
    {
        await Net_Connect.InsRunner.Shutdown(true);
    }
}
