using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataBase;
using static PlayerValue;
using static Manifesto;
using static Statics;
public class BattleManager : MonoBehaviourPunCallbacks,IPunObservable
{
    static public BattleManager BTManager;

    public int Stage;
    public int Dife;
    public bool Chaos;
    public int Time;
    public int DeathCount;
    public int Star;
    public bool Win;
    public bool End;
    public List<string> Messages = new List<string>();
    [System.NonSerialized] public Data_Stage StageD = null;
    [System.NonSerialized] public int FDeathStar;
    [System.NonSerialized] public List<State_Base> StateList = new List<State_Base>();
    [System.NonSerialized] public List<State_Hit> HitList = new List<State_Hit>();
    [System.NonSerialized] public List<State_Base> PlayerList = new List<State_Base>();
    [System.NonSerialized] public List<State_Base> BossList = new List<State_Base>();
    [System.NonSerialized] public List<Enemy_WaveSpawne> WaveSpList = new List<Enemy_WaveSpawne>();
    [System.NonSerialized] public List<PCamLockdObj> PCamLockdList = new List<PCamLockdObj>();
    [System.NonSerialized] public State_Base[] LocalCharas = new State_Base[4];
    [System.NonSerialized] public List<Class_Save_GeneData> DropGenes = new List<Class_Save_GeneData>();

    float ListTimer = 0;
    bool EndSave = false;

    void Awake()
    {
        BTManager = this;
        if (!PhotonNetwork.InRoom) return;
        if (photonView.IsMine)
        {
            Stage = StageID;
            Dife = DifeMode;
            Chaos = ChaosSet;
            StageD = DB.Stages[StageID];
            Time = StageD.TimeLimSec * 60;
            FDeathStar = Mathf.RoundToInt(StageD.DeathStar * (1f + (BTManager.PlayerList.Count - 1) * 0.3f));
        }
    }
    private void Update()
    {
        StageD = DB.Stages[Stage];
        ListTimer += UnityEngine.Time.fixedUnscaledDeltaTime;
        if (ListTimer <= 5f) return;
        ListTimer = 0;
        ListReflesh();
    }
    void FixedUpdate()
    {
        if (!PhotonNetwork.InRoom) return;
        if (End)
        {
            if (Win)
            {
                if (PhotonNetwork.OfflineMode) Stages.SoloStars[Stage] = Mathf.Max(Stages.SoloStars[Stage], Star);
                else Stages.MultStars[Stage] = Mathf.Max(Stages.MultStars[Stage], Star);
            }
            if (!EndSave)
            {
                if (Win)
                {
                    var StageD = DB.Stages[Stage];
                    float CGDrops = StageD.ClearChaosGrain;
                    float DropMult = 1f;
                    switch (Dife + (Chaos ? 1: 0))
                    {
                        case 0: CGDrops *= 0.6f;DropMult = 0.5f; break;
                        case 2: CGDrops *= 1.3f; DropMult = 1.3f; break;
                        case 3: CGDrops *= 1.6f; DropMult = 1.6f; break;
                        case 4: CGDrops *= 2.0f; DropMult = 2.0f; break;
                    }
                    var DropCo = new Vector2Int(Mathf.RoundToInt(StageD.GeneDropCount.x * DropMult),
                         Mathf.RoundToInt(StageD.GeneDropCount.y * DropMult));
                    Genes.ChaosGrain += Mathf.RoundToInt(CGDrops);
                    if (StageD.GeneDropTypes.Length > 0)
                    {
                        for(int i=0;i< V2Int_Rand(DropCo); i++)
                        {
                            var Type = StageD.GeneDropTypes[Random.Range(0, StageD.GeneDropTypes.Length)];
                            var AddGeneD = GeneAdds(StageD.Name + "-" + (Genes.Datas.Count + 1), (int)Type);
                            DropGenes.Add(AddGeneD);
                            Genes.Datas.Add(AddGeneD);
                        }
                    }
                }

                EndSave = true;
                Save();
            }
        }
        if (!photonView.IsMine) return;
        if (StageD == null) return;
        FDeathStar = Mathf.RoundToInt(StageD.DeathStar * (1f + (BTManager.PlayerList.Count - 1) * 0.3f));
        var CRoom = PhotonNetwork.CurrentRoom;
        CRoom.IsOpen = false;
        if (!End)
        {
            if (Time > 0) Time--;
            var DifHPPer = 0f;
            if (!StageD.NoClears)
            {
                if (WaveSpList.Count <= 0 || StageD.DifencePer > 0)
                {
                    var BossCheck = BossList.Count > 0;
                    for (int i = 0; i < BossList.Count; i++)
                    {
                        if (BossList[i] == null) continue;
                        if (StageD.DifencePer <= 0 && BossList[i].HP > 0) BossCheck = false;
                        if (StageD.DifencePer > 0 && BossList[i].Team == 0)
                        {
                            DifHPPer = BossList[i].HP / Mathf.Max(1f, BossList[i].FMHP) * 100f;
                            if (BossList[i].HP > 0) BossCheck = false;
                        }
                    }
                    if (StageD.DifencePer <= 0) Win = BossCheck;
                    else Win = !BossCheck;
                    if (BossCheck || Time <= 0) End = true;
                }
                else
                {
                    var WaveCheck = true;
                    for (int i = 0; i < WaveSpList.Count; i++)
                    {
                        if (WaveSpList[i] == null) continue;
                        if (!WaveSpList[i].Clear) WaveCheck = false;
                    }
                    Win = WaveCheck;
                    if (WaveCheck || Time <= 0) End = true;
                }
            }
            if (StageD.TimeLimSec <= 0) End = false;
            Star = 2 + Dife;
            if (Chaos) Star++;
            if (StageD.DifencePer <= 0 && Time <= StageD.TimeStar * 60) Star--;
            if (StageD.DifencePer > 0 && DifHPPer <= StageD.DifencePer) Star--;
            if (DeathCount > FDeathStar) Star--;
            if (StageD.DifencePer <= 0 && Time <= 0) Star = 0;
            if (StageD.DifencePer > 0 && DifHPPer <= 0) Star = 0;
        }

    }
    void ListReflesh()
    {
        StateList.RemoveAll(x => x == null);
        HitList.RemoveAll(x => x == null);
        PlayerList.RemoveAll(x => x == null);
        BossList.RemoveAll(x => x == null);
        WaveSpList.RemoveAll(x => x == null);
        PCamLockdList.RemoveAll(x => x == null);
    }
    public void DeathAdd()
    {
        photonView.RPC(nameof(RPC_DeathAdd), RpcTarget.All);
    }
    public void SEPlay(Class_Base_SEPlay SEPlays,Vector3 Pos,bool Local = false)
    {
        SEPlay(SEPlays.Clip, Pos, SEPlays.Volume, SEPlays.Pitch, Local);
    }
    public void SEPlay(AudioClip SEClip, Vector3 Pos, float Volume, float Pitch,bool Local=false)
    {
        var SEID = DB.SEs.IndexOf(SEClip);
        if (SEID < 0) return;
        if (!Local) photonView.RPC(nameof(RPC_SEPlay), RpcTarget.All, SEID, Pos, Volume, Pitch);
        else RPC_SEPlay(SEID, Pos, Volume, Pitch);
    }
    public void MessageAdd(string Message)
    {
        photonView.RPC(nameof(RPC_MessageAdd), RpcTarget.All,Message);
    }
    [PunRPC]
    void RPC_DeathAdd()
    {
        if (!photonView.IsMine) return;
        DeathCount++;
    }
    [PunRPC]
    void RPC_SEPlay(int SEID, Vector3 Pos, float Volume, float Pitch)
    {
        int VCount = Mathf.CeilToInt(Volume / 100f);
        for (int i = 0; i < VCount; i++)
        {
            var SEObj = Instantiate(DB.SEObj, Pos, Quaternion.identity);
            SEObj.clip = DB.SEs[SEID];
            if (i == VCount - 1) SEObj.volume = (Volume * 0.01f) % 1f;
            else SEObj.volume = 1f;
            SEObj.pitch = Pitch / 100f;
            if (Pitch < 0) SEObj.time = DB.SEs[SEID].length - 0.01f;
            SEObj.Play();
            ObjStrageParent(SEObj.gameObject, "SEs");
            Destroy(SEObj.gameObject, 10f);
        }
    }
    [PunRPC]
    void RPC_MessageAdd(string Message)
    {
        Messages.Add(Message);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = LocalizationManager.Instance != null 
            ? string.Format(LocalizationManager.Instance.GetText("SYSTEM_LEFT_ROOM"), otherPlayer.NickName)
            : otherPlayer.NickName + " left the room.";
        Messages.Add(msg);
    }
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Stage);
            stream.SendNext(Dife);
            stream.SendNext(Chaos);
            stream.SendNext(Time);
            stream.SendNext(DeathCount);
            stream.SendNext(Star);
            stream.SendNext(FDeathStar);
            stream.SendNext(Win);
            stream.SendNext(End);
        }
        else
        {
            Stage = (int)stream.ReceiveNext();
            Dife = (int)stream.ReceiveNext();
            Chaos = (bool)stream.ReceiveNext();
            Time = (int)stream.ReceiveNext();
            DeathCount = (int)stream.ReceiveNext();
            Star = (int)stream.ReceiveNext();
            FDeathStar = (int)stream.ReceiveNext();
            Win = (bool)stream.ReceiveNext();
            End = (bool)stream.ReceiveNext();
        }
    }

    public void Clear()
    {
        Debug.Log("Clear");
        End = true;
        Win = true;
    }
    static public bool ChaosCheck(Enum_Stage StageN)
    {
        return BTManager.Chaos && BTManager.Stage == (int)StageN;
    }
}
