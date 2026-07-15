using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Suzuten_SelectUI;
public class Suzuten_PhotonUIs : MonoBehaviourPunCallbacks
{
    #region 変数
    [SerializeField] GameObject NetUI;
    [SerializeField] PhotonPrefabs PrePool;
    [SerializeField] Suzuten_DataBase DB;
    [SerializeField] Suzuten_SelectUI SelectUI;

    [SerializeField] TextMeshProUGUI CharaNametx;
    [SerializeField] RawImage CharaImage;
    [SerializeField] TMP_InputField NameIn;
    [SerializeField] TextMeshProUGUI TypeTx;
    [SerializeField] TMP_Dropdown MatchingDr;


    [SerializeField] GameObject NonConnetsUIs;
    [SerializeField] GameObject ConnetsUIs;
    [SerializeField] GameObject PlayerStayUIs;
    [SerializeField] TextMeshProUGUI StayRoomInfos;

    [SerializeField] GameObject BaseUIs;
    [SerializeField] RoomCreUIsC RoomCreUIs;
    [SerializeField] RoomIDJoinUIsC RoomIDJoinUIs;
    [SerializeField] LobbyJoinUIsC LobbyJoinUIs;

    static int MatchDrs = 0;
    #endregion
    #region クラス
    [System.Serializable]
    class RoomCreUIsC
    {
        public GameObject UI;
        public TMP_InputField RoomID_Input;
        public TMP_InputField Message_Input;
        public Toggle Private_To;
    }
    [System.Serializable]
    class RoomIDJoinUIsC
    {
        public GameObject UI;
        public TMP_InputField RoomID_Input;
    }
    [System.Serializable]
    class LobbyJoinUIsC
    {
        public GameObject UI;
        public List<ConnectRoomSinUIs> RoomSinUIs;
    }
    #endregion
    void Start()
    {
        PrePool.PrefabPoolSet();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        NameIn.text = PlayerPrefs.GetString("PlayerName", "");
        NameInput();
        BaseUIs.SetActive(true);
        RoomCreUIs.UI.SetActive(false);
        RoomIDJoinUIs.UI.SetActive(false);
        LobbyJoinUIs.UI.SetActive(false);
        ConnetsUIs.SetActive(true);
        NonConnetsUIs.SetActive(false);
        MatchingDr.value = MatchDrs;
    }
    private void LateUpdate()
    {
        ConnetsUIs.SetActive(!PhotonNetwork.IsConnectedAndReady&&!PhotonNetwork.InRoom);
        NonConnetsUIs.SetActive(PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom);
        PlayerStayUIs.SetActive(PhotonNetwork.InRoom);

        if (!PhotonNetwork.InRoom)
        {
            if (NetUI.activeSelf)
            {
                #region プレイヤー情報設定
                int CharaID = Suzuten_SelectUI.SelectSlots[0].x + Suzuten_SelectUI.SelectSlots[0].y * SelectUI.CharaXcounts - RaCharaCo;
                if (CharaID >= DB.Charas.Length) CharaID = -3;
                ExitGames.Client.Photon.Hashtable PLCharas = new ExitGames.Client.Photon.Hashtable();
                if (CharaID >= 0)
                {
                    var CData = DB.Charas[CharaID];
                    CharaNametx.text = CData.CharaName;
                    CharaImage.texture = CData.CharaImage;

                    PLCharas.Add("CharaID", CharaID);
                    for (int i = 0; i < 5; i++) PLCharas.Add("ACID_" + i, PlayerPrefs.GetInt("ACIDs_0_" + i + "_" + CData.name, i));
                    int MatchVal = 0;
                    if (CData.UseBandChara)
                    {
                        MatchVal = 2;
                        TypeTx.color = new Color(0.4f, 0f, 0.4f);
                        TypeTx.text = "使用禁止";
                    }
                    else if (CData.RandomNoSelects)
                    {
                        MatchVal = 1;
                        TypeTx.color = new Color(1f, 0.3f, 0.3f);
                        TypeTx.text = "ランダム除外";
                    }
                    else
                    {
                        TypeTx.color = Color.black;
                        TypeTx.text = "ランダム対象";
                    }
                    if (MatchingDr.value < MatchVal) MatchingDr.value = MatchVal;
                }
                else
                {
                    int RandID;
                    int MatchVal = 0;
                    switch (CharaID)
                    {
                        default:
                            CharaNametx.text = "ランダム";
                            TypeTx.color = Color.black;
                            TypeTx.text = "ランダムOnly";
                            while (true)
                            {
                                RandID = Random.Range(0, DB.Charas.Length);
                                if (!DB.Charas[RandID].RandomNoSelects&& !DB.Charas[RandID].UseBandChara) break;
                            }
                            break;
                        case -2:
                            CharaNametx.text = "ランダム(除外込み)";
                            MatchVal = 1;
                            TypeTx.color = new Color(1f, 0.3f, 0.3f);
                            TypeTx.text = "ランダム除外";
                            while (true)
                            {
                                RandID = Random.Range(0, DB.Charas.Length);
                                if (!DB.Charas[RandID].UseBandChara) break;
                            }
                            break;
                        case -1:
                            CharaNametx.text = "ランダム(禁止込み)";
                            MatchVal = 2;
                            TypeTx.color = new Color(0.4f, 0f, 0.4f);
                            TypeTx.text = "使用禁止";
                            RandID = Random.Range(0, DB.Charas.Length);
                            break;
                    }
                    if (MatchingDr.value < MatchVal) MatchingDr.value = MatchVal;
                    CharaImage.texture = DB.RandomStageImage.texture;
                    PLCharas.Add("CharaID", RandID);
                    for (int i = 0; i < 5; i++) PLCharas.Add("ACID_" + i, PlayerPrefs.GetInt("ACIDs_0_" + i + "_" + DB.Charas[RandID].name, i));
                }
                PhotonNetwork.SetPlayerCustomProperties(PLCharas);
                #endregion
                MatchDrs = MatchingDr.value;
                if (PhotonNetwork.InLobby) RoomUIsSet();
            }
        }
        else
        {
            StayRoomInfos.text = "ルームID:";
            StayRoomInfos.text += PhotonNetwork.CurrentRoom.Name + "\n";
            StayRoomInfos.text += "マッチング対象:";
            switch ((int)PhotonNetwork.CurrentRoom.CustomProperties["MatchType"])
            {
                case 0: StayRoomInfos.text += "ランダム対象\n"; break;
                case 1: StayRoomInfos.text += "<color=#FF7777>ランダム除外</color>\n"; break;
                case 2: StayRoomInfos.text += "<color=#550055>使用禁止</color>\n"; break;
                case 3: StayRoomInfos.text += "ミラーマッチ\n"; break;
                default: StayRoomInfos.text += "???\n"; break;
            }
            StayRoomInfos.text += "メッセージ\n";
            StayRoomInfos.text += PhotonNetwork.CurrentRoom.CustomProperties["Message"].ToString();

            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.CurrentRoom.IsOpen)
            {
                #region シーン移行
                PhotonNetwork.CurrentRoom.IsOpen = false;
                int SceneID;
                if (SelectUI.StageDr.value <= 0)
                {
                    while (true)
                    {
                        Suzuten_StageData StageD = DB.StageDatas[Random.Range(0, DB.StageDatas.Length)];
                        if (!StageD.RandomNoSelects)
                        {
                            SceneID = StageD.StageSceneID;
                            break;
                        }
                    }
                }
                else SceneID = DB.StageDatas[SelectUI.StageDr.value - 1].StageSceneID;
                SceneChangeUIs.SCUIDisp();
                PhotonNetwork.LoadLevel(SceneID);
                #endregion
            }
        }
    }

    #region UI操作
    public void NameInput()
    {
        if (NameIn.text == "") NameIn.text = "Player" + Random.Range(1000, 10000);
        PlayerPrefs.SetString("PlayerName", NameIn.text);
        PhotonNetwork.NickName = NameIn.text;
    }
    public void RoomCreOC(bool B)
    {
        BaseUIs.SetActive(!B);
        RoomCreUIs.UI.SetActive(B);
    }
    public void RoomIDJoinOC(bool B)
    {
        BaseUIs.SetActive(!B);
        RoomIDJoinUIs.UI.SetActive(B);
    }
    public void LobbyJoinOC(bool B)
    {
        BaseUIs.SetActive(!B);
        LobbyJoinUIs.UI.SetActive(B);
        if (B) PhotonNetwork.JoinLobby();
        else PhotonNetwork.LeaveLobby();
    }
    public void Cancel()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region ルーム作成・参加
    public void GameStart_Create()
    {
        string RoomName = RoomCreUIs.RoomID_Input.text != "" ? RoomCreUIs.RoomID_Input.text : "Room" + Random.Range(1000, 10000);
        var initialProps = new ExitGames.Client.Photon.Hashtable();
        initialProps["CreaterName"] = PhotonNetwork.NickName;
        initialProps["Message"] = RoomCreUIs.Message_Input.text;
        initialProps["MatchType"] = MatchingDr.value;
        initialProps["GameVer"] = Application.version;

        int CharaID = Suzuten_SelectUI.SelectSlots[0].x + Suzuten_SelectUI.SelectSlots[0].y * SelectUI.CharaXcounts - RaCharaCo;
        if (CharaID >= DB.Charas.Length) CharaID = -3;
        initialProps["CharaID"] = CharaID;
        // ロビーのルーム情報から取得できるカスタムプロパティ（キーの配列）
        var propsForLobby = new[] { "CreaterName", "Message", "MatchType","GameVer","CharaID" };
        // 作成するルームのルーム設定を行う
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = initialProps;
        roomOptions.CustomRoomPropertiesForLobby = propsForLobby;
        roomOptions.IsVisible = !RoomCreUIs.Private_To.isOn;
        PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);

    }
    public void GameStart_Join()
    {
        PhotonNetwork.JoinRoom(RoomIDJoinUIs.RoomID_Input.text);
    }
    public void GameStart_Room(RoomInfo Room)
    {
        PhotonNetwork.JoinRoom(Room.Name);
    }
    public void GameStart_Random()
    {
        var expectedProps = new ExitGames.Client.Photon.Hashtable();
        expectedProps.Add("MatchType", MatchingDr.value);
        expectedProps.Add("GameVer", Application.version);
        if (MatchingDr.value == 3)
        {
            int CharaID = Suzuten_SelectUI.SelectSlots[0].x + Suzuten_SelectUI.SelectSlots[0].y * SelectUI.CharaXcounts - RaCharaCo;
            if (CharaID >= DB.Charas.Length) CharaID = -3;
            expectedProps.Add("CharaID", CharaID);
        }
        PhotonNetwork.JoinRandomRoom(expectedProps,2);
    }
    #endregion

    #region コールバック
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string RoomName = "Room" + Random.Range(1000, 10000);

        var initialProps = new ExitGames.Client.Photon.Hashtable();
        initialProps["CreaterName"] = PhotonNetwork.NickName;
        initialProps["Message"] = "ランダムマッチ";
        initialProps["MatchType"] = MatchingDr.value;
        initialProps["GameVer"] = Application.version;

        int CharaID = Suzuten_SelectUI.SelectSlots[0].x + Suzuten_SelectUI.SelectSlots[0].y * SelectUI.CharaXcounts - RaCharaCo;
        if (CharaID >= DB.Charas.Length) CharaID = -3;
        initialProps["CharaID"] = CharaID;
        // ロビーのルーム情報から取得できるカスタムプロパティ（キーの配列）
        var propsForLobby = new[] { "CreaterName", "Message","MatchType","GameVer","CharaID" };
        // 作成するルームのルーム設定を行う
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = initialProps;
        roomOptions.CustomRoomPropertiesForLobby = propsForLobby;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }
    RoomList roomList = new RoomList();
    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        roomList.Update(changedRoomList);
        RoomUIsSet();
    }
    void RoomUIsSet()
    {
        List<RoomInfo> Rooms = roomList.ToList();
        for (int i = 0; i < Mathf.Max(LobbyJoinUIs.RoomSinUIs.Count, Rooms.Count); i++)
        {
            if (i < Rooms.Count)
            {
                if (LobbyJoinUIs.RoomSinUIs.Count <= i)
                {
                    LobbyJoinUIs.RoomSinUIs.Add(Instantiate(LobbyJoinUIs.RoomSinUIs[0], LobbyJoinUIs.RoomSinUIs[0].transform.parent));
                }
                ConnectRoomSinUIs RoomSin = LobbyJoinUIs.RoomSinUIs[i];
                RoomSin.gameObject.SetActive((int)Rooms[i].CustomProperties["MatchType"] == MatchingDr.value);
                if ((int)Rooms[i].CustomProperties["MatchType"] == MatchingDr.value)
                {
                    if ((int)Rooms[i].CustomProperties["MatchType"] == 3)
                    {
                        int CharaID = Suzuten_SelectUI.SelectSlots[0].x + Suzuten_SelectUI.SelectSlots[0].y * SelectUI.CharaXcounts - RaCharaCo;
                        if (CharaID >= DB.Charas.Length) CharaID = -3;
                        RoomSin.gameObject.SetActive((int)Rooms[i].CustomProperties["CharaID"] == CharaID);
                    }
                    else RoomSin.gameObject.SetActive(true);
                }
                else RoomSin.gameObject.SetActive(false);

                RoomSin.Disp(Rooms[i]);
            }
            else LobbyJoinUIs.RoomSinUIs[i].gameObject.SetActive(false);
        }
    }
    public override void OnJoinedLobby()
    {
        roomList.Clear();
    }
    public override void OnLeftLobby()
    {
        roomList.Clear();
    }
    #endregion
}


