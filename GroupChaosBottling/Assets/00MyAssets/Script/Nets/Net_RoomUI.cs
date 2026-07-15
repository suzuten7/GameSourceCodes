using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using static DataBase;
using static PlayerValue;

public class Net_RoomUI : MonoBehaviour
{
    [Tooltip("ルーム名テキスト"), SerializeField]TextMeshProUGUI RoomName;
    [Tooltip("参加プレイヤー用サブUI"), SerializeField] List<Net_JoinPlayerUI> JoinPlayers;
    [Tooltip("プレイヤー数テキスト"), SerializeField] TextMeshProUGUI PlayerCountTx;
    [Tooltip("OKトグル"), SerializeField] Toggle OKsT;
    [Tooltip("マスター用UI"), SerializeField] GameObject MasterOnly;
    [Tooltip("プライベート切り替えトグル"), SerializeField] Toggle PrivateT;
    [Tooltip("非マスター用UI"), SerializeField] GameObject NoMaster;

    [SerializeField] TextMeshProUGUI StageTx;
    [SerializeField] RawImage StageImage;
    private void Start()
    {
        if (!PhotonNetwork.InRoom) return;
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.DestroyAll();
    }
    void LateUpdate()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        if (CRoom == null) return;
        if (PhotonNetwork.IsMasterClient) StageSet();
        UISet();
        PlayerDisp();
    }
    private void OnEnable()
    {
        OKsT.isOn = false;
    }
    void PlayerDisp()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        var PlayerKeys = CRoom.Players.Keys.ToArray();
        int OKCount = 0;
        for (int i = 0; i < Mathf.Max(PlayerKeys.Length, JoinPlayers.Count); i++)
        {
            if (JoinPlayers.Count <= i)
            {
                JoinPlayers.Add(Instantiate(JoinPlayers[0], JoinPlayers[0].transform.parent));
            }
            if (i < PlayerKeys.Length)
            {
                JoinPlayers[i].UISet(i + 1, CRoom.Players[PlayerKeys[i]],false);
                var JoinCPro = CRoom.Players[PlayerKeys[i]].CustomProperties;
                if (JoinCPro.TryGetValue("OK", out var oOKs) && (bool)oOKs) OKCount++;
            }
            JoinPlayers[i].gameObject.SetActive(i < PlayerKeys.Length);
        }
        PlayerCountTx.text = "Player:" + PlayerKeys.Length + "/" + CRoom.MaxPlayers;
        PlayerCountTx.text += " <color=#FF8800>OK:" + OKCount + "/" + PlayerKeys.Length + "</color>";
    }
    void UISet()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        var CPro = CRoom.CustomProperties;
        var PlayerCPro = PhotonNetwork.LocalPlayer.CustomProperties;

        RoomName.text = CRoom.Name;
        MasterOnly.SetActive(PhotonNetwork.IsMasterClient);
        NoMaster.SetActive(!PhotonNetwork.IsMasterClient);
        PrivateT.isOn = !CRoom.IsVisible;
        CRoom.IsOpen = true;
        var SID = CPro.TryGetValue("StageID", out var oSID) ? (int)oSID : 0;
        var StageData = DB.Stages[SID];

        PlayerCPro["OK"] = OKsT.isOn;
        PhotonNetwork.SetPlayerCustomProperties(PlayerCPro);

        StageTx.text = StageData.Name;
        StageImage.texture = StageData.Icon;
    }
    void StageSet()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        var CPro = CRoom.CustomProperties;
        bool Change = false;
        if (!CPro.TryGetValue("StageID",out var oStageID) || (int)oStageID!=StageID)
        {
            Change = true;
            CPro["StageID"] = StageID;
        }
        if (!CPro.TryGetValue("DifeMode", out var oDifeMode) || (int)oDifeMode != DifeMode)
        {
            Change = true;
            CPro["DifeMode"] = DifeMode;
        }
        if (!CPro.TryGetValue("ChaosSet", out var oChaosSet) || (bool)oChaosSet != ChaosSet)
        {
            Change = true;
            CPro["ChaosSet"] = ChaosSet;
        }
        if(Change)CRoom.SetCustomProperties(CPro);
    }
    //ルーム退室
    public void Net_RoomExit()
    {
        PhotonNetwork.LeaveRoom();
    }
    //プライベート切り替え
    public void Net_PrivateTChange()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        CRoom.IsVisible = !PrivateT.isOn;
    }

    //ゲーム開始
    public void Net_GameStart()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        CRoom.IsOpen = false;

        var CRoomCP = new ExitGames.Client.Photon.Hashtable();
        CRoomCP["SceneID"] = DB.Stages[PlayerValue.StageID].SceneID;
        CRoom.SetCustomProperties(CRoomCP);
    }
}
