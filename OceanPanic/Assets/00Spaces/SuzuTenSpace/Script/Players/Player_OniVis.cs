using UnityEngine;
using Photon.Pun;
using static GameInfos;
using static DataBase;
public class Player_OniVis : MonoBehaviour
{
    #region 変数
    [SerializeField] GameObject[] Objs;
    [SerializeField] SkinnedMeshRenderer SMRends;
    [SerializeField] Material VisLMat;
    [SerializeField] Material NoVisLMat;
    #endregion
    void LateUpdate()
    {
        if (!PhotonNetwork.InRoom) return;
        #region 取得
        var CRoom = PhotonNetwork.CurrentRoom;
        var RoomHashs = CRoom.CustomProperties;
        int IOni_Visit = -1;
        int IOni_Pilot = -1;
        if (RoomHashs.TryGetValue("Oni_Visit", out var GOni_Visit)) IOni_Visit = (int)GOni_Visit;
        if (RoomHashs.TryGetValue("Oni_Pilot", out var GOni_Pilot)) IOni_Pilot = (int)GOni_Pilot;
        int Types = -1;
        if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Pilot) Types = 1;
        else if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Visit) Types = 2;
        bool PilotViews = false;
        if (Types == 2)
        {
            for(int i = 0; i < VisitAcs.Length; i++)
            {
                if (VisitAcs[i].photonView.IsMine)
                {
                    if (VisitAcs[i].PSta.PassL.TryGetValue((int)Oni_Visit_PassE.操縦者からの離脱, out var OutBodyLV) && OutBodyLV > 0) PilotViews = true;
                }
            }
        }
        #endregion
        #region 表示変更
        for (int i = 0; i < Objs.Length; i++)
        {
            Objs[i].SetActive(Types <= 0);
        }
        if (SMRends!=null)
        {
            if (PilotViews || Types != 2) SMRends.material = NoVisLMat;
            else SMRends.material = VisLMat;
        }
        #endregion
    }
}
