using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerValue;
public class Net_BaseUI : MonoBehaviourPunCallbacks
{
    [Tooltip("同期するプレファブ"), SerializeField] Net_PhotonPrefabs Prefabs;
    [Tooltip("オフラインUI"), SerializeField] GameObject OfflineUI;
    [Tooltip("オンラインUI"), SerializeField] GameObject OnlineUI;
    [Tooltip("接続待機UI"), SerializeField] GameObject ConnectWait;
    [Tooltip("接続完了UI"), SerializeField] GameObject ConnectEnd;
    [Tooltip("ルーム選択処理"), SerializeField] UIChange Selects;
    [Tooltip("ルーム選択用UI"), SerializeField] GameObject SelectUI;
    [Tooltip("プレイヤー名入力"), SerializeField] TMP_InputField PlayerNameIn;
    [Tooltip("作成ルーム名入力"), SerializeField] TMP_InputField CreateNameIn;
    [Tooltip("作成メッセージ入力"), SerializeField] TMP_InputField MessageIn;
    [Tooltip("作成プライベート"),SerializeField] Toggle PrivateT;
    [Tooltip("参加ルーム名入力"), SerializeField] TMP_InputField JoinNameIn;
    [Tooltip("ロビー用サブUI"), SerializeField] List<SinsUI_LobbyRoomUIs> LobbySinUIs;
    [Tooltip("ルーム内UI"), SerializeField] GameObject InRoomUI;

    private void Awake()
    {
        Prefabs.PrefabPoolSet();
        Net_MyCustomTypes.Register();
        Application.targetFrameRate = 60;
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.AutomaticallySyncScene = false;
        PlayerNameIn.text = PlayerPrefs.GetString("PlayerName", "");
        Net_PlayerNameSet();

        PlayerValue.Load();
    }
    private void Update()
    {
       CharaSet();
    }
    void LateUpdate()
    {
        if (!PlayerNameIn.isFocused) PlayerNameIn.text = PhotonNetwork.NickName;

        UIActives();
        LobbyIns();
    }
    /// <summary>UI表示</summary>
    void UIActives()
    {
        bool Connect = !PhotonNetwork.IsConnected || !PhotonNetwork.IsConnectedAndReady;
        OfflineUI.SetActive(PhotonNetwork.OfflineMode);
        OnlineUI.SetActive(!PhotonNetwork.OfflineMode);
        ConnectWait.SetActive(Connect);
        ConnectEnd.SetActive(!Connect);
        SelectUI.SetActive(!PhotonNetwork.InRoom);
        InRoomUI.SetActive(PhotonNetwork.InRoom);
    }
    /// <summary>ロビー参加</summary>
    void LobbyIns()
    {
        if (Selects.UIID == 3 && !PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();
        if (Selects.UIID != 3 && PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
    }
    //プレイヤー名変更
    public void Net_PlayerNameSet()
    {
        if (PlayerNameIn.text != "") PhotonNetwork.NickName = PlayerNameIn.text;
        else PhotonNetwork.NickName = "無名" + Random.Range(1000,10000);
        PlayerPrefs.SetString("PlayerName", PhotonNetwork.NickName);
    }
    //サーバー接続
    public void Net_Connects()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    //サーバー切断
    public void Net_Disconnect()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.OfflineMode = true;
    }
    //新規ルーム作成
    public void Net_RoomCreate()
    {
        var RoomIDs = CreateNameIn.text;
        if (RoomIDs == "")
        {
            if (Random.value >= 0.1f) RoomIDs = "CreateRoom";
            else RoomIDs = "NaNameRoom";
            RoomIDs += Random.Range(0, 10000).ToString("D4");
        }
        PhotonNetwork.CreateRoom(RoomIDs,RoomOptionGet(MessageIn.text,PrivateT.isOn));
    }
    //名前ルーム参加
    public void Net_RoomJoin()
    {
        PhotonNetwork.JoinRoom(JoinNameIn.text);
    }
    //ランダムルーム参加
    public void Net_RandomJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    //ランダムルーム用作成
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        string RoomIDs;
        if (Random.value >= 0.1f) RoomIDs = "RandomRoom";
        else RoomIDs = "BackRoom";
        RoomIDs += Random.Range(0, 10000).ToString("D4");
        PhotonNetwork.CreateRoom(RoomIDs, RoomOptionGet("ランダムルーム",false));
    }
    /// <summary>ルームオプション設定</summary>
    public RoomOptions RoomOptionGet(string Message, bool Private)
    {
        var RoomOP = new RoomOptions();
        var RoomHash = new ExitGames.Client.Photon.Hashtable();
        RoomHash["Message"] = Message;
        RoomHash["GameVer"] = Application.version;
        RoomHash["GameStarts"] = false;
        RoomOP.MaxPlayers = 20;
        RoomOP.CustomRoomProperties = RoomHash;
        RoomOP.CustomRoomPropertiesForLobby = new string[] { "Message", "GameVer", "GameStarts" };
        RoomOP.IsVisible = !Private;
        return RoomOP;
    }
    //オフライン化
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.OfflineMode = true;
    }
    //ロビー用
    Net_RoomList roomList = new Net_RoomList();
    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        roomList.Update(changedRoomList);
        if (PhotonNetwork.InLobby)
        {
            List<RoomInfo> Rooms = roomList.ToList();
            for (int i = 0; i < Mathf.Max(LobbySinUIs.Count, Rooms.Count); i++)
            {
                if (i < Rooms.Count)
                {
                    if (LobbySinUIs.Count <= i)
                    {
                        LobbySinUIs.Add(Instantiate(LobbySinUIs[0], LobbySinUIs[0].transform.parent));
                    }
                    var RoomSin = LobbySinUIs[i];
                    RoomSin.gameObject.SetActive(true);
                    RoomSin.Disp(Rooms[i]);
                }
                else LobbySinUIs[i].gameObject.SetActive(false);
            }
        }
    }
    static public void CharaSet()
    {
        var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
        bool Change = false;
        var SubUse = PSaves.AddSet1 >= 0;
        if (!CPro.TryGetValue("SubUse", out var oSubUse) || (bool)oSubUse != SubUse)
        {
            CPro["SubUse"] = SubUse;
            Change = true;
        }

        for (int i = 0; i < 2; i++)
        {
            var PriSetID = PSaves.PriSetID;
            if (i >= 1 && PSaves.AddSet1 >= 0) PriSetID = PSaves.AddSet1;
            var PriSet = PriSets[PriSetID];
            var SStr = i <= 0 ? "PL_" : "SB_";

            var MemoStr = "(" + (PriSetID+1) + ")" +  PriSet.Disp;
            if (!CPro.TryGetValue(SStr + "Memo", out var oMemo) || (string)oMemo != MemoStr)
            {
                CPro[SStr + "Memo"] = MemoStr;
                Change = true;
            }

            if (!CPro.TryGetValue(SStr + "Chara", out var oChara) || (int)oChara != PriSet.CharaID)
            {
                CPro[SStr + "Chara"] = PriSet.CharaID;
                Change = true;
            }

            if (!CPro.TryGetValue(SStr + "FAtk_0", out var oFAtk_0) || (int)oFAtk_0 != PriSet.AtkF.N_AtkID)
            {
                CPro[SStr + "FAtk_0"] = PriSet.AtkF.N_AtkID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "FAtk_1", out var oFAtk_1) || (int)oFAtk_1 != PriSet.AtkF.S1_AtkID)
            {
                CPro[SStr + "FAtk_1"] = PriSet.AtkF.S1_AtkID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "FAtk_2", out var oFAtk_2) || (int)oFAtk_2 != PriSet.AtkF.S2_AtkID)
            {
                CPro[SStr + "FAtk_2"] = PriSet.AtkF.S2_AtkID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "FAtk_3", out var oFAtk_3) || (int)oFAtk_3 != PriSet.AtkF.E_AtkID)
            {
                CPro[SStr + "FAtk_3"] = PriSet.AtkF.E_AtkID;
                Change = true;
            }

            if (!CPro.TryGetValue(SStr + "BAtk_0", out var oBAtk_0) || (int)oBAtk_0 != PriSet.AtkB.N_AtkID)
            {
                CPro[SStr + "BAtk_0"] = PriSet.AtkB.N_AtkID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "BAtk_1", out var oBAtk_1) || (int)oBAtk_1 != PriSet.AtkB.S1_AtkID)
            {
                CPro[SStr + "BAtk_1"] = PriSet.AtkB.S1_AtkID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "BAtk_2", out var oBAtk_2) || (int)oBAtk_2 != PriSet.AtkB.S2_AtkID)
            {
                CPro[SStr + "BAtk_2"] = PriSet.AtkB.S2_AtkID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "BAtk_3", out var oBAtk_3) || (int)oBAtk_3 != PriSet.AtkB.E_AtkID)
            {
                CPro[SStr + "BAtk_3"] = PriSet.AtkB.E_AtkID;
                Change = true;
            }

            if (!CPro.TryGetValue(SStr + "Passive_0", out var oPass_0) || (int)oPass_0 != PriSet.Passive.P1_ID)
            {
                CPro[SStr + "Passive_0"] = PriSet.Passive.P1_ID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "Passive_1", out var oPass_1) || (int)oPass_1 != PriSet.Passive.P2_ID)
            {
                CPro[SStr + "Passive_1"] = PriSet.Passive.P2_ID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "Passive_2", out var oPass_2) || (int)oPass_2 != PriSet.Passive.P3_ID)
            {
                CPro[SStr + "Passive_2"] = PriSet.Passive.P3_ID;
                Change = true;
            }
            if (!CPro.TryGetValue(SStr + "Passive_3", out var oPass_3) || (int)oPass_3 != PriSet.Passive.P4_ID)
            {
                CPro[SStr + "Passive_3"] = PriSet.Passive.P4_ID;
                Change = true;
            }
        }
        if (Change) PhotonNetwork.SetPlayerCustomProperties(CPro);
    }
}
