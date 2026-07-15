using Photon.Pun;
using TMPro;
using UnityEngine;
using static BattleManager;
public class Net_RoomExitButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Texts;
    [SerializeField] string MasterTxs;
    [SerializeField] string NoMasterTxs;
    [SerializeField] bool EndWaits;
    float EndTimed = 0;
    bool Master => !PhotonNetwork.OfflineMode && PhotonNetwork.IsMasterClient;
    float EndWait = 10f;
    private void Update()
    {

        if (PhotonNetwork.OfflineMode) Texts.text = "タイトルに戻る";
        else
        {
            if (!PhotonNetwork.IsMasterClient && EndWaits && BTManager.End)
            {
                EndTimed += Time.unscaledDeltaTime;
                if (EndTimed < EndWait)
                {
                    Texts.text = "退室可能まで" + (EndWait - EndTimed).ToString("0") + "秒";
                }
                else Texts.text = NoMasterTxs;
            }
            else Texts.text = PhotonNetwork.IsMasterClient ? MasterTxs : NoMasterTxs;
        }
    }
    //退室
    public void ExitB()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient && EndWaits && EndTimed < EndWait) return;
        if (Master)
        {
            var CRoom = PhotonNetwork.CurrentRoom;
            var CRoomCP = new ExitGames.Client.Photon.Hashtable();
            CRoomCP["SceneID"] = 0;
            CRoom.SetCustomProperties(CRoomCP);
        }
        else PhotonNetwork.LeaveRoom();
    }
}
