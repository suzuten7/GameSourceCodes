using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using static DataBase;
using static GlovalValues;

public class TitleUIs : MonoBehaviourPunCallbacks
{
    #region エディタ変数
    [SerializeField] PhotonPrefabs PrePool;
    [Header("基本")]
    [SerializeField] TextMeshProUGUI GameVerTx;
    [SerializeField] GameObject[] UIs;
    [SerializeField] GameObject[] WaitUIs;
    [SerializeField] TMP_InputField PlayerNameInput;
    [Header("ルーム作成")]
    [SerializeField] TMP_InputField RoomCreateIDInput;
    [SerializeField] TMP_InputField RoomCreateMsInput;
    [SerializeField] Toggle RoomCreatePrivateT;
    [Header("ルーム参加")]
    [SerializeField] TMP_InputField RoomIDJoinIDInput;
    [SerializeField] List<Title_LobbySinUIs> LobbySinUIs;
    [Header("ルーム内")]
    [SerializeField] TextMeshProUGUI InRoomIDTx;
    [SerializeField] GameObject InRoomMasterOnly;
    [Header("ルーム内(開始前)")]
    [SerializeField] GameObject InRoomNoStartUIs;
    [SerializeField] List<Title_Sin_RoomPlayer> InRoomPlayerUIs;
    [SerializeField] TextMeshProUGUI InRoomPlayerOKs;
    [SerializeField] TextMeshProUGUI InRoomPlayerViews;
    [SerializeField] GameObject InRoomNoMaster;
    [SerializeField] Toggle InRoomViewT;
    [SerializeField] Toggle InRoomOKT;
    [SerializeField] GameObject InRoomMsNoStartUIs;
    [SerializeField] TMP_Dropdown InRoomRoleSelDr;
    [SerializeField] Toggle InRoomPrivateT;

    [SerializeField] GameObject InRoomSetsUI;
    [SerializeField] GameObject[] SetsUIs;
    [SerializeField] TextMeshProUGUI OptionsTx;
    [SerializeField] List<Title_Sin_GameOption> OptionUIs;
    [SerializeField] RawImage StageImage;
    [SerializeField] TextMeshProUGUI StageTx;
    [SerializeField] TMP_Dropdown StageTimeDr;
    [SerializeField] List<Title_Sin_GameStage> StageUIs;
    [Header("ルーム内(開始待機)")]
    [SerializeField] GameObject InRoomStartWaitUIs;
    [SerializeField] GameObject InRoomMsStartWaitUIs;
    [SerializeField] TextMeshProUGUI InRoomSWaitTx;
    [SerializeField] Title_SkillSets SkillSetUI;

    #endregion
    #region 内部変数
    bool Masters = false;
    int UIID = 0;
    int ChangeWaits = 0;
    bool SceneLoad = false;

    static int MasterRoal = 0;
    #endregion
    void Start()
    {
        #region ルーム参加チェック
        if (!PhotonNetwork.InRoom)
        {
            PrePool.PrefabPoolSet();
            PhotonNetwork.EnableCloseConnection = true;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName", "");
        }
        else
        {
            UIID = 1;
        }
        #endregion
        InRoomRoleSelDr.value = MasterRoal;

        var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
        CPro["OK"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(CPro);

        InRoomSetsUI.SetActive(false);
        RoomSetsUIChange(0);
        Title_SkillSets.SkillLoad();
    }

    // Update is called once per frame
    void Update()
    {
        #region ゲームバージョン
        GameVerTx.text = "Ver." + Application.version;
        #endregion
        #region UI切り替え
        for (int i = 0; i < UIs.Length; i++) UIs[i].SetActive(i == UIID);
        #endregion
        #region 接続状況UI切り替え
        WaitUIs[0].SetActive(!PhotonNetwork.InRoom &&UIID>0&&PhotonNetwork.IsConnected && !PhotonNetwork.IsConnectedAndReady);
        WaitUIs[1].SetActive(!PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady);
        WaitUIs[2].SetActive(PhotonNetwork.InRoom);
        #endregion
        #region ニックネーム設定
        if (!PlayerNameInput.isFocused)
        {
            if (PhotonNetwork.NickName == "") PhotonNetwork.NickName = "無名" + Random.Range(0, 10000).ToString("D4");
            PlayerNameInput.text = PhotonNetwork.NickName;
            PlayerPrefs.SetString("PlayerName", PhotonNetwork.NickName);
        }
        #endregion
        #region ルーム内処理
        if (PhotonNetwork.InRoom)
        {
            #region 取得など
            var CRoom = PhotonNetwork.CurrentRoom;
            InRoomMasterOnly.SetActive(PhotonNetwork.IsMasterClient);
            InRoomNoMaster.SetActive(!PhotonNetwork.IsMasterClient);
            InRoomIDTx.text = CRoom.Name;
            var PlKeys = CRoom.Players.Keys.ToArray();

            if (PhotonNetwork.IsMasterClient)
            {
                var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
                CPro["Views"] = MasterRoal == 4;
                PhotonNetwork.LocalPlayer.SetCustomProperties(CPro);
            }
            else
            {
                var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
                InRoomViewT.isOn = (CPro.TryGetValue("Views", out var Viewd) && (bool)Viewd);
                InRoomViewT.interactable = !InRoomOKT.isOn;
                InRoomOKT.isOn = (CPro.TryGetValue("OK", out var OKs) && (bool)OKs);
            }
            var RoomHashs = CRoom.CustomProperties;
            bool IStarts = false;
            if (RoomHashs.TryGetValue("GameStarts", out var GStarts)) IStarts = (bool)GStarts;
            #endregion
            #region プレイヤーリスト
            int OKc = 0;
            int Viewc = 0;
            for (int i = 0; i < Mathf.Max(PlKeys.Length,InRoomPlayerUIs.Count); i++)
            {
                if(i < PlKeys.Length)
                {
                    bool OKd = false;
                    if (InRoomPlayerUIs.Count <= i)
                    {
                        var SUIBase = InRoomPlayerUIs[0];
                        InRoomPlayerUIs.Add(Instantiate(SUIBase, SUIBase.transform.parent));
                    }
                    var SinUI = InRoomPlayerUIs[i];
                    var PPlayer = CRoom.Players[PlKeys[i]];
                    var CPro = PPlayer.CustomProperties;

                    SinUI.gameObject.SetActive(true);
                    SinUI.PPlayer = PPlayer;
                    SinUI.MasterOnlys.gameObject.SetActive(PhotonNetwork.IsMasterClient&&!IStarts&&PPlayer!=PhotonNetwork.LocalPlayer);
                    SinUI.PIDTx.text = "[" + (i + 1) + "]";
                    SinUI.PlayerNametx.text = PPlayer.NickName;
                    bool Views = CPro.TryGetValue("Views", out var Viewd) ? (bool)Viewd : false;
                    if (Views)
                    {
                        SinUI.PlayerNametx.text += "<color=#FFFF00>(観戦)</color>";
                        Viewc++;
                        OKd = true;
                    }
                    if (PPlayer.IsMasterClient)
                    {
                        SinUI.StateTx.text = "マスター";
                        SinUI.StateTx.color = Color.yellow;
                        OKd = true;
                    }
                    else
                    {
                        if (CPro.TryGetValue("OK", out var OKs) && (bool)OKs)
                        {
                            SinUI.StateTx.text = "OK";
                            SinUI.StateTx.color = new Color(1,0.7f,0.7f);
                            OKd = true;
                        }
                        else
                        {
                            if (!Views)
                            {
                                SinUI.StateTx.text = "待機中";
                                SinUI.StateTx.color = Color.white;
                            }
                            else
                            {
                                SinUI.StateTx.text = "観戦待機";
                                SinUI.StateTx.color = new Color(1,0.5f,0);
                            }

                        }
                    }
                    if (OKd) OKc++;
                }
                else
                {
                    InRoomPlayerUIs[i].gameObject.SetActive(false);
                }

            }
            InRoomPlayerOKs.text = "OKor観戦:" + OKc + "/" + PlKeys.Length;
            InRoomPlayerViews.text = "観戦:" + Viewc + "/" + PlKeys.Length;
            #endregion
            #region 各UI表示
            InRoomNoStartUIs.SetActive(!IStarts);
            InRoomMsNoStartUIs.SetActive(!IStarts);
            InRoomStartWaitUIs.SetActive(IStarts);
            InRoomMsStartWaitUIs.SetActive(IStarts);
            #endregion
            #region オプション設定
            OptionsTx.text = "";
            for(int i = 0; i < OptionUIs.Count; i++)
            {
                bool On = RoomHashs.TryGetValue("GameOption" + i, out var OpVal) && (bool)OpVal == true;
                OptionUIs[i].ID = i;
                if (On)
                {
                    if (OptionsTx.text!="") OptionsTx.text += "\n";
                    OptionsTx.text += OptionUIs[i].OptionNameTx.text;
                    OptionUIs[i].OptionOnOffTx.text = "ON";
                    OptionUIs[i].OnOffButtonImage.color = Color.green;
                }
                else
                {
                    OptionUIs[i].OptionOnOffTx.text = "OFF";
                    OptionUIs[i].OnOffButtonImage.color = new Color(1,0.5f,0.2f);
                }
            }
            #endregion

            #region ステージ設定
            int StageID = RoomHashs.TryGetValue("StageID", out var SIDVal) ? (int)SIDVal : -1;
            if (StageID >= 0)
            {
                var SelStageData = DB.Stages[StageID];
                StageTx.text = SelStageData.Name;
                StageImage.texture = SelStageData.StageImage;
                StageImage.color = Color.white;
            }
            else
            {
                StageTx.text = "ランダムステージ";
                StageImage.color = Color.clear;
            }
            StageTimeDr.value = RoomHashs.TryGetValue("StageTime", out var STimeVal) ? (int)STimeVal : 3;
            StageTimeDr.interactable = PhotonNetwork.IsMasterClient;
            for(int i = 0; i < DB.Stages.Length; i++)
            {
                if(StageUIs.Count <= i)
                {
                    var CreUI = Instantiate(StageUIs[0], StageUIs[0].transform.parent);
                    StageUIs.Add(CreUI);
                }
                var StageData = DB.Stages[i];
                StageUIs[i].StageImage.texture = StageData.StageImage;
                StageUIs[i].StageNameTx.text = StageData.Name;
                StageUIs[i].ID = i;
            }
            #endregion
            #region スタート後
            if (IStarts)
            {
                InRoomSetsUI.SetActive(false);
                if (ChangeWaits >= 0)
                {
                    int Times = (60 * 31) - 1 - ChangeWaits;
                    InRoomSWaitTx.text = "開始まで" + (Times / 60).ToString("D0") + "秒";
                    #region 取得
                    int IOni_Visit = -1;
                    int IOni_Pilot = -1;
                    bool Non = false;
                    if (RoomHashs.TryGetValue("Oni_Visit", out var GOni_Visit)) IOni_Visit = (int)GOni_Visit;
                    else Non = true;
                    if (RoomHashs.TryGetValue("Oni_Pilot", out var GOni_Pilot)) IOni_Pilot = (int)GOni_Pilot;
                    else Non = true;
                    #endregion
                    if (!Non)
                    {
                        #region ロール表示・パッシブ設定
                        var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
                        int TypeID = 3;
                        if (IOni_Visit == PhotonNetwork.LocalPlayer.ActorNumber) TypeID = 2;
                        else if (IOni_Pilot == PhotonNetwork.LocalPlayer.ActorNumber) TypeID = 1;
                        else if (CPro.TryGetValue("Views", out var Viewd) && (bool)Viewd) TypeID = 3;
                        else TypeID = 0;
                        if (TypeID == 2 && RoomHashs.TryGetValue("GameOption1", out var Op1Val) && (bool)Op1Val) TypeID = 1;
                        SkillSetUI.RoalSet(TypeID);
                        #endregion
                    }
                    else SkillSetUI.RoalSet(-1); 
                }
                else
                {
                    InRoomSWaitTx.text = "";
                    SkillSetUI.RoalSet(-2);
                }

            }
            #endregion
            #region 非スタート
            else
            {
                ChangeWaits = 0;
            }
            #endregion
        }
        else
        {
            var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
            CPro["OK"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(CPro);
            InRoomSetsUI.SetActive(false);
            RoomSetsUIChange(0);
        }
        #endregion
    }
    private void FixedUpdate()
    {
        #region ルーム内処理
        if (PhotonNetwork.InRoom)
        {
            #region 取得
            var CRoom = PhotonNetwork.CurrentRoom;
            InRoomPrivateT.isOn = !CRoom.IsVisible;

            var RoomHashs = CRoom.CustomProperties;
            bool IStarts = false;
            if (RoomHashs.TryGetValue("GameStarts", out var GStarts)) IStarts = (bool)GStarts;
            #endregion
            if (IStarts)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    #region ロール設定
                    if (ChangeWaits <= 0)
                    {
                        var PlKeys = CRoom.Players.Keys.ToList();
                        for(int i = PlKeys.Count-1; i >= 0; i--)
                        {
                           var CPro = CRoom.Players[PlKeys[i]].CustomProperties;
                            if (CPro.TryGetValue("Views", out var Viewd) && (bool)Viewd) PlKeys.RemoveAt(i);
                        }
                        int Oni_Pilot = -1;
                        if (MasterRoal == 2) Oni_Pilot = PhotonNetwork.LocalPlayer.ActorNumber;
                        else while (PlKeys.Count >= 1)
                            {
                                Oni_Pilot = PlKeys[Random.Range(0, PlKeys.Count)];
                                if (Oni_Pilot == PhotonNetwork.LocalPlayer.ActorNumber && MasterRoal != 0)
                                {
                                    if (PlKeys.Count < 2)
                                    {
                                        Oni_Pilot = -1;
                                        break;
                                    }
                                }
                                else break;
                            }

                        int Oni_Visit = -1;
                        if (MasterRoal == 3) Oni_Visit = PhotonNetwork.LocalPlayer.ActorNumber;
                        else while (PlKeys.Count > 3)
                            {
                                int RandPID = PlKeys[Random.Range(0, PlKeys.Count)];
                                if (RandPID != Oni_Pilot)
                                {
                                    Oni_Visit = RandPID;
                                    if (Oni_Visit == PhotonNetwork.LocalPlayer.ActorNumber && MasterRoal != 0)
                                    {
                                        if (PlKeys.Count < 3)
                                        {
                                            Oni_Visit = -1;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }

                        RoomHashs["Oni_Visit"] = Oni_Visit;
                        RoomHashs["Oni_Pilot"] = Oni_Pilot;
                        CRoom.SetCustomProperties(RoomHashs);
                    }
                    #endregion
                    #region シーン移行
                    if (!SceneLoad && ChangeWaits >= 60 * 30)
                    {
                        SceneLoad = true;
                        var StageID = RoomHashs.TryGetValue("StageID",out var SIDVal) ? (int)SIDVal : -1;
                        int SceneID;
                        if (StageID == -1)
                        {
                            while (true)
                            {
                                var StageData = DB.Stages[Random.Range(0, DB.Stages.Length - 1)];
                                if (!StageData.NoRandoms)
                                {
                                    SceneID = StageData.SceneID;
                                    break;
                                }
                            }
                        }
                        else SceneID = DB.Stages[StageID].SceneID;
                        PhotonNetwork.LoadLevel(SceneID);
                    }
                    #endregion
                }
                ChangeWaits++;
            }
        }
        #endregion
    }
    #region 呼び出しメソッド
    public void UISets(int ID)
    {
        UIID = ID;
        if (UIID == 4) PhotonNetwork.JoinLobby();
        else if(PhotonNetwork.InLobby)PhotonNetwork.LeaveLobby();
        if (UIID > 0&&!Masters) PhotonNetwork.ConnectUsingSettings();
    }
    public void RoomCreate()
    {
        var RoomOP = new RoomOptions();
        var RoomHash = new ExitGames.Client.Photon.Hashtable();
        RoomHash["Message"] = RoomCreateMsInput.text;
        RoomHash["GameVer"] = Application.version;
        RoomHash["GameStarts"] = false;
        RoomOP.MaxPlayers = 20;
        RoomOP.CustomRoomProperties = RoomHash;
        RoomOP.CustomRoomPropertiesForLobby = new string[] { "Message","GameVer", "GameStarts" };
        RoomOP.IsVisible = !RoomCreatePrivateT.isOn;
        var RoomIDs = RoomCreateIDInput.text;
        if (RoomIDs == "")
        {
            if (Random.value >= 0.1f) RoomIDs = "CreateRoom";
            else RoomIDs = "NaNameRoom";
            RoomIDs += Random.Range(0, 10000).ToString("D4");
        }
        PhotonNetwork.CreateRoom(RoomIDs, RoomOP, TypedLobby.Default);
    }
    public void RoomIDJoin()
    {
        PhotonNetwork.JoinRoom(RoomIDJoinIDInput.text);
    }
    public void RoomRandomJoin()
    {
        var expectedProps = new ExitGames.Client.Photon.Hashtable();
        expectedProps.Add("GameVer", Application.version);
        PhotonNetwork.JoinRandomRoom(expectedProps,20);
    }
    public void RoomLeave()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void StartButton()
    {
        if (!PhotonNetwork.InRoom) return;
        var CRoom = PhotonNetwork.CurrentRoom;
        var RoomHashs = CRoom.CustomProperties;
        bool IStarts = false;
        if (RoomHashs.TryGetValue("GameStarts", out var GStarts)) IStarts = (bool)GStarts;
        if (!IStarts)
        {
            RoomHashs["GameStarts"] = true;
            CRoom.SetCustomProperties(RoomHashs);
        }
        else
        {
            ChangeWaits = 60 * 30;
        }
    }
    public void RoomSetsUIOC(bool Open)
    {
        InRoomSetsUI.SetActive(Open);
    }
    public void RoomSetsUIChange(int ID)
    {
        for(int i=0;i < SetsUIs.Length; i++)
        {
            SetsUIs[i].SetActive(ID == i);
        }
    }
    public void RoomSetsUIOptionSet(int ID)
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        var RoomProp = PhotonNetwork.CurrentRoom.CustomProperties;

        string PropKey = "GameOption" + ID;
        RoomProp[PropKey] = RoomProp.TryGetValue(PropKey, out var Val) ? !(bool)Val : true;
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(RoomProp);
    }
    public void RoomSetsUIStageIDSet(int ID)
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        var RoomProp = PhotonNetwork.CurrentRoom.CustomProperties;
        RoomProp["StageID"] = ID;
        PhotonNetwork.CurrentRoom.SetCustomProperties(RoomProp);
    }
    public void RoomSetsUIStageTimeSet()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        var RoomProp = PhotonNetwork.CurrentRoom.CustomProperties;
        RoomProp["StageTime"] = StageTimeDr.value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(RoomProp);
    }

    public void LobbyRoomJoin(RoomInfo RoomIn)
    {
        PhotonNetwork.JoinRoom(RoomIn.Name);
    }
    public void PlayerNameSet()
    {
        PhotonNetwork.NickName = PlayerNameInput.text;
    }
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else 
        Application.Quit();//ゲームプレイ終了
#endif
    }
    public void InRoomPrivateChange()
    {
        if (!PhotonNetwork.InRoom) return;
        PhotonNetwork.CurrentRoom.IsVisible = !InRoomPrivateT.isOn;
    }
    public void InRoomViewTChange()
    {
        if (!PhotonNetwork.InRoom) return;
        var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
        CPro["Views"] = InRoomViewT.isOn;
        PhotonNetwork.LocalPlayer.SetCustomProperties(CPro);
    }
    public void InRoomOKTChange()
    {
        if (!PhotonNetwork.InRoom) return;
        var CPro = PhotonNetwork.LocalPlayer.CustomProperties;
        CPro["OK"] = InRoomOKT.isOn;
        PhotonNetwork.LocalPlayer.SetCustomProperties(CPro);
    }



    public void MasterRoalSet()
    {
        MasterRoal = InRoomRoleSelDr.value;
    }
    #endregion
    #region Photonコールバック
    public override void OnConnectedToMaster()
    {
        Masters = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Masters = false;
    }
    RoomList roomList = new RoomList();
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var RoomOP = new RoomOptions();
        var RoomHash = new ExitGames.Client.Photon.Hashtable();
        RoomHash["Message"] = "ランダムルーム";
        RoomHash["GameVer"] = Application.version;
        RoomHash["GameStarts"] = false;
        RoomOP.MaxPlayers = 20;
        RoomOP.CustomRoomProperties = RoomHash;
        RoomOP.CustomRoomPropertiesForLobby = new string[] { "Message", "GameVer", "GameStarts" };
        RoomOP.IsVisible = true;
        var RoomIDs = "";
        if (Random.value >= 0.1f) RoomIDs = "FrontRoom";
        else RoomIDs = "BackRoom";
        RoomIDs += Random.Range(0, 10000).ToString("D4");
        PhotonNetwork.CreateRoom(RoomIDs, RoomOP, TypedLobby.Default);
    }
    #endregion
}
