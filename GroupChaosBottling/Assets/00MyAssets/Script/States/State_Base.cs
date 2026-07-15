using Photon.Pun;
using UnityEngine;
using static DataBase;
using static Statics;
using static BattleManager;
using System.Collections.Generic;
using System.Linq;
using static PlayerValue;
using static Manifesto;
using static Calculation;
using NaughtyAttributes;
using Unity.VisualScripting;

public class State_Base : MonoBehaviourPun
{
    #region インスペクター変数
    [Foldout("設定")]public string Name;
    [Foldout("設定")] public Rigidbody Rig;
    [Foldout("設定")] public Transform CamTrans;
    [Foldout("設定")] public float Hight;
    [Foldout("設定")] public float SizeAdd;
    [Foldout("設定"),Tooltip("チーム")] public int Team;
    [Foldout("設定"), Tooltip("プレイヤー")] public bool Player;
    [Foldout("設定"), Tooltip("ボス")] public bool Boss;
    [Foldout("設定"), Tooltip("不死")] public bool Undet;
    [Foldout("設定")] public Class_Base_SEPlay DamageSE;
    [Foldout("設定")] public Class_Base_SEPlay DeathSE;
    [Foldout("設定"), Tooltip("死亡エフェクト")] public GameObject DeathEffect;
    [Foldout("設定"), Tooltip("数値同期間隔(秒)")] public float StreamTime;
    [Foldout("設定"), Tooltip("プレイヤー用変数")] public Player_BattleValues PLValues;
    [Foldout("設定"), Tooltip("プレイヤー用キャラ")] public Player_CharaSet PLCharaSet;
    [Foldout("設定"), Tooltip("武器表示用変数")] public State_WeponValues WepValues;
    //
    [Foldout("ステータス"), Tooltip("最大HP")]public int MHP;
    [Foldout("ステータス"), Tooltip("秒間HP回復速度")] public float HPRegene;
    [Foldout("ステータス"), Tooltip("最大MP(移動力)")] public int MMP;
    [Foldout("ステータス"), Tooltip("秒間MP回復速度")] public float MPRegene;
    [Foldout("ステータス"), Tooltip("秒間SP回復速度")] public float SPRegene;
    [Foldout("ステータス"), Tooltip("最大ブレイク値")] public int MBreak;
    [Foldout("ステータス"), Tooltip("ブレイク時間")] public int BreakTime;
    [Foldout("ステータス"), Tooltip("ブレイクHP減少%")] public float BreakHPRemPer;
    [Foldout("ステータス"), Tooltip("攻撃力")] public int Atk;
    [Foldout("ステータス"), Tooltip("防御力")] public int Def;
    [Foldout("ステータス"), Tooltip("近距離軽減%")] public float ShortCut;
    [Foldout("ステータス"), Tooltip("遠距離軽減%")] public float RangeCut;
    //
    [Foldout("数値")]public float HP;
    [Foldout("数値")] public float MP;
    [Foldout("数値")] public float SP;
    [Foldout("数値")] public float SPB;
    [Foldout("数値")] public float BreakV;
    [Foldout("数値")] public int BreakT;
    [Foldout("数値")] public bool Ground;
    [Foldout("数値")] public int DeathTime;
    [Foldout("数値")] public State_Base Target;
    [Foldout("数値")] public State_Hit TargetHit;
    [Foldout("数値")] public State_Base Fixation;
    [Foldout("数値")] public List<Class_Sta_BufInfo> Bufs = new List<Class_Sta_BufInfo>();
    //

    [Foldout("変数")] public bool LowMP;
    [Foldout("変数")] public int DashTime;
    [Foldout("変数")] public bool GroundB;

    [Foldout("変数")] public Data_Atk AtkD;
    [Foldout("変数")] public int AtkSlot;
    [Foldout("変数")] public bool AtkBack;
    [Foldout("変数")] public int AtkTime;
    [Foldout("変数")] public int AtkBranch;

    [Foldout("変数")] public float SpeedRem;
    [Foldout("変数")] public bool NoJump;
    [Foldout("変数")] public bool NoDash;
    [Foldout("変数")] public bool Aiming;
    [Foldout("変数")] public bool NGravity;
    [Foldout("変数")] public bool NoDamage;

    [Foldout("変数")] public int Anim_MoveID;
    [Foldout("変数")] public int Anim_AtkID;
    [Foldout("変数")] public float Anim_AtkSpeed;
    [Foldout("変数")] public int Anim_OtherID;

    #endregion
    #region 内部変数

    public Dictionary<int,Class_Sta_AtkCT> AtkCTs = new Dictionary<int,Class_Sta_AtkCT>();
    public Dictionary<int, Dictionary<State_Base, int>> MultHit = new Dictionary<int, Dictionary<State_Base, int>>();
    public Dictionary<int,GameObject> BufEffects = new Dictionary<int,GameObject>();
    public Dictionary<int, int> LocalCTs = new Dictionary<int, int>();
    [System.NonSerialized] public GameObject BreakEffect = null;
    [System.NonSerialized] public bool LimitFlag = false;
    [System.NonSerialized] public int MPUseTime = 1000;
    LineRenderer FixTLineObj;
    float STimer = 0;

    int BPCount = 0;

    float BHPPer = 1f;
    float BBreakPer = 0f;
    public float PLCStaMult()
    {
        if (Team == 0 || Team >= 100000) return 1f;
        else if (BTManager != null)
        {
            float Mult = 1f + (BTManager.PlayerList.Count - 1) * 0.8f;
            return Mult;
        }
        else return 1f;
    }
    public float FMHP
    {
        get
        {

            float FVal = MHP;
            FVal *= 1f + BufPowGet(Enum_Bufs.HP増加) * 0.01f;
            FVal *= PLCStaMult();
            if (BTManager != null)
            {
                switch (BTManager.Dife)
                {
                    case 0: FVal *= 0.5f; break;
                    case 2: FVal *= 1.5f; break;
                    case 3: FVal *= 1.8f; break;
                }
                if (BTManager.Chaos && BTManager.StageD!=null&& BTManager.StageD.ChaosStates!=null)
                {
                    var ChaosStas = BTManager.StageD.ChaosStates;
                    for (int i = 0; i < ChaosStas.Length; i++)
                    {
                        if (ChaosTargets(ChaosStas[i].Target) && ChaosStas[i].State == Enum_ChaosSta.最大HP) FVal *= 1f + (ChaosStas[i].Per * 0.01f);
                    }
                }
            }
            return FVal;
        }
    }
    public int FMMP
    {
        get
        {
            float FVal = MMP;
            return Mathf.RoundToInt(FVal);
        }
    }
    public int FAtk
    {
        get
        {
            float FVal = Atk;
            FVal *= 1f + (BufPowGet(Enum_Bufs.攻撃増加) * 0.01f) - (BufPowGet(Enum_Bufs.攻撃低下) * 0.01f);
            if (BTManager != null)
            {
                switch (BTManager.Dife)
                {
                    case 0: FVal *= 0.7f; break;
                    case 2: FVal *= 1.2f; break;
                    case 3: FVal *= 1.4f; break;
                }
                if (BTManager.Chaos && BTManager.StageD != null && BTManager.StageD.ChaosStates != null)
                {
                    var ChaosStas = BTManager.StageD.ChaosStates;
                    for (int i = 0; i < ChaosStas.Length; i++)
                    {
                        if (ChaosTargets(ChaosStas[i].Target) && ChaosStas[i].State == Enum_ChaosSta.攻撃力) FVal *= 1f + (ChaosStas[i].Per * 0.01f);
                    }
                }
            }
            return Mathf.RoundToInt(FVal);
        }
    }
    public int FDef
    {
        get
        {
            float FVal = Def;
            FVal *= 1f + (BufPowGet(Enum_Bufs.防御増加) * 0.01f) - (BufPowGet(Enum_Bufs.防御低下) * 0.01f);
            if (BTManager != null && BTManager.Chaos && BTManager.StageD != null && BTManager.StageD.ChaosStates != null)
            {
                var ChaosStas = BTManager.StageD.ChaosStates;
                for (int i = 0; i < ChaosStas.Length; i++)
                {
                    if (ChaosTargets(ChaosStas[i].Target) && ChaosStas[i].State == Enum_ChaosSta.防御力) FVal *= 1f + (ChaosStas[i].Per * 0.01f);
                }
            }
            
            return Mathf.RoundToInt(FVal);
        }
    }
    public float FMBreak
    {
        get
        {
            float FVal = MBreak;
            FVal *= PLCStaMult();
            return FVal;
        }
    }
    #endregion
    private void Awake()
    {
        HP = FMHP;
    }
    private void Start()
    {
        BTManager.StateList.Add(this);
        if (Player) BTManager.PlayerList.Add(this);
        if (Boss) BTManager.BossList.Add(this);
        if (CamTrans == null) CamTrans = transform;
        PlayerStateSet();
        HP = FMHP;
        MP = FMMP;
        if (!Player)
        {
            if(Team!=0 && Team<10000) ObjStrageParent(gameObject, "Enemy");
            else ObjStrageParent(gameObject, "Friend");
        }
        else ObjStrageParent(gameObject, "Players");

    }

    void Update()
    {
        //return;
        if (PhotonNetwork.OfflineMode) return;
        if (!photonView.IsMine) return;
        STimer -= Time.unscaledDeltaTime;
        if (STimer <= 0)
        {
            STimer = Mathf.Max(0.05f, StreamTime);
            photonView.RPC(nameof(RPC_Stream_Base), RpcTarget.Others, MHP,MMP,Atk,Def,MBreak,Name,Team,NoDamage,LimitFlag,ShortCut,RangeCut);
            photonView.RPC(nameof(RPC_Stream_Value), RpcTarget.Others, HP,BreakV,BreakT,Fixation != null ? Fixation.photonView.ViewID : -1);
            photonView.RPC(nameof(RPC_Stream_Anim), RpcTarget.Others, Anim_MoveID,Anim_AtkID,Anim_AtkSpeed,Anim_OtherID);
            var Buf_ID = new List<int>();
            var Buf_Index = new List<int>();
            var Buf_Time = new List<int>();
            var Buf_Pow = new List<int>();
            var Buf_TimeMax = new List<int>();
            var Buf_PowMax = new List<int>();
            for (int i = 0; i < Bufs.Count; i++)
            {
                Buf_ID.Add(Bufs[i].ID);
                Buf_Index.Add(Bufs[i].Index);
                Buf_Time.Add(Bufs[i].Time);
                Buf_Pow.Add(Bufs[i].Pow);
                Buf_TimeMax.Add(Bufs[i].TimeMax);
                Buf_PowMax.Add(Bufs[i].PowMax);
            }
            photonView.RPC(nameof(RPC_Stream_Buf), RpcTarget.Others,
                Buf_ID.ToArray(),Buf_Index.ToArray(),Buf_Time.ToArray(),Buf_Pow.ToArray(),Buf_TimeMax.ToArray(),Buf_PowMax.ToArray());
        }
    }
    private void FixedUpdate()
    {
        Anim_OtherID = 0;
        if (HP <= 0) Anim_OtherID = 2;
        else if (BreakT > 0) Anim_OtherID = 3;
        BufEffectSet();

        if (photonView.IsMine && Team != 0 && Team < 100000 && BPCount != BTManager.PlayerList.Count)
        {
            BPCount = BTManager.PlayerList.Count;
            HP = BHPPer * FMHP;
            BreakV = BBreakPer * FMBreak;
        }
        BHPPer = HP / FMHP;
        BBreakPer = BreakV / FMBreak;
        if (!photonView.IsMine) return;
        PlayerStateSet();
        Ground = GroundB;
        GroundB = false;
        MPUseTime++;
        if (MP <= 0)LowMP = true;
        if(Ground && MPUseTime >= 60)MP += MPRegene / 60f;
        MP = Mathf.Min(MP, FMMP);
        if (LowMP && MP >= FMMP * 0.2f) LowMP = false;
        if (HP <= 0)
        {
            StateDeaths();
        }
        if (HP > 0)
        {
            LivesTimes();
        }
        if (!Player && Team != 0 && BTManager.End) Deletes();

        DashTime--;
        BufTimeRems();
        LocalCTRems();
        Vector3 CRot = transform.forward;
        if (PLValues == null || PLValues.SubID > 0)
        {
            if (Target != null || TargetHit != null)
            {
                var TVect = (Target != null ? Target.PosGet() : TargetHit.PosGet()) - PosGet();
                CRot = Quaternion.LookRotation(TVect, Vector3.forward).eulerAngles;
            }
            else RotGet();
        }
        else CRot = CamTrans.eulerAngles;
        AtkPlays(CRot);
        if (Rig) Rig.useGravity = !NGravity;
        AddInfoChange();
    }
    #region 内部メソッド
    void PlayerStateSet()
    {
        if (!photonView.IsMine || !Player) return;
        var BaseSta = DB.Player;
        var Val = BaseSta.MHP + PVal_GeneGet(Enum_GeneOptions.最大HP);
        var Per = PVal_PassiveLVGet(Enum_Passive.HP増加) * 0.2f;
        if (PVal_GeneSetCo(Enum_GeneTypes.体力) >= 2) Per += 0.2f;
        Val *= 1f + Per;
        if (PVal_GeneSetCo(Enum_GeneTypes.混沌) >= 2) Val /= 2f;
        MHP = Mathf.RoundToInt(Val);
        Val = BaseSta.HPRegene + PVal_GeneGet(Enum_GeneOptions.HP回復速度);
        Val *= 1f + PVal_PassiveLVGet(Enum_Passive.自然再生) * 0.5f;
        HPRegene = Mathf.RoundToInt(Val);
        Val = BaseSta.MMP + PVal_GeneGet(Enum_GeneOptions.最大MP);
        Val *= 1f + PVal_PassiveLVGet(Enum_Passive.MP増加) * 0.2f;
        MMP = Mathf.RoundToInt(Val);
        Val = BaseSta.MPRegene + PVal_GeneGet(Enum_GeneOptions.MP回復速度);
        Val *= 1f + PVal_PassiveLVGet(Enum_Passive.気力増幅) * 0.1f;
        MPRegene = Mathf.RoundToInt(Val);
        Val = BaseSta.Atk + PVal_GeneGet(Enum_GeneOptions.攻撃力);
        Per = PVal_PassiveLVGet(Enum_Passive.攻撃力増加) * 0.1f;
        if (PVal_GeneSetCo(Enum_GeneTypes.攻撃) >= 2) Per += 0.1f;
        Val *= 1f + Per;
        Atk = Mathf.RoundToInt(Val);
        Val = BaseSta.Def + PVal_GeneGet(Enum_GeneOptions.防御力);
        Per = PVal_PassiveLVGet(Enum_Passive.防御力増加) * 0.25f;
        if (PVal_GeneSetCo(Enum_GeneTypes.防御) >= 2) Per += 0.25f;
        Val *= 1f + Per;
        Def = Mathf.RoundToInt(Val);
        ShortCut = PVal_PassiveLVGet(Enum_Passive.近接防衛) * 16f;
        RangeCut = PVal_PassiveLVGet(Enum_Passive.遠隔防衛) * 19f;

        if (PVal_GeneSetCo(Enum_GeneTypes.防御) >= 4 && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_防御4))
        {
            LocalCTs.Add((int)Enum_OtherCT.因子_防御4, 60 * 15);
            BufSets(Enum_Bufs.シールド, -1000, Enum_BufSet.付与, -1, Mathf.RoundToInt(FMHP*0.5f));
        }

        if (!PhotonNetwork.OfflineMode)
        {
            Name = "";
            if (PLValues.SubID > 0) Name = "<size=75%>";
            Name += photonView.Owner.NickName;
            if (PLValues.SubID > 0) Name += "</size>のサブ";
        }
        else
        {
            if (PLValues.SubID <= 0) Name = "プレイヤー";
            else Name = "サブ" + PLValues.SubID;
        }
    }
    void Deletes()
    {
        photonView.RPC(nameof(RPC_DeathEffect), RpcTarget.All);
        PhotonNetwork.Destroy(gameObject);
    }
    void AtkPlays(Vector3 CamRot)
    {
        if (!PhotonNetwork.InRoom) return;
        #region CT減少
        var CTKeys = AtkCTs.Keys.ToArray();
        for (int i = 0; i < CTKeys.Length; i++)
        {
            AtkCTs[CTKeys[i]].CT--;
            if((CTKeys[i] == 1 || CTKeys[i] == 2) && BufCheck(Enum_Bufs.タイムブレイク)) AtkCTs[CTKeys[i]].CT-=2;
            if (AtkCTs[CTKeys[i]].CT <= 0) AtkCTs.Remove(CTKeys[i]);
        }
        #endregion
        if (WepValues != null)
        {
            var WeponKeys = WepValues.WeponSets.Keys.ToArray();
            for (int i = 0; i < WeponKeys.Length; i++)
            {
                WepValues.WeponSets[WeponKeys[i]] = -1;
            }
        }
        SpeedRem = 0;
        NoJump = false;
        NoDash = false;
        Aiming = false;
        NGravity = false;
        NoDamage = false;
        #region スキル処理
        if (HP <= 0) AtkD = null;
        if (BreakT > 0) AtkD = null;
        Anim_AtkID = 0;
        Anim_AtkSpeed = 1;
        if (AtkD == null)
        {
            AtkTime = 0;
            AtkSlot = -1;
            return;
        }
        State_Atk.Fixed(this);
        State_Atk.Shot(this, PosGet(), RotGet(), CamRot);
        State_Atk.Move(this, CamRot);
        State_Atk.State(this);
        State_Atk.Buf(this);
        State_Atk.WeponSet(this);
        State_Atk.Anim(this);
        State_Atk.SEPlay(this);
        AtkTime++;
        var EndTime = AtkD.EndTime;
        var NoEnd = false;
        if (AtkD.BranchInfos.Count > 0)
        {
            var BranchGet = AtkD.BranchInfos.Find(x => x.BID == AtkBranch);
            if (BranchGet != null)
            {
                if (BranchGet.ChangeEndTime > 0) EndTime = BranchGet.ChangeEndTime;
                NoEnd = BranchGet.NoEnd;
            }
        }
        if (AtkTime >= EndTime)
        {
            if (!NoEnd) AtkD = null;
            else AtkTime = EndTime - 1;
        }
        #endregion
    }
    void StateDeaths()
    {
        var GutsLV = BufPowGet(Enum_Bufs.根性);
        if (GutsLV <= 0)
        {
            if (Player)
            {
                if (DeathTime == 0)
                {
                    PLValues.DeathCount++;
                    BTManager.SEPlay(DeathSE, PosGet());
                    BTManager.DeathAdd();
                    BTManager.MessageAdd("<color=#FF0000>" + Name + "</color>\\<color=#FF0000>は倒れた!!!</color>");
                    var DeathPowLV = PVal_PassiveLVGet(Enum_Passive.死に力);
                    if (DeathPowLV > 0)
                    {
                        BufSets(Enum_Bufs.攻撃増加, -1000, Enum_BufSet.付与増加, 0, DeathPowLV * 5);
                    }
                }
            }
            else if (!Boss && !Undet)
            {
                if (DeathTime >= 60)
                {
                    BTManager.SEPlay(DeathSE, PosGet());
                    Deletes();
                }
            }
            else
            {
                if (DeathTime == 0) BTManager.SEPlay(DeathSE, PosGet());
            }
        }
        else
        {
            BufPowRem(Enum_Bufs.根性, 1);
            HP = 1;
        }
        DeathTime++;
        DashTime = 0;
    }
    void LivesTimes()
    {
        var Regenes = HPRegene / 60f * PLCStaMult();
        if (PVal_GeneSetCo(Enum_GeneTypes.体力) >= 4 && (HP / FMHP) < 0.7f) Regenes *= 1.3f;
        if (PVal_GeneSetCo(Enum_GeneTypes.落下) >= 4 && !Ground && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_落下4))
        {
            BufSets(Enum_Bufs.落下強化, -1000, Enum_BufSet.付与増加, 60, 2, 60, 40);
            LocalCTs.Add((int)Enum_OtherCT.因子_落下4, 30);
        }
        if (BTManager != null)
        {
            if (BTManager.Chaos && BTManager.StageD != null && BTManager.StageD.ChaosStates != null)
            {
                var ChaosStas = BTManager.StageD.ChaosStates;
                for (int i = 0; i < ChaosStas.Length; i++)
                {
                    if (ChaosTargets(ChaosStas[i].Target) && ChaosStas[i].State == Enum_ChaosSta.HP回復速度)Regenes *= 1f + (ChaosStas[i].Per * 0.01f);
                }
            }
        }
        HP += Regenes;
        HP -= BufPowGet(Enum_Bufs.毒) / 60f;
        if (BufCheck(Enum_Bufs.過力)) HP -= FMHP * 0.1f / 60f;
        HP = Mathf.Clamp(HP, 1, FMHP);
        SPAdd(SPRegene / 60f);
        DeathTime = 0;
        if (Player)
        {
            var GutLV = PVal_PassiveLVGet(Enum_Passive.根性);
            if (GutLV > 0 && !BufCheck(Enum_Bufs.根性CT))
            {
                BufSets(Enum_Bufs.根性, -1000, Enum_BufSet.付与, 0, GutLV);
                BufSets(Enum_Bufs.根性CT, -1000, Enum_BufSet.付与, 60 * 15, 0);
            }
            if(PVal_GeneSetCo(Enum_GeneTypes.混沌)>=4 && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_混沌4))
            {
                LocalCTs.Add((int)Enum_OtherCT.因子_混沌4, 60 * 10);
                switch (Random.Range(0, 5))
                {
                    case 0:BufSets(Enum_Bufs.攻撃増加, -1999, Enum_BufSet.付与, 60 * 20, 15); break;
                    case 1: BufSets(Enum_Bufs.攻撃低下, -1999, Enum_BufSet.付与, 60 * 20, 25); break;
                    case 2: BufSets(Enum_Bufs.防御増加, -1999, Enum_BufSet.付与, 60 * 20, 15); break;
                    case 3: BufSets(Enum_Bufs.防御低下, -1999, Enum_BufSet.付与, 60 * 20, 25); break;
                    case 4: BufSets(Enum_Bufs.与ダメージ増加, -1999, Enum_BufSet.付与, 60 * 20, 15); break;
                    case 5: BufSets(Enum_Bufs.毒, -1999, Enum_BufSet.付与, 60 * 20, Mathf.RoundToInt(FMHP/100f)); break;
                    case 6: BufSets(Enum_Bufs.近距離強化, -1999, Enum_BufSet.付与, 60 * 20, 15); break;
                    case 7: BufSets(Enum_Bufs.遠距離強化, -1999, Enum_BufSet.付与, 60 * 20, 15); break;
                }
            }
        }
        if (FMBreak >= 0)
        {
            if (BreakT <= 0)
            {
                if (BreakV >= FMBreak)
                {
                    BreakT = BreakTime;
                    if (BreakHPRemPer != 0) photonView.RPC(nameof(RPC_DamFix), RpcTarget.All, Mathf.RoundToInt(FMHP * BreakHPRemPer * 0.01f));
                }
            }
            else
            {
                BreakT--;
                BreakV = 0;
            }
        }
        if (BreakT > 0)
        {
            if (BreakEffect == null)
            {
                BreakEffect = Instantiate(DB.BreakEffect, PosGet(), Quaternion.identity);
                BreakEffect.transform.localScale = Vector3.one * (1f + SizeAdd * 0.01f);
                BreakEffect.transform.parent = Rig != null ? Rig.transform : transform;
            }
        }
        else if (BreakEffect != null) Destroy(BreakEffect);
        #region カオス
        if (ChaosCheck(Enum_Stage.EXステージ) && Team > 0 && BTManager.Time % (60 * 10) == 0)
        {
            BufSets(Enum_Bufs.攻撃増加, -2999, Enum_BufSet.付与増加, -1, 5);
        }
        if (ChaosCheck(Enum_Stage.BOSブロッコリー) && Boss)
        {
            var ECount = 0;
            for(int i = 0; i < BTManager.StateList.Count; i++)
            {
                var Sta = BTManager.StateList[i];
                if (Sta!=null && Sta.gameObject.activeSelf && Sta.Team > 0) ECount++;
            }
            BufSets(Enum_Bufs.防御増加, -2999, Enum_BufSet.上書き, 60, 10 * ECount);
        }
        #endregion
        if (LimitFlag && !BufCheck(Enum_Bufs.時間制限))
        {
            Deletes();
        }
    }
    void BufTimeRems()
    {
        for (int i = Bufs.Count - 1; i >= 0; i--)
        {
            var Bufi = Bufs[i];
            if (Bufi.TimeMax <= 0) continue;
            Bufi.Time--;
            if (Bufi.Time <= 0) Bufs.RemoveAt(i);
        }
    }
    void LocalCTRems()
    {
        var LocalKeys = LocalCTs.Keys.ToArray();
        for (int i = 0; i < LocalKeys.Length; i++)
        {
            LocalCTs[LocalKeys[i]]--;
            if (LocalCTs[LocalKeys[i]] <= 0) LocalCTs.Remove(LocalKeys[i]);
        }
    }
    void BufEffectSet()
    {
        var BufKeys = BufEffects.Keys.ToArray();
        for (int i = 0; i < BufKeys.Length; i++)
        {
            var BufGet = Bufs.Find(x => x.ID == BufKeys[i]);
            if (BufGet == null)
            {
                Destroy(BufEffects[BufKeys[i]]);
                BufEffects.Remove(BufKeys[i]);
            }
        }
        for (int i = 0; i < Bufs.Count; i++)
        {
            var Bufi = Bufs[i];
            var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
            if (BufD != null && BufD.EffectObj!=null && !BufEffects.ContainsKey(Bufi.ID))
            {
                var EffectIns = Instantiate(BufD.EffectObj, PosGet(), Quaternion.identity);
                EffectIns.transform.localScale = Vector3.one * (1f + SizeAdd * 0.01f);
                EffectIns.transform.parent = Rig != null ? Rig.transform : transform;
                BufEffects.Add(Bufi.ID, EffectIns);
            }
        }

        if (BufCheck(Enum_Bufs.標的固定))
        {
            if(Fixation!=null)
            {
                if(Fixation.HP <= 0 || !Fixation.gameObject.activeInHierarchy)
                {
                    if (photonView.IsMine)
                    {
                        BufPowRem(Enum_Bufs.標的固定, 99999);
                        Fixation = null;
                    }
                }
                else
                {
                    if (FixTLineObj == null) FixTLineObj = Instantiate(DB.FixTargetLine, PosGet(), Quaternion.identity);
                    FixTLineObj.transform.parent = Rig != null ? Rig.transform : transform;
                    FixTLineObj.SetPositions(new Vector3[] { PosGet(), Fixation.PosGet() });
                    if (photonView.IsMine)
                    {
                        Target = Fixation;
                    }
                }

            }
        }
        else if (FixTLineObj != null) Destroy(FixTLineObj);
    }
    void BufPowRem(Enum_Bufs BufID, int Val,bool NClear = false)
    {
        for (int i = Bufs.Count - 1; i >= 0; i--)
        {
            var Bufi = Bufs[i];
            if (Bufi.ID == (int)BufID)
            {
                var Vald = Val;
                Val -= Bufi.Pow;
                Bufi.Pow -= Vald;
                if (Bufi.Pow <= 0)
                {
                    var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
                    if (BufD != null) BTManager.SEPlay(BufD.RemSE, PosGet());
                    if (!NClear) Bufs.Remove(Bufi);
                }
            }
            if (Val <= 0) return;
        }
    }
    void AddInfoChange()
    {
        if (PLValues == null) return;
        PLValues.AddTimer++;
        if(PLValues.AddTimer >= 60)
        {
            PLValues.AddTimer = 0;
            for (int i = PLValues.AddDams.Length - 1; i > 0; i--)
            {
                PLValues.AddDams[i] = PLValues.AddDams[i - 1];
                PLValues.AddHits[i] = PLValues.AddHits[i - 1];
            }
            PLValues.AddDams[0] = 0;
            PLValues.AddHits[0] = 0;
        }
    }
    #endregion
    #region 呼び出しメソッド
    public Vector3 PosGet()
    {
        if (Rig != null) return Rig.position + Vector3.up * Hight;
        else return transform.position + Vector3.up * Hight;
    }
    public Vector3 RotGet()
    {
        if (Rig != null) return Rig.transform.eulerAngles;
        else return transform.eulerAngles;
    }
    public void Damage(Vector3 HitPos, int Val,float Break,bool NDPerce,int AddViewID)
    {
        photonView.RPC(nameof(RPC_Damage), RpcTarget.All, HitPos, Val,Break, NDPerce, AddViewID);
    }
    public void TargetSet()
    {
        float NearDis = -1;
        Target = null;
        if (Fixation != null && BufCheck(Enum_Bufs.標的固定))
        {
            Target = Fixation;
            return;
        }
        foreach (var Sta in BTManager.StateList)
        {
            if (Sta == null) continue;
            if (!TeamCheck(this, Sta)) continue;
            if (Sta.HP <= 0) continue;
            if (!Sta.gameObject.activeInHierarchy) continue;
            float Dis = Vector3.Distance(PosGet(), Sta.PosGet());
            if (NearDis < 0 || NearDis > Dis)
            {
                NearDis = Dis;
                Target = Sta;
            }
        }
    }
    public void AtkInput(int UseAtkSlot, Data_Atk UseAtkD, bool Enter)
    {
        if (HP <= 0) return;
        if (BreakT > 0) return;
        if (AtkD == UseAtkD) return;

        if (!Enter) return;
        if (AtkCTs.ContainsKey(UseAtkSlot)) return;
        if (SP < UseAtkD.SPUse) return;
        if (Player)
        {
            switch (UseAtkSlot)
            {
                default: if (AtkSlot > 0) return; break;
                case 10: if (AtkSlot >= 10) return; break;
            }
        }
        else
        {
            if (AtkSlot != -1 && AtkD != null) return;
        }

        if (UseAtkD.SPUse > 0) SP = 0;
        int CTs = Mathf.RoundToInt(UseAtkD.CT * 60);
        if (Player)
        {
            var CTPer = PVal_PassiveLVGet(Enum_Passive.CTカット) * 0.10f;
            if (PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 2) CTPer -= 0.5f;
            if (PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 4) CTPer -= 0.5f;
            CTs = Mathf.RoundToInt(CTs * (1f - CTPer));
            if (UseAtkD.AtkType == Enum_AtkType.必殺)
            {
                PLValues.E_AtkCount++;
                var SpHealLV = PVal_PassiveLVGet(Enum_Passive.必殺再生);
                if (SpHealLV > 0) Damage(PosGet(), Mathf.RoundToInt(MHP * SpHealLV * -0.25f),0,true,photonView.ViewID);
                var SpReturnLV = PVal_PassiveLVGet(Enum_Passive.必殺返還);
                if (SpReturnLV > 0) SPAdd(SpReturnLV * 25);
                if(PVal_GeneSetCo(Enum_GeneTypes.必殺) >= 4)
                {
                    for(int i = 0; i < BTManager.StateList.Count; i++)
                    {
                        var FrSta = BTManager.StateList[i];
                        if (FrSta != null && FrSta.Team == Team) FrSta.BufSets(Enum_Bufs.与ダメージ増加, -2000, Enum_BufSet.付与, 60 * 6, 30);
                    }
                }
            }
        }
        if (UseAtkD.AtkType == Enum_AtkType.必殺)
        {
            var ColStr = Team == 0 ? "<color=#AAAA00>" : "<color=#AA00AA>";
            var MessageStr = ColStr + Name + "</color>\\";
            MessageStr += ColStr + "の必殺「" + UseAtkD.Name + "」</color>";
            BTManager.MessageAdd(MessageStr);
        }

        AtkCTs.Add(UseAtkSlot, new Class_Sta_AtkCT { CT = CTs, CTMax = CTs });

        AtkD = UseAtkD;
        AtkSlot = UseAtkSlot;
        if (PLValues == null) AtkBack = false;
        else AtkBack = PLValues.Backs;
        AtkTime = 0;
        AtkBranch = 0;
        MultHit.Clear();

    }
    public void AtkBranchSet(Data_Atk UseAtkD, bool Enter, bool Stay, int StayFl, bool Exit)
    {
        if (HP <= 0) return;
        if (BreakT > 0) return;
        if (UseAtkD != AtkD)
        {
            Enter = false;
            Stay = false;
            StayFl = 0;
            Exit = false;
        }
        State_Atk.Branch(this, Enter, Stay, StayFl, Exit);
    }
    public void BufSets(Class_Base_BufSet BufSet,State_Base AddSta)
    {
        var TimeVal = BufSet.TimeVal;
        var TimeMax = BufSet.TimeVal;
        var PowVal = Mathf.RoundToInt((float)Cal(BufSet.PowVal, AddSta, this));
        var PowMax = Mathf.RoundToInt((float)Cal(BufSet.PowMax, AddSta, this));
        //パッシブ
        if (BufSet.Buf == Enum_Bufs.毒 && PVal_GeneSetCo(Enum_GeneTypes.毒殺) >= 2) PowVal = Mathf.RoundToInt(PowVal * 1.3f);
        #region カオス
        if (BTManager != null && BTManager.Chaos && BTManager.StageD != null && BTManager.StageD.ChaosBfUps != null)
        {
            var ChaosBufs = BTManager.StageD.ChaosBfUps;
            for (int i = 0; i < ChaosBufs.Length; i++)
            {
                if (ChaosTargets(ChaosBufs[i].Target) && ChaosBufs[i].Buf == BufSet.Buf)
                {
                    TimeVal = Mathf.RoundToInt(TimeVal * (1f + (ChaosBufs[i].TimePer * 0.01f)));
                    TimeMax = Mathf.RoundToInt(TimeMax * (1f + (ChaosBufs[i].TimePer * 0.01f)));
                    PowVal = Mathf.RoundToInt(PowVal * (1f + (ChaosBufs[i].PowPer * 0.01f)));
                    PowMax = Mathf.RoundToInt(PowMax * (1f + (ChaosBufs[i].PowPer * 0.01f)));
                }
            }
        }
        
        #endregion
        if (AddSta != null && AddSta.PLValues!=null)
        {
            int ANum = AddSta.AtkSlot;
            if (AddSta.AtkBack) ANum += 100;

            var BufD = DB.Bufs.Find(x => x.Buf == BufSet.Buf);
            if (BufD != null && BufD.Type == Enum_BufType.バフ)
            {
                AddSta.PLValues.AddBuf++;
                switch (ANum)
                {
                    case 0: AddSta.PLValues.AtkBufs[0]++; break;
                    case 1: AddSta.PLValues.AtkBufs[1]++; break;
                    case 2: AddSta.PLValues.AtkBufs[2]++; break;
                    case 10: AddSta.PLValues.AtkBufs[3]++; break;
                    case 100: AddSta.PLValues.AtkBufs[4]++; break;
                    case 101: AddSta.PLValues.AtkBufs[5]++; break;
                    case 102: AddSta.PLValues.AtkBufs[6]++; break;
                    case 110: AddSta.PLValues.AtkBufs[7]++; break;
                }
            }
            if (BufD != null && BufD.Type == Enum_BufType.デバフ)
            {
                AddSta.PLValues.AddDBuf++;
                switch (ANum)
                {
                    case 0: AddSta.PLValues.AtkDBufs[0]++; break;
                    case 1: AddSta.PLValues.AtkDBufs[1]++; break;
                    case 2: AddSta.PLValues.AtkDBufs[2]++; break;
                    case 10: AddSta.PLValues.AtkDBufs[3]++; break;
                    case 100: AddSta.PLValues.AtkDBufs[4]++; break;
                    case 101: AddSta.PLValues.AtkDBufs[5]++; break;
                    case 102: AddSta.PLValues.AtkDBufs[6]++; break;
                    case 110: AddSta.PLValues.AtkDBufs[7]++; break;
                }
            }
        }
        if (BufSet.Buf == Enum_Bufs.標的固定) FixationSet(AddSta);
        BufSets(BufSet.Buf, BufSet.Index,BufSet.Set, TimeVal, PowVal, TimeMax, PowMax);
    }
    public void BufSets(Enum_Bufs BufID, int Index, Enum_BufSet Sets, int Time, int Pow, int TMax=0, int PMax=0)
    {
        photonView.RPC(nameof(RPC_BufSet), RpcTarget.All, (int)BufID, Index, (int)Sets, Time, Pow, TMax, PMax);
    }
    public void FixationSet(State_Base UseSta)
    {
        photonView.RPC(nameof(RPC_FixationSet), RpcTarget.All, UseSta.photonView.ViewID);
    }
    public int BufPowGet(Enum_Bufs BufID)
    {
        int Pow = 0;
        for(int i = 0; i < Bufs.Count; i++)
        {
            if (Bufs[i].ID == (int)BufID) Pow += Bufs[i].Pow;
        }
        return Pow;
    }
    public bool BufCheck(Enum_Bufs BufID)
    {
        for (int i = 0; i < Bufs.Count; i++)
        {
            if (Bufs[i].ID == (int)BufID) return true;
        }
        return false;
    }
    public float BufTPMultGet(Enum_Bufs BufID)
    {
        float Val = 0;
        for (int i = 0; i < Bufs.Count; i++)
        {
            if (Bufs[i].ID == (int)BufID) Val += (float)Bufs[i].Time * Bufs[i].Pow;
        }
        return Val;
    }
    public void HitEvents(State_Base HitSta,Vector3 Pos, Enum_DamageType DamType, bool ShortAtk)
    {
        if (BufCheck(Enum_Bufs.原刺の刃) && SP >= 50 && !LocalCTs.ContainsKey((int)Enum_OtherCT.原刺の刃))
        {
            SP -= 50;
            LocalCTs.Add((int)Enum_OtherCT.原刺の刃, 60 * 2);
            State_Atk.ShotAdd(this, 0, DB.AddAtks[(int)Enum_AddAtk.原刺の刃], 0, Pos, Vector3.zero,null,-1);
        }
        if (Player)
        {
            var TaruLV = PVal_PassiveLVGet(Enum_Passive.タルタル);
            int TaruCTID = 400000 + ((int)Enum_OtherCT.タルタル * 10) + PLValues.SubID;
            if (TaruLV > 0 && !HitSta.LocalCTs.ContainsKey(TaruCTID))
            {
                HitSta.LocalCTs.Add(TaruCTID, 60 * 3);
                State_Atk.ShotAdd(this, TaruLV, DB.AddAtks[(int)Enum_AddAtk.タルタル], 0, Pos, Vector3.zero, null, -1);
            }
            var AddShashLV = PVal_PassiveLVGet(Enum_Passive.追斬);
            if (AddShashLV > 0 && !LocalCTs.ContainsKey((int)Enum_OtherCT.追斬))
            {
                LocalCTs.Add((int)Enum_OtherCT.追斬, 60 * 4);
                State_Atk.ShotAdd(this, AddShashLV, DB.AddAtks[(int)Enum_AddAtk.追斬], 0, Pos, Vector3.zero, null, -1);
            }
            var WSystemLV = PVal_PassiveLVGet(Enum_Passive.Wシステム);
            if (WSystemLV > 0 && !LocalCTs.ContainsKey((int)Enum_OtherCT.Wシステム) && DamType != Enum_DamageType.パッシブ)
            {
                LocalCTs.Add((int)Enum_OtherCT.Wシステム, 60 * 1);
                int Timed = 60 * 6;
                if (ShortAtk) BufSets(Enum_Bufs.遠距離強化, -1000, Enum_BufSet.付与増加, Timed, 2 * WSystemLV, Timed, 40);
                else BufSets(Enum_Bufs.近距離強化, -1000, Enum_BufSet.付与増加, Timed, 3 * WSystemLV, Timed, 60);
            }
            if(PVal_GeneSetCo(Enum_GeneTypes.攻撃) >= 4 && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_攻撃4))
            {
                LocalCTs.Add((int)Enum_OtherCT.因子_攻撃4, 60 * 20);
                BufSets(Enum_Bufs.攻撃増加, -1000, Enum_BufSet.付与, 60 * 10, 20);
            }
            if (PVal_GeneSetCo(Enum_GeneTypes.速度) >= 4 && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_速度4))
            {
                bool TypeCheck = false;
                if (DamType == Enum_DamageType.通常) TypeCheck = true;
                if (DamType == Enum_DamageType.重撃) TypeCheck = true;
                if (DamType == Enum_DamageType.落下) TypeCheck = true;
                if (TypeCheck)
                {
                    LocalCTs.Add((int)Enum_OtherCT.因子_速度4, 60 * 2);
                    var AtkCTKey = AtkCTs.Keys.ToArray();
                    for (int i = 0; i < AtkCTKey.Length; i++) AtkCTs[AtkCTKey[i]].CT -= 30;
                    Debug.Log("速度4");
                }
            }
            int Poison4CTID = 400000 + ((int)Enum_OtherCT.因子_毒殺4 * 10) + PLValues.SubID;
            if (PVal_GeneSetCo(Enum_GeneTypes.毒殺) >= 4 && !HitSta.LocalCTs.ContainsKey(Poison4CTID))
            {
                HitSta.LocalCTs.Add(Poison4CTID, 60*3);
                HitSta.Damage(HitSta.PosGet(), HitSta.BufPowGet(Enum_Bufs.毒), 0,false, photonView.ViewID);
            }
            if (PVal_GeneSetCo(Enum_GeneTypes.通常) >= 4 && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_通常4) && DamType == Enum_DamageType.通常)
            {
                LocalCTs.Add((int)Enum_OtherCT.因子_通常4, 6);
                BufSets(Enum_Bufs.メイン強化, -1000, Enum_BufSet.付与増加, 60 * 6, 1, 60 * 6, 25);
            }
            if (PVal_GeneSetCo(Enum_GeneTypes.スキル) >= 4 && !LocalCTs.ContainsKey((int)Enum_OtherCT.因子_スキル4) && DamType == Enum_DamageType.スキル)
            {
                LocalCTs.Add((int)Enum_OtherCT.因子_スキル4, 60*30);
                BufSets(Enum_Bufs.スキル強化, -1000, Enum_BufSet.付与, 60 * 10, 20);
                var AtkCTKey = AtkCTs.Keys.ToArray();
                for (int i = 0; i < AtkCTKey.Length; i++)
                {
                    if(AtkCTKey[i] == 1 || AtkCTKey[i] == 2)AtkCTs[AtkCTKey[i]].CT -= 60 * 30;
                }
            }
        }
    }
    public void AddInfoAdd(int Dam,int Type)
    {
        if (PLValues == null) return;
        if (Dam > 0)
        {
            PLValues.AddDamTotal += Dam;
            PLValues.AddDams[0] += Dam;
            PLValues.AddHitTotal++;
            PLValues.AddHits[0]++;

            switch (Type)
            {
                case 0:
                    PLValues.AtkDams[0] += Dam;
                    PLValues.AtkHits[0]++;
                    break;
                case 1:
                    PLValues.AtkDams[1] += Dam;
                    PLValues.AtkHits[1]++;
                    break;
                case 2:
                    PLValues.AtkDams[2] += Dam;
                    PLValues.AtkHits[2]++;
                    break;
                case 10:
                    PLValues.AtkDams[3] += Dam;
                    PLValues.AtkHits[3]++;
                    break;
                case 100:
                    PLValues.AtkDams[4] += Dam;
                    PLValues.AtkHits[4]++;
                    break;
                case 101:
                    PLValues.AtkDams[5] += Dam;
                    PLValues.AtkHits[5]++;
                    break;
                case 102:
                    PLValues.AtkDams[6] += Dam;
                    PLValues.AtkHits[6]++;
                    break;
                case 110:
                    PLValues.AtkDams[7] += Dam;
                    PLValues.AtkHits[7]++;
                    break;
            }
        }
        else
        {
            Dam *= -1;
            PLValues.AddHeal += Dam;
            switch (Type)
            {
                case 0:PLValues.AtkHeals[0] += Dam;break;
                case 1:PLValues.AtkHeals[1] += Dam;break;
                case 2:PLValues.AtkHeals[2] += Dam;break;
                case 10:PLValues.AtkHeals[3] += Dam;break;
                case 100:PLValues.AtkHeals[4] += Dam;break;
                case 101:PLValues.AtkHeals[5] += Dam;break;
                case 102:PLValues.AtkHeals[6] += Dam; break;
                case 110:PLValues.AtkHeals[7] += Dam;break;
            }
        }
    }
    public void AddInfoReset()
    {
        if (PLValues == null) return;
        PLValues.AddTimer = 0;
        for(int i = 0; i < PLValues.AddDams.Length; i++)
        {
            PLValues.AddDams[i] = 0;
            PLValues.AddHits[i] = 0;
        }
        PLValues.AddDamTotal = 0;
        PLValues.AddHitTotal = 0;
    }
    public void KBSet(Vector3 Vect,bool NoMass,bool SetVect)
    {
        photonView.RPC(nameof(RPC_KBSet), RpcTarget.All, Vect, NoMass, SetVect);
    }
    public void SPAdd(float Val)
    {
        var BackSPPer = 0.3f;
        var Per = PVal_PassiveLVGet(Enum_Passive.SPブースト) * 0.25f;
        Per += PVal_GeneGet(Enum_GeneOptions.SP回復量) * 0.01f;
        if (PVal_GeneSetCo(Enum_GeneTypes.必殺) >= 2) Per += 0.2f;
        Val *= 1f + Per;
        BackSPPer += PVal_PassiveLVGet(Enum_Passive.裏蓄積) * 0.15f;
        SP += Val;
        SPB += Val * BackSPPer;
    }
    public bool ChaosTargets(Enum_ChaosTarget ChSta)
    {
        switch (ChSta)
        {
            case Enum_ChaosTarget.全敵: return Team > 0 && Team < 100000;
            case Enum_ChaosTarget.敵ザコ: return Team > 0 && Team < 100000 && !Boss;
            case Enum_ChaosTarget.敵ボス: return Team > 0 && Team < 100000 && Boss;
            case Enum_ChaosTarget.全味方: return Team <= 0 || Team >= 100000;
            case Enum_ChaosTarget.プレイヤー: return (Team <= 0 || Team >= 100000) && PLValues != null;
            case Enum_ChaosTarget.召喚味方: return (Team <= 0 || Team >= 100000) && PLValues == null;
        }
        return false;
    }

    public Class_Save_Atks PVal_AtkGet(bool Back)
    {
        if (PLValues == null) return null;
        return !Back ? PLValues.Sets.AtkF : PLValues.Sets.AtkB;
    }
    public int PVal_PassiveLVGet(Enum_Passive Pass)
    {
        if (PLValues == null) return 0;
        return PLValues.Sets.PassiveLVGet(Pass);
    }
    public float PVal_GeneGet(Enum_GeneOptions GOP)
    {
        if (PLValues == null) return 0;
        return GenePowT(PLValues.Genes, GOP);
    }
    public int PVal_GeneSetCo(Enum_GeneTypes Type)
    {
        if (PLValues == null) return 0;
        return GeneSetCount(PLValues.Genes, Type);
    }
    #endregion
    #region RPCメソッド
    [PunRPC]
    void RPC_Damage(Vector3 HitPos, int Val,float Break,bool NDPerce,int AddViewID,PhotonMessageInfo PhInfo)
    {
        if (Val > 0 && !NDPerce && NoDamage) return;
        if (HP <= 0) return;
        BreakV += Break;
        if (Val < 0)
        {
            var Healm = FMHP - HP;
            Val = -(int)Mathf.Min(-Val, Healm);
        }
        else if (Val > 0)
        {
            int Barria = BufPowGet(Enum_Bufs.バリア);
            if (Barria > 0)
            {
                Val /= 10;
                if (photonView.IsMine)
                {
                    BufPowRem(Enum_Bufs.バリア, 1);
                    BTManager.SEPlay(DB.BarriaHitSE, HitPos, true);
                }
            }
            if (BufCheck(Enum_Bufs.反撃蓄積))
            {
                if (photonView.IsMine)
                {
                    BufPowRem(Enum_Bufs.反撃蓄積, Val,true);
                }
            }
            int Shilds = BufPowGet(Enum_Bufs.シールド);
            if (Shilds > 0)
            {
                int Vald = Val;
                int ShValue = Mathf.Min(Val, Shilds);
                Val = Mathf.Max(0, Val - Shilds);
                if (photonView.IsMine)
                {
                    Color ShildCol = Color.blue;
                    switch (Team)
                    {
                        case 0:
                            ShildCol = Color.cyan;
                            break;
                    }
                    DamageObj.DamageSet(HitPos, Mathf.Abs(ShValue), ShildCol,photonView.ViewID,AddViewID);
                    BufPowRem(Enum_Bufs.シールド, Vald);
                    BTManager.SEPlay(DB.ShildHitSE, HitPos, true);
                }
            }
        }
        if (Val == 0) return;
        Color DamCol = Val >= 0 ? Color.white : Color.magenta;
        GameObject HitEffect = Val >= 0 ? DB.HitEffects[1] : DB.HealEffects[1];
        switch (Team)
        {
            case 0:
                DamCol = Val >= 0 ? Color.red : Color.green;
                HitEffect = Val >= 0 ? DB.HitEffects[0] : DB.HealEffects[0];
                break;
        }
        DamageObj.DamageSet(HitPos, Mathf.Abs(Val), DamCol,photonView.ViewID, AddViewID);
        var InsHitEffect = Instantiate(HitEffect, HitPos, Quaternion.identity);
        ObjStrageParent(InsHitEffect, "Effects");
        if (Val >= 0)BTManager.SEPlay(DamageSE, HitPos,true);
        if (!photonView.IsMine && PhInfo.Sender != PhotonNetwork.LocalPlayer) return;
        if (Player)
        {
            if (Val > 0)PLValues.ReceiveDam += Val;
            var LifeVibrationLV = PVal_PassiveLVGet(Enum_Passive.生命の振動);
            if (LifeVibrationLV > 0 && !LocalCTs.ContainsKey((int)Enum_OtherCT.生命の振動))
            {
                LocalCTs.Add((int)Enum_OtherCT.生命の振動, 60 * 1);
                int Timed = 60 * 6;
                BufSets(Enum_Bufs.与ダメージ増加, -1000, Enum_BufSet.付与増加, Timed, 3 * LifeVibrationLV, Timed, 36);
            }
        }
        HP -= Val;
    }
    [PunRPC]
    void RPC_DamFix(int Val)
    {
        DamageObj.DamageSet(PosGet(), Mathf.Abs(Val),new Color(2,2,2), photonView.ViewID, photonView.ViewID);
        if (photonView.IsMine)HP -= Val;
    }
    [PunRPC]
    void RPC_DeathEffect()
    {
        if (DeathEffect != null)
        {
            var InsDeathEffect = Instantiate(DeathEffect, PosGet(), Quaternion.identity);
            ObjStrageParent(InsDeathEffect, "Effects");
        }
    }
    [PunRPC]
    void RPC_BufSet(int BufID, int Index, int Sets, int Time, int Pow, int TMax, int PMax, PhotonMessageInfo PhInfo)
    {
        if (!photonView.IsMine && PhInfo.Sender != PhotonNetwork.LocalPlayer) return;
        Class_Sta_BufInfo Bufi = null;
        for (int i = 0; i < Bufs.Count; i++)
        {
            var Bufd = Bufs[i];
            if(Bufd.ID == BufID && Bufd.Index == Index)
            {
                Bufi = Bufd;
                break;
            }
        }
        if (Sets != (int)Enum_BufSet.消去)
        {
            if (Bufi != null && Sets == (int)Enum_BufSet.切り替え)
            {
                var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
                if (BufD != null) BTManager.SEPlay(BufD.RemSE, PosGet());
                Bufs.Remove(Bufi);
            }
            else
            {
                if (Bufi == null && Sets == (int)Enum_BufSet.不付与増加) return;
                if (Bufi == null)
                {
                    Bufi = new Class_Sta_BufInfo { ID = BufID, Index = Index, Time = 0, Pow = 0, TimeMax = 1,PowMax=1 };
                    Bufs.Add(Bufi);
                }
                if (Sets == (int)Enum_BufSet.上書き)
                {
                    Bufi.Time = Time;
                    Bufi.Pow = Pow;
                    Bufi.TimeMax = Time;
                    Bufi.PowMax = Pow;
                }
                else
                {
                    if (Sets == (int)Enum_BufSet.付与 || Sets == (int)Enum_BufSet.切り替え)
                    {
                        Bufi.Time = Mathf.Max(Bufi.Time, Time);
                        Bufi.Pow = Mathf.Max(Bufi.Pow, Pow);
                    }
                    else
                    {
                        if (TMax <= 0) Bufi.Time += Time;
                        else Bufi.Time = Mathf.Max(Mathf.Min(Bufi.Time + Time, TMax), Bufi.Time);
                        if (PMax <= 0) Bufi.Pow += Pow;
                        else Bufi.Pow = Mathf.Max(Mathf.Min(Bufi.Pow + Pow, PMax), Bufi.Pow);
                    }
                    if (Bufi.Time <= 0) Bufi.TimeMax = 0;
                    if (Bufi.TimeMax > 0) Bufi.TimeMax = Mathf.Max(Bufi.Time, Bufi.TimeMax);
                    if (Bufi.Pow <= 0) Bufi.PowMax = 0;
                    if (Bufi.PowMax > 0) Bufi.PowMax = Mathf.Max(Bufi.Pow, Bufi.PowMax);
                }

            }
        }
        else if (Bufi != null)
        {
            var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
            if (BufD != null) BTManager.SEPlay(BufD.RemSE, PosGet());
            Bufs.Remove(Bufi);
        }
    }
    [PunRPC]
    void RPC_FixationSet(int PViewID,PhotonMessageInfo PhInfo)
    {
        if (!photonView.IsMine && PhInfo.Sender != PhotonNetwork.LocalPlayer) return;
        var PObj = PhotonNetwork.GetPhotonView(PViewID);
        if (PObj == null) return;
        var PSta = PObj.GetComponent<State_Base>();
        if (PSta == null) return;
        Target = PSta;
        Fixation = PSta;
    }
    [PunRPC]
    void RPC_KBSet(Vector3 Vect, bool NoMass, bool SetVect, PhotonMessageInfo PhInfo)
    {
        if (!photonView.IsMine && PhInfo.Sender != PhotonNetwork.LocalPlayer) return;
        if (Rig == null) return;
        var RigVect = !SetVect ? Rig.linearVelocity : Vector3.zero;
        RigVect += Vect / (!NoMass ? Rig.mass : 1f);
        Rig.linearVelocity = RigVect;
    }
    [PunRPC]
    void RPC_Stream_Base(int Str_MHP,int Str_MMP, int Str_Atk, int Str_Def, int Str_MBreak, string Str_Name, int Str_Team, bool Str_NoDamage, bool Str_LimitFlag,float Str_ShortCut,float Str_RangeCut)
    {
        MHP = Str_MHP;
        MMP = Str_MMP;
        Atk = Str_Atk;
        Def = Str_Def;
        MBreak = Str_MBreak;
        Name = Str_Name;
        Team = Str_Team;
        NoDamage = Str_NoDamage;
        LimitFlag = Str_LimitFlag;
        ShortCut = Str_ShortCut;
        RangeCut = Str_RangeCut;
    }
    [PunRPC]
    void RPC_Stream_Value(float Str_HP, float Str_BreakV, int Str_BreakT,int FixTID)
    {
        HP = Str_HP;
        BreakV = Str_BreakV;
        BreakT = Str_BreakT;
        var FixTObj = PhotonNetwork.GetPhotonView(FixTID);
        if (FixTObj != null)
        {
            var FixTSta = FixTObj.GetComponent<State_Base>();
            Fixation = FixTSta;
        }
        else Fixation = null;
    }
    [PunRPC]
    void RPC_Stream_Anim(int Str_MoveID, int Str_AtkID, float Str_AtkSpeed, int Str_OtherID)
    {
        Anim_MoveID = Str_MoveID;
        Anim_AtkID = Str_AtkID;
        Anim_AtkSpeed = Str_AtkSpeed;
        Anim_OtherID = Str_OtherID;
    }
    [PunRPC]
    void RPC_Stream_Buf(int[] Str_ID, int[] Str_Index, int[] Str_Time, int[] Str_Pow, int[] Str_TimeMax, int[] Str_PowMax)
    {
        Bufs.Clear();
        for (int i = 0; i < Str_ID.Length; i++)
        {
            Bufs.Add(new Class_Sta_BufInfo
            {
                ID = Str_ID[i],
                Index = Str_Index[i],
                Time = Str_Time[i],
                Pow = Str_Pow[i],
                TimeMax = Str_TimeMax[i],
                PowMax = Str_PowMax[i],
            });
        }
    }
    #endregion

}
