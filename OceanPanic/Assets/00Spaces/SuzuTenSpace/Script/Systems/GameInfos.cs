using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static DataBase;

public class GameInfos : MonoBehaviourPun, IPunObservable, IOnEventCallback
{
    /// <summary></summary>
    #region 変数
    static public GameInfos GInfo;
    [SerializeField] int TimeLimit;
    public int PlayerAddLimit;
    /// <summary>ゲーム時間</summary>
    public int GameTime;
    /// <summary>総合経過時間</summary>
    public int ToGameTime;
    /// <summary>開始時間</summary>
    public int STTime;
    /// <summary>鬼待機時間</summary>
    public const int OniWaitTime = 1200;
    /// <summary>途中参加観戦モード時間</summary>
    public const int ViewModeTime = 1800;
    /// <summary>制限時間-終</summary>
    public int TLimits { get 
        {
            if (STTime <= ViewModeTime)
            {
                int TmBa = TimeLimit + (PlayerAddLimit * Mathf.Max(0, FePlanTotal - 1));
                int TmCh = 0;
                for (int i = 0; i < FugiAcs.Length; i++)
                {
                    if (FugiAcs[i] == null) continue;
                    switch (FugiAcs[i].PSta.PassL.TryGetValue((int)Fugi_PassE.やくしろ, out var OPassyaku) ? OPassyaku : 0)
                    {
                        default:break;
                        case 1: TmCh -= 60 * 10; break;
                        case 2: TmCh -= 60 * 20; break;
                        case 3: TmCh -= 60 * 30; break;
                        case 4: TmCh -= 60 * 45; break;
                    }
                }
                for (int i = 0; i < PilotAcs.Length; i++)
                {
                    if (PilotAcs[i] == null) continue;
                    switch (PilotAcs[i].PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.時間延長, out var OPassTimeAdd) ? OPassTimeAdd : 0)
                    {
                        default:break;
                        case 1: TmCh += (int)(TmBa * 0.05f); break;
                        case 2: TmCh += (int)(TmBa * 0.10f); break;
                        case 3: TmCh += (int)(TmBa * 0.15f); break;
                        case 4: TmCh += (int)(TmBa * 0.20f); break;
                        case 5: TmCh += (int)(TmBa * 0.30f); break;
                    }
                    if (PilotAcs[i].PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.全力暴走,out var RampLV)&&RampLV>0) TmCh -= (int)(TmBa * 0.5f);
                }
                for (int i = 0; i < VisitAcs.Length; i++)
                {
                    if (VisitAcs[i] == null) continue;
                    switch (VisitAcs[i].PSta.PassL.TryGetValue((int)Oni_Visit_PassE.時間延長, out var OPassTimeAdd) ? OPassTimeAdd : 0)
                    {
                        default: break;
                        case 1: TmCh += (int)(TmBa * 0.05f); break;
                        case 2: TmCh += (int)(TmBa * 0.10f); break;
                        case 3: TmCh += (int)(TmBa * 0.15f); break;
                        case 4: TmCh += (int)(TmBa * 0.20f); break;
                        case 5: TmCh += (int)(TmBa * 0.30f); break;
                    }
                }
                MTime = Mathf.Max(ViewModeTime, TmBa + TmCh);
            }
            return MTime;
        }
    }
    int MTime =0;
    /// <summary>逃走者総数</summary>
    public int FePlanTotal;
    /// <summary>逃走者生存数</summary>
    public int FePlanCount;

    public Dictionary<string, int> Messages = new Dictionary<string, int>();

    /// <summary>ゲーム終了</summary>
    public bool End;
    public int EndTime = 0;

    public bool NPlaySet = false;
    public bool Connets = false;
    public bool EndSChange = false;
    public bool Ramps = false;
    public int Deaths = 0;
    bool RampMes = false;

    static public GameObject[] Planctons = new GameObject[0];
    static public Player_F_PlanctonAction[] FugiAcs = new Player_F_PlanctonAction[0];
    static public Player_Pilot_WhalesAction[] PilotAcs = new Player_Pilot_WhalesAction[0];
    static public Player_Visit_WhalesAction[] VisitAcs = new Player_Visit_WhalesAction[0];
    #endregion
    void Start()
    {
        GInfo = this;
        FindSets();
    }

    void FixedUpdate()
    {
        FindSets();
        if (!photonView.IsMine) return;
        var RoomHash = PhotonNetwork.CurrentRoom.CustomProperties;
        Connets = true;
        #region 逃走者カウント
        FePlanTotal = 0;
        FePlanCount = 0;
        for(int i=0;i<FugiAcs.Length;i++)
        {
            if (FugiAcs[i] == null) continue;
            FePlanTotal++;
            if (FugiAcs[i].PSta.HP > 0&&!FugiAcs[i].PSta.GostModes) FePlanCount++;
        }
        #endregion
        #region 時間管理・暴走モード
        STTime++;
        if (STTime > OniWaitTime)
        {
            GameTime++;
            if(!End)ToGameTime++;
        }
        GameTime = Mathf.Min(GameTime, TLimits);
        if((TLimits * 0.75f <= GameTime) && !Ramps)
        {
            Ramps = true;
            Ev_Message_Send("<color=#FF0000>制限時間残り25%</color>",MessageE.全員);
        }
        var Pilotd = FindFirstObjectByType<Player_Pilot_WhalesAction>();
        if (Pilotd != null && !GInfo.Ramps && Pilotd.PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.全力暴走,out var RampLV)&&RampLV>0)
        {
            GInfo.Ramps = true;
        }
        if (Ramps&&!RampMes)
        {
            RampMes = true;
            Ev_Message_Send("<color=#FF0000>鬼が暴走モードに突入！！！</color>", MessageE.全員);
        }
        #endregion
        #region ゲーム終了条件
        if (!End&& STTime > OniWaitTime)
        {
            if (GameTime >= TLimits) End = true;
            if (FePlanTotal > 0)
            {
                if (FePlanCount <= 0) End = true;
                if (RoomHash.TryGetValue("GameOption2", out var Op2Val) && (bool)Op2Val && Deaths >= Mathf.RoundToInt(FePlanTotal * 1.5f)) End = true;
            }
        }
        #endregion
        #region 終了後処理
        if (End)
        {
            if (!NPlaySet)
            {
                NPlaySet = true;
                var CRoom = PhotonNetwork.CurrentRoom;
                var RoomHashs = CRoom.CustomProperties;
                RoomHashs["GameStarts"] = false;
                CRoom.SetCustomProperties(RoomHashs);
            }
            EndTime++;
            if (EndTime >= 600&&!EndSChange)
            {
                EndSChange = true;
                PhotonNetwork.DestroyAll();
                PhotonNetwork.LoadLevel(0);
            }
        }
        #endregion
    }
    void FindSets()
    {
        Planctons = GameObject.FindGameObjectsWithTag("Fugitive");
        FugiAcs = FindObjectsByType<Player_F_PlanctonAction>(FindObjectsSortMode.None);
        PilotAcs = FindObjectsByType<Player_Pilot_WhalesAction>(FindObjectsSortMode.None);
        VisitAcs = FindObjectsByType<Player_Visit_WhalesAction>(FindObjectsSortMode.None);
    }
    static public void TimeAdds(int Value)
    {
        if (GInfo.Ramps) Value /= 2;
        GInfo.photonView.RPC(nameof(Rpc_TimeAdds), RpcTarget.MasterClient, Value);
    }
    [PunRPC]
    void Rpc_TimeAdds(int Value)
    {
        if (!photonView.IsMine) return;
        GameTime -= Value*60;
        Ev_Message_Send("制限時間+" + (Value)+"秒", MessageE.全員);
    }
    static public void DeathAdds()
    {
        GInfo.photonView.RPC(nameof(Rpc_DeathAdds), RpcTarget.MasterClient);
    }
    [PunRPC]
    void Rpc_DeathAdds()
    {
        if (!photonView.IsMine) return;
        Deaths++;
    }

    #region イベント送受信
    public enum EventCodes : byte//自作イベント：byteは扱うデータ(0 ～ 255)
    {
        Ev_Message,
    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;

        EventCodes eventCode = (EventCodes)photonEvent.Code;//今回のイベントコードを格納（型変換）
        object[] data = (object[])photonEvent.CustomData;//インデクサーとCustomDataKeyを介して、イベントのカスタムデータにアクセスします
        switch (eventCode)
        {
            case EventCodes.Ev_Message: Ev_Message_Rece(data); break;
        }

    }
    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);//追加する
    }
    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);//削除する
    }
    public enum MessageE
    {
        全員 = 0,
        逃走者 = 1,
        鬼 = 2,
    }
    static public void Ev_Message_Send(string Message, MessageE SendType)
    {
        object[] info = new object[2];
        info[0] = Message;
        info[1] = (int)SendType;
        PhotonNetwork.RaiseEvent
        (
            (byte)EventCodes.Ev_Message,
            info,//送るもの（プレイヤーデータ）
            new RaiseEventOptions { Receivers = ReceiverGroup.All },//ルームマスターだけに送信される設定
            new SendOptions { Reliability = true }//信頼性の設定：信頼できるのでプレイヤーに送信される
        );
    }
    void Ev_Message_Rece(object[] gets)
    {
        Messages.TryAdd((string)gets[0], (int)gets[1]);
    }
    #endregion

    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Connets);
            stream.SendNext(STTime);
            stream.SendNext(GameTime);
            stream.SendNext(ToGameTime);
            stream.SendNext(FePlanTotal);
            stream.SendNext(FePlanCount);
            stream.SendNext(Ramps);
            stream.SendNext(End);
            stream.SendNext(EndTime);
            stream.SendNext(MTime);
            stream.SendNext(Deaths);
        }
        else
        {
            Connets = (bool)stream.ReceiveNext();
            STTime = (int)stream.ReceiveNext();
            GameTime = (int)stream.ReceiveNext();
            ToGameTime = (int)stream.ReceiveNext();
            FePlanTotal = (int)stream.ReceiveNext();
            FePlanCount = (int)stream.ReceiveNext();
            Ramps = (bool)stream.ReceiveNext();
            End = (bool)stream.ReceiveNext();
            EndTime = (int)stream.ReceiveNext();
            MTime = (int)stream.ReceiveNext();
            Deaths = (int)stream.ReceiveNext();
        }
    }
}
