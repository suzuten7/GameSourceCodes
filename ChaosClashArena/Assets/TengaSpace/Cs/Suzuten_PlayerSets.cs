using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static CameraSettings_Gabu;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class Suzuten_PlayerSets : MonoBehaviourPunCallbacks, IPunObservable
{
    #region 変数
    static public Suzuten_PlayerSets PSet;

    [SerializeField] Suzuten_DataBase DB;
    [SerializeField] PlayerInputManager PIM;
    [SerializeField] Suzuten_BattleUI[] BUIs;
    [SerializeField] GameObject[] UIObjs;
    public Transform[] StartPos = new Transform[2];
    static public int StageID;
    static public int[] CharaID = new int[2];
    static public int[,] ACSetID = new int[2, 5];
    static public Suzuten_PlayerState[] PSs = new Suzuten_PlayerState[2];
    static public int[] WinCounts = new int[2];
    static public float[] SPs = new float[2];
    static public int RoundCount;
    static public int BattleTime;
    static public int StartTime;
    static public bool BattleFlag;
    static public int WinNums;
    static public int OutTime;
    /// <summary>
    /// <para>0=VS表示</para>
    /// <para>1=バトル時間  2=勝利ラウンド数</para>
    /// <para>3=バトルスピード</para>
    /// <para>4=アイテム出現間隔,5=アイテム出現数</para>
    /// <para>6=カオスボール出現割合</para>
    /// </summary>
    static public int[] BOP_Battle, BOPC_Battle = new int[7];
    /// <summary>
    /// <para>0=最大HP  1=HP自然回復速度</para>
    /// <para>2=最大MP  3=MP自然回復速度</para>
    /// <para>4=最大SP  5=SP自然回復速度</para>
    /// <para>6=初期SP%  7=ダメージSP増加%</para>
    /// </summary>
    static public int[] BOP_HMSP, BOPC_HMSP = new int[8];
    /// <summary>
    /// <para>0=地上移動速度  1=空中移動速度</para>
    /// <para>2=ブースト</para>
    /// <para>3=ジャンプ  4=落下</para>
    /// </summary>
    static public int[] BOP_Moves, BOPC_Moves = new int[5];
    /// <summary>
    /// <para>0=衝突ダメージ  1=ACダメージ</para>
    /// <para>2=キャラスタン耐性  3=ACスタン耐性</para>
    /// <para>4=スタン自然回復速度  5=スタン時間</para>
    /// <para>6=状態ダメージ  7=ACCT時間</para>
    /// </summary>
    static public int[] BOP_AtkStan, BOPC_AtkStan = new int[8];
    static bool StartOP = false;
    static public float StopTime = 0;
    int EndTime = 0;
    int SpeedAddTime = 0;
    float SpeedAddVal = 0;

    bool SceneChange = false;
    [System.NonSerialized]public int StartStay = 0;
    [System.NonSerialized]public bool[] PlayerOKs = new bool[2];
    [System.NonSerialized]public bool[] PlayerCos = new bool[2];
    bool LocalOK = false;
    [System.NonSerialized]public bool NetStay;
    int OKsWaits = 0;


    [System.NonSerialized]public int StartRound = 0;
    #endregion
    private void Awake()
    {
        StartOPSets();
    }
    private void Start()
    {
        #region 初期処理
        PSet = this;
        StopTime = 0;
        BattleFlag = false;
        WinNums = -2;
        RoundCount++;
        BattleTime = 60 * BOP_Battle[1];
        if (BOP_Battle[0] != 1) StartTime = 0;
        else StartTime = 60 * (RoundCount <= 1 ? 6 : 2);
        OutTime = 60 * 3;
        StartRound = RoundCount;
        #endregion
    }
    private void LateUpdate()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!PSsSet()) return;
        }
        if (MultiDisplayMode)
        {
            PIM.splitScreen = false;
            int count = Mathf.Min(Display.displays.Length, 2);
            for (int i = 0; i < count; ++i)
            {
                Display.displays[i].Activate();
            }
        }
        else PIM.splitScreen = true;
        if (!SettingOpen || !PhotonNetwork.OfflineMode)
        {
            if (BattleFlag)
            {
                if (StopTime < 0.02f) Time.timeScale = (BOP_Battle[3] * 0.01f);
                else
                {
                    Time.timeScale = (BOP_Battle[3] * 0.01f) * 0.1f;
                    StopTime -= 0.02f;
                }
                if (SpeedAddTime > 0) Time.timeScale *= 1f + SpeedAddVal;
            }
            else Time.timeScale = 1;
            InputSystem.ResumeHaptics();
        }
        else
        {
            Time.timeScale = 0;
            InputSystem.PauseHaptics();
        }
        for (int i = 0; i < 2; i++)
        {
            UIObjs[i].SetActive(!Settings_Bool[i, 1]);
        }
    }
    private void FixedUpdate()
    {
        #region プレイヤー存在判定
        if (!PhotonNetwork.OfflineMode)
        {
            NetStay = true;
            StartStay++;
            int PIDs = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            bool CoCheck = true;
            for (int i = 0; i < 2; i++)
            {
                if (!PlayerCos[i])
                {
                    CoCheck = false;
                    break;
                }
            }

            if (!CoCheck)
            {
                photonView.RPC(nameof(RpcPlCo), RpcTarget.All, PIDs);
                return;
            }
            OKsWaits++;
            if (OKsWaits>=120&&!LocalOK && (PIDs <= 0 || PlayerOKs[PIDs - 1]))
            {
                photonView.RPC(nameof(RpcPlOK), RpcTarget.All, PIDs);
                Startsd();
                LocalOK = true;
            }
            if (!PSsSet()) return;
            NetStay = false;
            if (!photonView.IsMine) return;
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    var PData = PhotonNetwork.CurrentRoom.Players[i + 1].CustomProperties;
                    for (int j = 0; j < 5; j++)
                    {
                        if (PData.ContainsKey("ACID_" + j)) ACSetID[i, j] = (int)PData["ACID_" + j];
                        else ACSetID[i, j] = j;
                    }
                }
            }
        }
        else if (!LocalOK)
        {
            LocalOK = true;
            Startsd();
        }
        #endregion
        #region 時間処理
        if (WinNums >= -1)
        {
            #region バトル後処理
            BattleFlag = false;
            EndTime++;
            if (EndTime == 1)
            {
                if (WinNums >= 0) WinCounts[WinNums]++;
            }
            if (EndTime >= 300)
            {
                bool WinCheck = false;
                for (int i = 0; i < WinCounts.Length; i++)
                {
                    if (WinCounts[i] >= BOP_Battle[2] && BOP_Battle[2] > 0) WinCheck = true;
                    SPs[i] = PSs[i].SP;
                }
                if (!SceneChange)
                {
                    if (!WinCheck)
                    {
                        SceneChange = true;
                        photonView.RPC(nameof(RpcSceneReload), RpcTarget.All);
                    }
                    else if (EndTime >= 450)
                    {
                        SceneChange = true;
                        photonView.RPC(nameof(RpcEndExit), RpcTarget.All);
                    }
                }
            }
            #endregion
        }
        else
        {
            #region バトル各処理
            if (StartTime > 0)
            {
                #region 開始前処理
                BattleFlag = false;
                StartTime--;
                #endregion
            }
            else if (BattleTime > 0)
            {
                //バトル中判定処理
                BattleFlag = true;
                BattleTime--;
                SpeedAddTime--;
                //HP0より上のプレイヤーカウント
                int PCount = 0;
                for (int i = 0; i < PSs.Length; i++)
                {
                    if (PSs[i].HP > 0) PCount++;
                }
                //プレイヤー数
                if (PCount == 1)
                {
                    //残弾用時間
                    OutTime--;
                    if (OutTime <= 0)
                    {
                        //勝ちプレイヤー設定
                        for (int i = 0; i < PSs.Length; i++)
                        {
                            if (PSs[i].HP > 0)
                            {
                                WinNums = i;
                                break;
                            }
                        }
                    }
                }
                else if (PCount <= 0)
                {
                    //引き分け
                    WinNums = -1;
                }
            }
            else if (BattleTime >= -120)
            {
                BattleFlag = false;
                BattleTime--;
            }
            else
            {
                BattleFlag = false;
                //タイムアップ判定処理
                int MaxPerPlayer = -1;
                bool Mults = false;
                int MaxPer = 0;
                //HP割合判定
                for (int i = 0; i < PSs.Length; i++)
                {
                    int HPPer = Mathf.RoundToInt(PSs[i].HP / PSs[i].CD.MHP * 1000);
                    if (HPPer > MaxPer)
                    {
                        MaxPerPlayer = i;
                        MaxPer = HPPer;
                        Mults = false;
                    }
                    else if (HPPer == MaxPer) Mults = true;
                }
                //同割合がいると引き分け
                if (!Mults) WinNums = MaxPerPlayer;
                else WinNums = -1;
            }
            #endregion
        }
        #endregion
    }
    #region 各メソッド
    /// <summary>プレイヤー生成処理</summary>
    void Startsd()
    {
        if (PhotonNetwork.OfflineMode)
        {
            for (int i = 0; i < 2; i++)
            {

                int ID = CharaID[i];

                if (ID < 0)
                {
                    switch (ID)
                    {
                        default:
                            while (true)
                            {
                                ID = Random.Range(0, DB.Charas.Length);
                                if (!DB.Charas[ID].RandomNoSelects && !DB.Charas[ID].UseBandChara)
                                {
                                    CharaID[i] = ID;
                                    break;
                                }
                            }
                            break;
                        case -2:
                            while (true)
                            {
                                ID = Random.Range(0, DB.Charas.Length);
                                if (!DB.Charas[ID].UseBandChara)
                                {
                                    CharaID[i] = ID;
                                    break;
                                }
                            }
                            break;
                        case -1:
                            ID = Random.Range(0, DB.Charas.Length);
                            CharaID[i] = ID;
                            break;
                    }
                }
                ACIDLoad(DB, i);
                GameObject InsPlayer = PhotonNetwork.Instantiate(DB.Charas[ID].CharaObj.name, StartPos[i].position, Quaternion.identity);
                PSs[i] = InsPlayer.GetComponent<Suzuten_PlayerState>();
                Suzuten_CharaData CD = DB.Charas[ID];
                PSs[i].CD = CD;
                PSs[i].HP = CD.MHP * (BOP_HMSP[0] * 0.01f);
                PSs[i].MP = CD.MMP * (BOP_HMSP[2] * 0.01f);
                if (SPs[i] >= 0) PSs[i].SP = SPs[i];
                else PSs[i].SP = CD.MSP * (BOP_HMSP[4] * 0.01f) * (CD.StartSPPer * 0.01f) * (BOP_HMSP[6] * 0.01f);
                PSs[i].CHST = CD.StanRegist * (BOP_AtkStan[2] * 0.01f);
                PSs[i].PI.enabled = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Transform Trans = PSs[i].transform;
                Trans.LookAt(PSs[i == 0 ? 1 : 0].RigObj.transform);
                Trans.eulerAngles = new Vector3(0, Trans.eulerAngles.y, 0);
            }
        }
        else
        {
            int PIDs = PhotonNetwork.LocalPlayer.ActorNumber;
            var PLCharas = PhotonNetwork.LocalPlayer.CustomProperties;
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + ":" + PIDs + ":" + PLCharas["CharaID"].ToString());
            foreach (var PLs in PhotonNetwork.CurrentRoom.Players.Values)
            {
                Debug.Log(PLs.NickName + ":" + PLs.ActorNumber + ":" + PLs.CustomProperties["CharaID"].ToString());
            }
            Suzuten_CharaData CD = DB.Charas[(int)PLCharas["CharaID"]];
            GameObject InsPlayer = PhotonNetwork.Instantiate(CD.CharaObj.name, StartPos[PIDs - 1].position, Quaternion.identity);
            PSs[PIDs - 1] = InsPlayer.GetComponent<Suzuten_PlayerState>();
            PSs[PIDs - 1].HP = CD.MHP * (BOP_HMSP[0] * 0.01f);
            PSs[PIDs - 1].MP = CD.MMP * (BOP_HMSP[2] * 0.01f);
            if (SPs[PIDs - 1] >= 0) PSs[PIDs - 1].SP = SPs[PIDs - 1];
            else PSs[PIDs - 1].SP = CD.MSP * (BOP_HMSP[4] * 0.01f) * (CD.StartSPPer * 0.01f) * (BOP_HMSP[6] * 0.01f);
            PSs[PIDs - 1].CHST = CD.StanRegist * (BOP_AtkStan[2] * 0.01f);
            PSs[PIDs - 1].PI.enabled = true;
            Debug.Log("SetOk");
        }
    }
    /// <summary>プレイヤー変数設定</summary>
    bool PSsSet()
    {
        bool OKs = true;
        foreach (var PSd in FindObjectsOfType<Suzuten_PlayerState>())
        {
            PSs[PSd.photonView.OwnerActorNr - 1] = PSd;
        }
        for (int i = 0; i < 2; i++)
        {
            if (PSs[i] != null)
            {
                if (LocalOK) PSs[i].PI.enabled = true;
                else OKs = false;
            }
            else OKs = false;
        }
        return OKs;

    }
    /// <summary>プレイヤーチェック</summary>
    static public bool PSsCheck()
    {
        bool OKs = true;
        for (int i = 0; i < 2; i++)
        {
            if (PSs[i] == null || !PSs[i].PI.enabled) OKs = false;
        }
        return OKs;
    }
    /// <summary>アクション設定ロード</summary>
    static public void ACIDLoad(Suzuten_DataBase DB, int PID)
    {
        for (int j = 0; j < 5; j++)
        {
            if (CharaID[PID] >= 0)
            {
                Suzuten_CharaData CD = DB.Charas[CharaID[PID]];
                int SetID = PlayerPrefs.GetInt("ACIDs_" + PID + "_" + j + "_" + CD.name, j);
                ACSetID[PID, j] = Mathf.Clamp(SetID, -1, CD.Actions.Length - 1);
            }
            else ACSetID[PID, j] = j;
        }
    }
    /// <summary>アクション設定セーブ</summary>
    static public void ACIDSave(Suzuten_DataBase DB, int PID)
    {
        for (int j = 0; j < 5; j++)
        {
            if (CharaID[PID] >= 0)
            {
                Suzuten_CharaData CD = DB.Charas[CharaID[PID]];
                PlayerPrefs.SetInt("ACIDs_" + PID + "_" + j + "_" + CD.name, ACSetID[PID, j]);
            }
        }
    }
    /// <summary>バトル設定プリセット</summary>
    static public void BOPPriSets(int ID)
    {
        if (ID > 1) BOPPriSets(0);
        switch (ID)
        {
            case 0:
                BOPC_Battle[0] = 1;
                BOPC_Battle[1] = 99;
                BOPC_Battle[2] = 2;
                BOPC_Battle[3] = 100;
                BOPC_Battle[4] = 100;
                BOPC_Battle[5] = 100;
                BOPC_Battle[6] = 25;
                for (int i = 0; i < BOPC_HMSP.Length; i++) BOPC_HMSP[i] = 100;
                for (int i = 0; i < BOPC_Moves.Length; i++) BOPC_Moves[i] = 100;
                for (int i = 0; i < BOPC_AtkStan.Length; i++) BOPC_AtkStan[i] = 100;
                break;
            case 1:
                BOPC_Battle[0] = PlayerPrefs.GetInt("BOP_Battle_0", 1);
                BOPC_Battle[1] = PlayerPrefs.GetInt("BOP_Battle_1", 99);
                BOPC_Battle[2] = PlayerPrefs.GetInt("BOP_Battle_2", 2);
                BOPC_Battle[3] = PlayerPrefs.GetInt("BOP_Battle_3", 100);
                BOPC_Battle[4] = PlayerPrefs.GetInt("BOP_Battle_4", 100);
                BOPC_Battle[5] = PlayerPrefs.GetInt("BOP_Battle_5", 100);
                BOPC_Battle[6] = PlayerPrefs.GetInt("BOP_Battle_6", 25);
                for (int i = 0; i < BOPC_HMSP.Length; i++) BOPC_HMSP[i] = PlayerPrefs.GetInt("BOP_HMSP_" + i, 100);
                for (int i = 0; i < BOPC_Moves.Length; i++) BOPC_Moves[i] = PlayerPrefs.GetInt("BOP_Moves_" + i, 100);
                for (int i = 0; i < BOPC_AtkStan.Length; i++) BOPC_AtkStan[i] = PlayerPrefs.GetInt("BOP_AtkStan_" + i, 100);
                break;
            case 2:
                BOPC_Moves[2] = 120;
                BOPC_Moves[3] = 120;
                BOPC_Moves[4] = 150;
                BOPC_AtkStan[0] = 130;
                BOPC_AtkStan[1] = 70;
                BOPC_AtkStan[6] = 70;
                break;
            case 3:
                BOP_HMSP[0] = 1;
                break;
            case 4:
                BOPC_AtkStan[2] = 300;
                BOPC_AtkStan[3] = 300;
                BOPC_AtkStan[5] = 1000;
                break;
        }
        BOP_Battle = BOPC_Battle.ToArray();
        BOP_HMSP = BOPC_HMSP.ToArray();
        BOP_Moves = BOPC_Moves.ToArray();
        BOP_AtkStan = BOPC_AtkStan.ToArray();
    }
    /// <summary>勝ち数リセット</summary>
    static public void WinResets()
    {
        for (int i = 0; i < WinCounts.Length; i++)
        {
            WinCounts[i] = 0;
            SPs[i] = -1;
        }
        RoundCount = 0;
    }
    /// <summary>初期化用</summary>
    static public void StartOPSets()
    {
        if (!StartOP)
        {
            StageID = 0;
            CharaID[0] = -1;
            CharaID[1] = -1;
            WinResets();
            BOPPriSets(PlayerPrefs.GetInt("BOPSet", 0));
            StartOP = true;
        }
    }
    /// <summary>バトルメッセージ</summary>
    static public void InfoDisplays(string InfoTx, Vector2 XPos, float FontSize = 36, float MoveSpeed = 15, float AlphaSpeed = 5, int PID = 0)
    {
        PSet.InfoDr(InfoTx, XPos, FontSize, MoveSpeed, AlphaSpeed, PID);
    }
    /// <summary>バトルメッセージ</summary>
    public void InfoDr(string InfoTx, Vector2 XPos, float FontSize = 36, float MoveSpeed = 15, float AlphaSpeed = 5, int PID = 0)
    {
        photonView.RPC(nameof(RpcInfoDisp), RpcTarget.All, InfoTx, XPos, FontSize, MoveSpeed, AlphaSpeed, PID);
    }
    /// <summary>バトルメッセージ</summary>
    [PunRPC]
    public void RpcInfoDisp(string InfoTx, Vector2 XPos, float FontSize, float MoveSpeed, float AlphaSpeed, int PID)
    {
        for (int i = 0; i < 2; i++)
        {
            Suzuten_InfoTxs InfoTxIns = Instantiate(PSet.BUIs[i].InfoTxs, PSet.BUIs[i].InfoTxs.RT.parent);
            InfoTxIns.gameObject.SetActive(true);
            InfoTxIns.Tx.text = InfoTx;
            InfoTxIns.Tx.fontSize = FontSize;
            InfoTxIns.MoveSpeed = MoveSpeed;
            InfoTxIns.AlphaSpeed = AlphaSpeed;
            if (PID <= 0 || PID == PhotonNetwork.LocalPlayer.ActorNumber) InfoTxIns.RT.localPosition += new Vector3(Random.Range(XPos.x, XPos.y), 0, 0);
            else InfoTxIns.RT.localPosition -= new Vector3(Random.Range(XPos.x, XPos.y), 0, 0);

        }
    }
    /// <summary>シーン再読み込み</summary>
    [PunRPC]
    void RpcSceneReload()
    {
        Debug.Log("シーン変化");
        SceneChangeUIs.SCUIDisp();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>終了シーン変更</summary>
    [PunRPC]
    void RpcEndExit()
    {
        SceneChangeUIs.SCUIDisp();
        if (!PhotonNetwork.OfflineMode) SceneManager.LoadSceneAsync(2);
        else SceneManager.LoadSceneAsync(1);
        PhotonNetwork.LeaveRoom();
    }
    /// <summary>プレイヤー接続送信</summary>
    [PunRPC]
    void RpcPlCo(int ID)
    {
        if (!photonView.IsMine) return;
        PlayerCos[ID] = true;
    }
    /// <summary>プレイヤー生成送信</summary>
    [PunRPC]
    void RpcPlOK(int ID)
    {
        if (!photonView.IsMine) return;
        PlayerOKs[ID] = true;
    }
    /// <summary>時間変化</summary>
    static public void TimeChage(int Val)
    {
        PSet.photonView.RPC(nameof(RpcTimeChange), RpcTarget.All, Val);
    }
    [PunRPC]
    void RpcTimeChange(int Val)
    {
        if (BattleTime > 0)
        {
            BattleTime = Mathf.Max(0, BattleTime+ Val);
        }
    }
    /// <summary>時間速度変化</summary>
    static public void SpeedAdds(int Times ,float Val)
    {
        PSet.photonView.RPC(nameof(RpcSpeedAdds), RpcTarget.All, Times, Val);
    }
    [PunRPC]
    void RpcSpeedAdds(int Times, float Val)
    {
        SpeedAddTime = Times;
        SpeedAddVal = Val;
    }
    #endregion
    /// <summary>変数同期用</summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            var ACP1 = new List<int>();
            var ACP2 = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                ACP1.Add(ACSetID[0, i]);
                ACP2.Add(ACSetID[1, i]);
            }
            stream.SendNext(ACP1.ToArray());
            stream.SendNext(ACP2.ToArray());

            stream.SendNext(PlayerCos);
            stream.SendNext(PlayerOKs);
            stream.SendNext(SPs);

            stream.SendNext(WinCounts);
            stream.SendNext(BattleTime);
            stream.SendNext(StartTime);
            stream.SendNext(BattleFlag);
            stream.SendNext(WinNums);
            stream.SendNext(OutTime);

            stream.SendNext(BOP_Battle);
            stream.SendNext(BOP_HMSP);
            stream.SendNext(BOP_Moves);
            stream.SendNext(BOP_AtkStan);

            stream.SendNext(SpeedAddTime);
            stream.SendNext(SpeedAddVal);

        }
        else
        {
            var ACP1 = (int[])stream.ReceiveNext();
            var ACP2 = (int[])stream.ReceiveNext();
            for (int i = 0; i < 5; i++)
            {
                ACSetID[0, i] = ACP1[i];
                ACSetID[1, i] = ACP2[i];
            }
            PlayerCos = (bool[])stream.ReceiveNext();
            PlayerOKs = (bool[])stream.ReceiveNext();
            SPs = (float[])stream.ReceiveNext();

            WinCounts = (int[])stream.ReceiveNext();
            BattleTime = (int)stream.ReceiveNext();
            StartTime = (int)stream.ReceiveNext();
            BattleFlag = (bool)stream.ReceiveNext();
            WinNums = (int)stream.ReceiveNext();
            OutTime = (int)stream.ReceiveNext();

            BOP_Battle = (int[])stream.ReceiveNext();
            BOP_HMSP = (int[])stream.ReceiveNext();
            BOP_Moves = (int[])stream.ReceiveNext();
            BOP_AtkStan = (int[])stream.ReceiveNext();

            SpeedAddTime = (int)stream.ReceiveNext();
            SpeedAddVal = (float)stream.ReceiveNext();

        }
    }
}
