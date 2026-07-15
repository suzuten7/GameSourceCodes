using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/* 内容
 * ・プレイヤーの基礎ステータス
 */

#region 変数倉庫1
/// <summary>
/// プレイヤーのステータス
/// </summary>
[System.Serializable]
public class PlayerStates
{
    [Header("- プレイヤー情報 -")]
    [Tooltip("名前")] public string name;
    [Tooltip("チーム識別ID")] public int teamID;
    [Tooltip("CPUモードID")] public int cpuMode;
    [Tooltip("パッシブ")] public List<Vector2Int> passives;
    [Tooltip("キャラ外見")] public int charaImgID;
    [Tooltip("カスタム外見")] public int loadImgID;
    [Header("- 基礎 -")]
    [Tooltip("体力")] public float max_HP = 100;
    public float injury_Multi = 0.20f;
    [Tooltip("移動速度(ベース)")] public float base_SPD = 1000;
    [Tooltip("移動ステータス")] public List<MoveStateData> set_MoveState;
    [Tooltip("窒息時間")] public float suffocationTime;
    [Header("- 現在の装備状況 -")]
    [Tooltip("銃ID")] public int gun_IndexNum = 0;
    [Tooltip("近接ID")] public int melee_IndexNum = 0;
    [Tooltip("ガシェットID")] public int gadget_IndexNum = 0;
    [Tooltip("パッシブID")] public int passive_IndexNum = 0;
    [Tooltip("必殺ID")] public int ult_IndexNum = 0;

    [Header("- 自動回復 -")]
    [Tooltip("体力回復速度(HP/s)")] public float regeneHP = 20;
    [Tooltip("遅延体力回復(s)")] public float weit_RegeneDelay = 1;

    [Header("- 視界 -")]
    [Tooltip("視界")] public float vision_Angle = 90;
    [Tooltip("カメラサイズ(倍率)")] public float cameraSize_Multi = 1;
    [Header("- その他 -")]
    [Tooltip("カカシ状態になる")] public bool cantMove;
}

/// <summary>
///  プレイヤーのオブジェクト
/// </summary>
[System.Serializable]
public class PlayerObjects
{
    public Rigidbody2D rb;
    public GameObject ownerOnlys;
    public Transform scaleTrans;
    public SpriteRenderer auraSr;
    public GameObject charaImgObj;
    public Transform handTrans;
    public Camera mapCam;
    public Player_ValueSync valueSync;
    public Net_RigSync rigSync;
    [Header("- 視界 -")]
    [Tooltip("周辺")] public Light2D circle_Light;
    public SpotMask circle_Mask;
    [Tooltip("視界")] public Light2D vision_Light;
    public SpotMask vision_Mask;
    [Tooltip("全可視")] public GameObject viewAll;
    [Header("- レティクル関係 -")]
    [Tooltip("カメラ")] public Camera camera;
    [Tooltip("レティクル")] public Transform targetPoint;
    [Tooltip("アシストレティクル")] public Transform assistPoint;
    [Tooltip("リコイル")] public Transform recoilPoint;
    [Tooltip("最終")] public Transform finalPoint;
    [Tooltip("レティクル距離")] public TextMeshProUGUI targetDisTx;
    [Tooltip("拡散範囲")] public SpotMask dif_View;
    [Tooltip("拡散表示")] public MeshRenderer dif_Mr;
    [Header("- ガジェット -")]
    [Tooltip("最大値")] public SpriteRenderer throwPowMax;
    [Tooltip("現在")] public SpriteRenderer throwPowNow;
    [Tooltip("最低値")] public SpriteRenderer throwPowMin;
    [Header("- UI -")]
    public GameObject HPUI;
    public GameObject nameUI;
    public RectTransform damageCanvas;
}
/// <summary>
///  プレイヤーの変数
/// </summary>
[System.Serializable]
public class PlayerValues
{
    [Header("-  体力 -")]
    [Tooltip("現在HP")] public float hpNow;
    [Tooltip("追加HP")] public float hpOver;
    [Tooltip("シールド")] public float hpShild;
    [Tooltip("負傷")] public float hpInjury;
    [Tooltip("回復遅延")] public float hpRegeneDelay;
    [Header("-  移動 -")]
    [Tooltip("現在の移動ステータス")] public MoveState now_MoveState;
    [Tooltip("移動タイプ")] public Data_MoveType moveType;
    [Tooltip("窒息時間")] public float suffocationTime;
    [Tooltip("窒息ダメCT")] public float suffocationDamCT;
    [Header("-  バフ -")]
    [Tooltip("バフ")] public List<BufSet> bufs;
    [Header("-  銃 -")]
    [Tooltip("残弾数")] public int gun_bullet;
    [Tooltip("リロード時間")] public float set_ReloadTime;
    [Tooltip("現在のリロード時間")] public float now_ReloadTime;

    [Tooltip("残弾数")] public int melee_bullet;
    [Header("-  ガシェット -")]
    [Tooltip("現在の保持数")] public int now_Retention;
    [Tooltip("現在の取得までの時間")] public float now_GetTime;
    [Tooltip("現在の使うまでの時間")] public float now_UseTime;
    [Tooltip("現在の投げる強さ")] public float now_throwPower;
    [Tooltip("現在の投げれる最大の距離")] public float max_throwRange;
    [Header("-  必殺 -")]
    [Tooltip("必殺チャージ")] public float ultCharge;
    [Header("-  カーソル -")]
    [Tooltip("現在のカーソルのモード")]public CursorState now_CursorState = CursorState.Shot;
    [Header("-  その他 -")]

    [Tooltip("最終攻撃時間")] public float lastAtkTime;
    [Tooltip("最終ガシェット時間")] public float lastGGTime;
    [Tooltip("最終必殺時間")] public float lastUltTime;
    [Tooltip("アニメーションID")] public int atkAnimID;

    [Tooltip("キル数")] public int kill;
    [Tooltip("連続キル数")] public int killcons;
    [Tooltip("連続キル発光")] public bool killLight;
    [Tooltip("連続キル最大")] public int killcmax;
    [Tooltip("スコア")] public float score;

    [Tooltip("合計与ダメ")] public float addDamages;
    [Tooltip("合計被ダメ")] public float takeDamages;
    [Tooltip("合計与回復")] public float addHeals;

    [Tooltip("前チーム")] public int team_back;
    [Tooltip("前キル数")] public int kill_back;
    [Tooltip("前連続キル最大")] public int killcmax_back;
    [Tooltip("前スコア")] public float score_back;

    [Tooltip("前合計与ダメ")] public float addDamages_back;
    [Tooltip("前合計被ダメ")] public float takeDamages_back;
    [Tooltip("前合計与回復")] public float addHeals_back;

    [Tooltip("死亡時間")] public float deathTime = 0;
    [Tooltip("最終攻撃ID")] public int lastAtkID;
    [Tooltip("最終攻撃者")] public Player_Manager lastAtkPm;
    [Tooltip("最終攻撃キル者")] public Player_Manager lastAtkKillPm;
    [Tooltip("キルスコア倍率")] public float killScoreMult;
    [Tooltip("キルHS")] public bool killHeadShot;

    [Tooltip("無敵時間")] public float noDamTime;
    [Tooltip("非表示")] public bool noView;
}
/// <summary>
/// パッシブ補正
/// </summary>
[System.Serializable]
public class PlayerPassiveChanges
{
    [Tooltip("自動回復遅延倍率(倍率)")] public float regeneDelay_Multi = 1;
    [Tooltip("負傷倍率(倍率)")] public float injust_Multi = 1;

    [Tooltip("音感知(倍率)")] public float soundGet_Multi = 1;
    [Tooltip("移動速度(倍率)")] public float moveSpeed_Multi = 1;
    [Tooltip("移動音量(倍率)")] public float moveSound_Multi = 1;
    [Tooltip("低速エリア軽減")] public float slowArea_Regist = 1;
    [Tooltip("ダメージエリア軽減")] public float damageArea_Regist = 1;
    [Tooltip("KB耐性")] public float kb_Regist;
    [Tooltip("回転速度(倍率)")] public float spinSpeed_Multi = 1;

    [Tooltip("与ダメージ(倍率)")] public float damageAllAdd_Multi = 1;
    [Tooltip("銃与ダメージ(倍率)")] public float damageGunAdd_Multi = 1;
    [Tooltip("近接与ダメージ(倍率)")] public float damageMeleeAdd_Multi = 1;
    [Tooltip("ヘットショット与ダメージ(倍率)")] public float damageHeadAdd_Multi = 1;

    [Tooltip("銃近接切り替え時間(倍率)")] public float changeWaitTime_Mult = 1;
    [Tooltip("弾RPS(倍率)")] public float bulletRPS_Multi = 1;
    [Tooltip("弾速度(倍率)")] public float bulletSpeed_Multi = 1;
    [Tooltip("リロード速度(倍率)")] public float reload_Multi = 1;
    [Tooltip("攻撃音量(倍率)")] public float atkSound_Multi = 1;
    [Tooltip("拡散率(倍率)")] public float diffusion_Multi = 1;
    [Tooltip("反動(倍率)")] public float recoil_Multi = 1;
    [Tooltip("マガジン容量(倍率)")] public float magazine_Multi = 1;

    [Tooltip("ガシェット使用待機時間(倍率)")] public float ggUseTime_Mult = 1;
    [Tooltip("ガシェットCT(倍率)")] public float ggCoolTime_Multi = 1;
    [Tooltip("ガシェットストック増加")] public int ggStockAdd = 0;
    [Tooltip("ガシェット射程(倍率)")] public float ggRange_Multi = 1;
    [Tooltip("ガシェット与ダメージ(倍率)")] public float damageGGAdd_Multi = 1;

    [Tooltip("全スコア(倍率)")] public float scoreAll_Multi = 1;
    [Tooltip("ダメージスコア(倍率)")] public float scoreDamage_Multi = 1;
    [Tooltip("キルスコア(倍率)")] public float scoreKill_Multi = 1;
    [Tooltip("ヘットキルスコア(倍率)")] public float scoreHeadKill_Multi = 1;

    [Tooltip("必殺チャージ増加(倍率)")] public float ultCharge_Multi = 1;
    [Tooltip("必殺チャージ時間増加(倍率)")] public float ultChargeTime_Multi = 1;
    [Tooltip("必殺チャージ与ダメ増加(倍率)")] public float ultChargeAddDam_Multi = 1;
    [Tooltip("必殺チャージ受ダメ増加(倍率)")] public float ultChargeHitDam_Multi = 1;
    [Tooltip("必殺チャージ回避増加(倍率)")] public float ultChargeDoge_Multi = 1;
    [Tooltip("必殺チャージキル増加(倍率)")] public float ultChargeKill_Multi = 1;

    [Tooltip("リスポーン速度(倍率)")] public float respawneSpeed_Multi = 1;

    [Tooltip("プレイヤーサイズ(倍率)")] public float charaScale_Multi = 1;
    [Tooltip("回避判定サイズ(倍率)")] public float dogeScale_Multi = 1;
    [Tooltip("ランタン(敵に壁越し視認される+周囲視野増加)")] public bool lantan;
    [Tooltip("発信機(敵にマップ位置ばれる距離)")] public float transmitter;

}
/// <summary>
/// 現在の移動データ
/// </summary>
[System.Serializable]
public class MoveStateData
{
    [Tooltip("移動のステータス")] public MoveState moveState;
    [Tooltip("移動速度倍率")] public float spd_Multi;
    [Tooltip("窒息蓄積倍率")] public float suffocationAddMult;
    [Tooltip("窒息回復倍率")] public float suffocationRemMult;
    [Tooltip("移動した時の音距離")] public float move_soundRange;
    [Tooltip("移動した時の音時間")] public float move_soundTime;
    [Tooltip("SE音量"), Range(0, 1)] public float seVolue = 1;
    [Tooltip("SEピッチ"), Range(-3, 3)] public float sePitch = 1;
}

[System.Serializable]
public struct BufSet : INetworkStruct
{
    public BufType buf;
    public float timeCurrent;
    public float timeMax;
}
[System.Serializable]
public class BufAdd
{
    public BufType buf;
    public float time;
}
#endregion
#region Enum
/// <summary>
/// 現在の移動ステータス
/// </summary>
public enum MoveState
{
    Stop, //停止
    Walk, //歩き
    Move, //通常
    Dash  //走る
}

/// <summary>
/// 現在のカーソルステータス
/// </summary>
public enum CursorState
{
    Shot,
    Melee,
    Gadget,
}
public enum Passive
{
    Normal = 0,

    HPMaxAdd,
    HPRegTimeRem,
    InjustRegist,

    ViewAdd,
    CamSizeAdd,

    SpeedAdd,
    SilrentMove,

    ReloadAdd,
    SilrentAtk,
    DifRem,
    RecoilRem,
    RPSAdd,
    BulletQuiq,
    DamageAdd,
    DamageHeadAdd,

    GGCTRem,

    ScoreAllAdd,
    ScoreDamageAdd,
    ScoreKillAdd,
    ScoreHeadKillAdd,

    GGStockAdd,
    GGRangeAdd,
    GGDamageAdd,

    UltChargeAdd,
    UltChargeTimeAdd,
    UltChargeAddDamAdd,
    UltChargeHitDamAdd,
    UltChargeDogeAdd,
    UltChargeKillAdd,

    HideHP,

    DogeAdd,

    SlowAreaRegist,
    DamageAreaRegist,

    KBRegist,
    QuiqHand,
    SpinAdd,

    Ex = 1000,
    Lantan,
    MagazineDouble,
    Tank,
    HevBullet,
    LightBullet,
    SoundMult,
    HeadHunter,
    Undeath,
    InjuryLife,
    DeathEscape,
    Small,
    Big,
    Revenjer,
    DogeScore,
    WaterReverce,
    BadRoadOwner,
    Fly,

    Negative =2000,
    Blind,
    WeekHP,
    LongReload,
    Transmitter,
    NoHearing,
    DeathPenarulty,
    NoEnemyHPUI,
    BadAreaMult,
    ClumsyHand,
    SpinSlow,

    Cheat = 9000,
    Cheat_Speed,
    Cheat_HP,
    Cheat_Regene,
    Cheat_Injust,
    Cheat_Reload,
    Cheat_GG,
    Cheat_View,
    Cheat_GPS,
    Cheat_Ult,
    Cheat_Visible,
    Cheat_Respawne,
    Cheat_NoMoveSound,
    Cheat_NoAtkSound,
}
public enum AtkID
{
    Gun = CSize * 0,
    Melee = CSize * 1,
    Gadget = CSize * 2,
    Ult = CSize * 3,
    CSize = 10000000,
}
public enum BufType
{
    InfinityMagazin,
    GadgetBoost,
    Runner,
    Visivle,
    WallHack,

    Poison = 1000,
    Fire,
    Iced = 1100,
    Confusion,
    Darkness,
    SpotLit,
    Gravitation,
    Laceration,
    Shock,
}
#endregion

public class Player_Manager : NetworkBehaviour
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    public PlayerStates states;
    public PlayerObjects objects;
    public PlayerValues values;
    public PlayerPassiveChanges passc;
    #endregion
    #region その他
    [Header("◆その他")]
    public Player_Controlle controlle;
    public UI_Damage damageObj;
    List<UI_Damage> cdams = new();
    #endregion
    #endregion
    //取得
    public float hpTotal { get { return values.hpNow + values.hpOver + values.hpShild; } }
    public int magazinMax
    {
        get
        {
            var cap = Data_Base.DB.guns[states.gun_IndexNum].capacity;
            if (cap <= 0) return cap;
            return Mathf.CeilToInt(cap * passc.magazine_Multi);
        }
    }
    public int meleeMax
    {
        get
        {
            var cap = Data_Base.DB.melles[states.melee_IndexNum].capacity;
            if(cap <= 0)return cap;
            return Mathf.CeilToInt(cap * passc.magazine_Multi);
        }
    }
    public Vector3 PosGet { get { return objects.rb.transform.position; } }
    public Transform TransGet { get { return objects.rb.transform; } }
    public bool NearDeath{get{return hpTotal <= states.max_HP * 0.25f;}}
    public bool View(Player_Manager pm)
    {
        var view = pm.states.teamID == states.teamID;
        if (PassiveLvGet(Passive.Lantan) > 0) view = true;
        if (BufGet(BufType.Fire)) view = true;
        if (values.killLight) view = true;
        return view;
    }
    public bool Visible
    {
        get
        {
            var visible = false;
            if (PassiveLvGet(Passive.Cheat_Visible) > 0) visible = true;
            if (BufGet(BufType.Visivle)) visible = true;
            return visible;
        }
    }
    public int PassiveLvGet(Passive pass)
    {
        for (int i = 0; i < states.passives.Count; i++)
        {
            if (states.passives[i].x == (int)pass) return states.passives[i].y;
        }
        return 0;
    }
    public float PassiveValGet(Passive pass, int addValueIndex = 0)
    {
        var lv = PassiveLvGet(pass);
        var psd = Data_Base.PassiveDGet(pass);
        if (psd == null) return 1f;
        var pv = psd.PassiveValues[addValueIndex];
        if (lv <= 0) return pv.noValue;
        return pv.values[Mathf.Min(lv - 1, pv.values.Length - 1)];
    }
    public bool BufGet(BufType _buf)
    {
        return values.bufs.FindIndex(x => x.buf == _buf) >= 0;
    }
    public bool StopMove
    {
        get
        {
            if (!Obj_LocalObjects.TimeStopd) return true;
            for(int i = 0; i < Obj_LocalObjects.TimeStops.Count; i++)
            {
                var ts = Obj_LocalObjects.TimeStops[i];
                if (ts == null) continue;
                if (ts.Object == null) continue;
                if (ts.pm == this) return true;
            }
            return false;
        }
    }
    public MoveStateData MoveStateGet
    {
        get
        {
            MoveStateData mvd = states.set_MoveState[0];
            foreach (var data in states.set_MoveState)
            {
                if (data.moveState == values.now_MoveState)
                {
                    mvd = data;
                    break;
                }
            }
            return mvd;
        }

    }
    //
    void Start()
    {
        Obj_LocalObjects.Players.Add(this);
        if (Net_Connect.CanControl(Object))
        {
            if(Object.InputAuthority != PlayerRef.None)
            {
                Obj_LocalObjects.MyPlayer = this;
                states.name = Net_Connect.PlayerName;
                states.passives = Player_Sets.CSetGet.passives;

                if (UI_TeamSet.teamID == 0)
                {
                    var teamCounts = new int[5];
                    for(int i = 0; i < Obj_LocalObjects.Players.Count; i++)
                    {
                        var pl = Obj_LocalObjects.Players[i];
                        if (pl == null) continue;
                        if (pl == this) continue;
                        teamCounts[pl.states.teamID]++;
                    }
                    var minTeam = 0;
                    var minCount = int.MaxValue;
                    for(int i = 0; i < teamCounts.Length; i++)
                    {
                        if (!Net_Value.NetValue.teamOn[i]) continue;
                        if(minCount > teamCounts[i])
                        {
                            minTeam = i;
                            minCount = teamCounts[i];
                        }
                    }
                    states.teamID = minTeam;
                }
                else states.teamID = UI_TeamSet.teamID - 1;
                Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.PlayerIO, "プレイヤー参戦", states.name);
            }
            else Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.PlayerIO, "CPU参戦", states.name);
            PassiveSet();
        }
        Respawne();
        objects.ownerOnlys.SetActive(this == Obj_LocalObjects.MyPlayer);
        objects.ownerOnlys.transform.position = PosGet;
    }

    void Update()
    {
        objects.ownerOnlys.SetActive(this == Obj_LocalObjects.MyPlayer);
        objects.ownerOnlys.transform.position = PosGet;
        if (Obj_LocalObjects.MyPlayer == this)
        {
            states.gun_IndexNum = Player_Sets.CSetGet.gunID;
            states.melee_IndexNum = Player_Sets.CSetGet.meleeID;
            states.gadget_IndexNum = Player_Sets.CSetGet.gadgetID;
            states.ult_IndexNum = Player_Sets.CSetGet.ultID;
            states.passives = Player_Sets.CSetGet.passives;
            states.charaImgID = Player_Sets.CSetGet.charaImgID;
            states.loadImgID = Player_Sets.CSetGet.loadImgID;
        }
        PassiveSet();
        BufSet();
        objects.scaleTrans.localScale = Vector3.one * passc.charaScale_Multi;
        objects.rigSync.noStop = StopMove;

        if (Net_Connect.CanControl(Object))
        {
            if (StopMove)
            {
                //生死判定
                if (hpTotal <= 0) DeathUpdate();
                else LiveUpdate();
            }
            if (values.killcons >= Net_Value.NetValue.killConsLight && !values.killLight)
            {
                values.killLight = true;
                Net_Log.NetLog.RPC_LogAddPl(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.Event, "連続キル発光!!!", this);
            }
        }
    }
    public void PassiveSet()
    {
        var bp = Data_Base.DB.basePlayer;
        //補正初期設定
        states.max_HP = bp.states.max_HP * PassiveValGet(Passive.HPMaxAdd);
        states.vision_Angle = bp.states.vision_Angle + PassiveValGet(Passive.ViewAdd);
        passc.regeneDelay_Multi = PassiveValGet(Passive.HPRegTimeRem);
        passc.soundGet_Multi = 1;
        passc.moveSound_Multi= PassiveValGet(Passive.SilrentMove);
        passc.moveSpeed_Multi = PassiveValGet(Passive.SpeedAdd);
        passc.damageAllAdd_Multi = PassiveValGet(Passive.DamageAdd);
        passc.damageHeadAdd_Multi = PassiveValGet(Passive.DamageHeadAdd);
        passc.damageGunAdd_Multi = 1f;
        passc.damageMeleeAdd_Multi = 1f;

        passc.changeWaitTime_Mult = PassiveValGet(Passive.QuiqHand, 0);
        passc.ggUseTime_Mult = PassiveValGet(Passive.QuiqHand, 1);
        passc.changeWaitTime_Mult *= PassiveValGet(Passive.ClumsyHand, 0);
        passc.ggUseTime_Mult *= PassiveValGet(Passive.ClumsyHand, 1);

        passc.bulletRPS_Multi = PassiveValGet(Passive.RPSAdd);
        passc.bulletSpeed_Multi = PassiveValGet(Passive.BulletQuiq);
        passc.reload_Multi = PassiveValGet(Passive.ReloadAdd);
        passc.diffusion_Multi = PassiveValGet(Passive.DifRem);
        passc.recoil_Multi = PassiveValGet(Passive.RecoilRem);
        passc.atkSound_Multi = PassiveValGet(Passive.SilrentAtk);
        passc.ggCoolTime_Multi = PassiveValGet(Passive.GGCTRem);
        passc.ggStockAdd = (int)PassiveValGet(Passive.GGStockAdd);
        passc.ggRange_Multi = PassiveValGet(Passive.GGRangeAdd);
        passc.damageGGAdd_Multi = PassiveValGet(Passive.GGDamageAdd);

        passc.injust_Multi = PassiveValGet(Passive.InjustRegist);
        passc.magazine_Multi = 1f;
        objects.camera.orthographicSize = bp.objects.camera.orthographicSize * PassiveValGet(Passive.CamSizeAdd);

        passc.scoreAll_Multi = PassiveValGet(Passive.ScoreAllAdd);
        passc.scoreDamage_Multi = PassiveValGet(Passive.ScoreDamageAdd);
        passc.scoreKill_Multi = PassiveValGet(Passive.ScoreKillAdd);
        passc.scoreHeadKill_Multi = PassiveValGet(Passive.ScoreHeadKillAdd);
        passc.ultCharge_Multi = PassiveValGet(Passive.UltChargeAdd);
        passc.ultChargeTime_Multi = PassiveValGet(Passive.UltChargeTimeAdd);
        passc.ultChargeAddDam_Multi = PassiveValGet(Passive.UltChargeAddDamAdd);
        passc.ultChargeHitDam_Multi = PassiveValGet(Passive.UltChargeHitDamAdd);
        passc.ultChargeDoge_Multi = PassiveValGet(Passive.UltChargeDogeAdd);
        passc.ultChargeKill_Multi = PassiveValGet(Passive.UltChargeKillAdd);
        passc.respawneSpeed_Multi = 1;
        passc.dogeScale_Multi = PassiveValGet(Passive.DogeAdd);

        passc.slowArea_Regist = PassiveValGet(Passive.SlowAreaRegist);
        passc.damageArea_Regist = PassiveValGet(Passive.DamageAreaRegist);

        passc.slowArea_Regist *= PassiveValGet(Passive.BadAreaMult, 0);
        passc.damageArea_Regist *= PassiveValGet(Passive.BadAreaMult, 1);

        passc.kb_Regist = PassiveValGet(Passive.KBRegist);
        passc.spinSpeed_Multi = PassiveValGet(Passive.SpinAdd);
        //パッシブ
        states.max_HP *= PassiveValGet(Passive.WeekHP);
        states.vision_Angle *= PassiveValGet(Passive.Blind);
        passc.reload_Multi *= PassiveValGet(Passive.LongReload);
        passc.soundGet_Multi *= PassiveValGet(Passive.NoHearing);

        passc.lantan = PassiveLvGet(Passive.Lantan) > 0;
        passc.transmitter = PassiveValGet(Passive.Transmitter);
        passc.magazine_Multi *= PassiveValGet(Passive.MagazineDouble,0);
        passc.reload_Multi *= PassiveValGet(Passive.MagazineDouble,1);
        states.max_HP *= PassiveValGet(Passive.Tank, 0);
        passc.moveSpeed_Multi *= PassiveValGet(Passive.Tank, 1);
        passc.damageGunAdd_Multi *= PassiveValGet(Passive.HevBullet, 0);
        passc.bulletRPS_Multi *= PassiveValGet(Passive.HevBullet, 1);
        passc.bulletSpeed_Multi *= PassiveValGet(Passive.HevBullet, 2);
        passc.damageGunAdd_Multi *= PassiveValGet(Passive.LightBullet, 0);
        passc.bulletRPS_Multi *= PassiveValGet(Passive.LightBullet, 1);
        passc.bulletSpeed_Multi *= PassiveValGet(Passive.LightBullet, 2);

        passc.soundGet_Multi *= PassiveValGet(Passive.SoundMult, 0);
        passc.moveSound_Multi *= PassiveValGet(Passive.SoundMult, 1);
        passc.atkSound_Multi *= PassiveValGet(Passive.SoundMult, 1);

        passc.scoreKill_Multi *= PassiveValGet(Passive.HeadHunter, 0);
        passc.scoreHeadKill_Multi *= PassiveValGet(Passive.HeadHunter, 1);

        passc.respawneSpeed_Multi *= PassiveValGet(Passive.Undeath);

        passc.injust_Multi *= PassiveValGet(Passive.InjuryLife);

        passc.charaScale_Multi = 1;
        passc.charaScale_Multi *= PassiveValGet(Passive.Small,0);
        states.max_HP *= PassiveValGet(Passive.Small,1);
        passc.charaScale_Multi *= PassiveValGet(Passive.Big);
        states.max_HP *= PassiveValGet(Passive.Big, 1);

        states.max_HP *= PassiveValGet(Passive.DogeScore, 1);

        passc.moveSpeed_Multi *= PassiveValGet(Passive.BadRoadOwner, 0);

        passc.kb_Regist *= PassiveValGet(Passive.Fly, 0);
        passc.moveSound_Multi *= PassiveValGet(Passive.Fly, 1);

        passc.spinSpeed_Multi *= PassiveValGet(Passive.SpinSlow);

        if (NearDeath) passc.moveSpeed_Multi *= PassiveValGet(Passive.DeathEscape,0);
        if (Net_Value.NetValue.options[2])
        {
            passc.moveSpeed_Multi *= PassiveValGet(Passive.Cheat_Speed);
            states.max_HP += PassiveValGet(Passive.Cheat_HP);
            passc.regeneDelay_Multi *= PassiveValGet(Passive.Cheat_Regene);
            passc.injust_Multi *= PassiveValGet(Passive.Cheat_Injust);
            passc.reload_Multi *= PassiveValGet(Passive.Cheat_Reload);
            passc.ggCoolTime_Multi *= PassiveValGet(Passive.Cheat_GG);
            passc.ultCharge_Multi *= PassiveValGet(Passive.Cheat_Ult);
            passc.respawneSpeed_Multi *= PassiveValGet(Passive.Cheat_Respawne);
            passc.moveSound_Multi *= PassiveValGet(Passive.Cheat_NoMoveSound);
            passc.atkSound_Multi *= PassiveValGet(Passive.Cheat_NoAtkSound);
        }
        else
        {
            var check = false;
            int cost = 0;
            for (int i = 0; i < states.passives.Count; i++)
            {
                var pss = states.passives[i];
                var psd = Data_Base.PassiveDGet((Passive)pss.x);
                if (psd != null) cost += psd.CostGet(pss.y);
                if (states.passives[i].x >= (int)Passive.Cheat)
                {
                    check = true;
                    break;
                }
            }
            if(cost > Data_Base.DB.CostMax)check=true;
            if(check)passc.moveSpeed_Multi = 0.01f;
        }
    }
    public void BufSet()
    {
        if (BufGet(BufType.InfinityMagazin))
        {
            passc.magazine_Multi = -1;
            passc.bulletRPS_Multi *= Data_Base.BufDGet(BufType.InfinityMagazin).values[0];
        }
        if (BufGet(BufType.GadgetBoost)) passc.ggCoolTime_Multi *= Data_Base.BufDGet(BufType.GadgetBoost).values[0];
        if (BufGet(BufType.Runner)) passc.moveSpeed_Multi *= Data_Base.BufDGet(BufType.Runner).values[0];
        if (BufGet(BufType.Iced)) passc.moveSpeed_Multi *= Data_Base.BufDGet(BufType.Iced).values[0];
        if (BufGet(BufType.Confusion)) passc.moveSpeed_Multi *= -1;
        if (BufGet(BufType.Shock)) passc.moveSpeed_Multi *= Data_Base.BufDGet(BufType.Shock).values[0];
    }
    void DeathUpdate()
    {
        if (values.deathTime <= 0)
        {
            var frienddeath = values.lastAtkPm == null || values.lastAtkPm.states.teamID == states.teamID;
            if (!frienddeath)
            {
                values.lastAtkPm.KillChange(0);
                if (BufGet(BufType.SpotLit)) values.killScoreMult *= Data_Base.BufDGet(BufType.SpotLit).values[0];
                values.killScoreMult *= PassiveValGet(Passive.DeathEscape, 1);
                values.lastAtkPm.ScoreChange(500 * Net_Value.NetValue.scoreMults[4] * values.killScoreMult);
                values.lastAtkPm.UltCharge(Data_Base.DB.ultCharge[4] * passc.ultCharge_Multi);
                Net_Log.NetLog.RPC_LogAddKill(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.Kill, "キル", values.lastAtkPm, values.lastAtkID, this);
                if (values.killLight) values.lastAtkPm.ScoreChange(100 * values.killcons * Net_Value.NetValue.scoreMults[11] * values.killScoreMult);
                values.lastAtkPm.RPC_KillEvent(this,values.killHeadShot);
                if (PassiveLvGet(Passive.Revenjer) > 0 && values.lastAtkKillPm == values.lastAtkPm)ScoreChange(-PassiveValGet(Passive.Revenjer, 1) * Net_Value.NetValue.scoreMults[6]);
                values.lastAtkKillPm = values.lastAtkPm;
            }
            else if (values.lastAtkPm != null && values.lastAtkPm != this)
            {
                values.lastAtkPm.KillChange(2);
                values.lastAtkPm.ScoreChange(-250 * Net_Value.NetValue.scoreMults[7]);
                Net_Log.NetLog.RPC_LogAddKill(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.FFSD, "FF!!", values.lastAtkPm, values.lastAtkID, this);
            }
            else
            {
                KillChange(1);
                ScoreChange(-250 * Net_Value.NetValue.scoreMults[6]);
                Net_Log.NetLog.RPC_LogAddKill(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.FFSD, "自滅!!", this,values.lastAtkID,this);
            }
            switch (PassiveLvGet(Passive.DeathPenarulty))
            {
                case 1:
                    if(values.now_Retention > 0)values.now_Retention--;
                    else values.now_GetTime = 0;
                    break;
                case 2:
                    values.now_Retention = 0;
                    values.now_GetTime = 0;
                    break;
            }
            values.ultCharge *= PassiveValGet(Passive.DeathPenarulty);
            values.lastAtkID = 0;
        }
        values.deathTime += Time.deltaTime / passc.respawneSpeed_Multi;
        if (values.deathTime >= 5) Respawne();
        return;
    }
    void LiveUpdate()
    {
        values.deathTime = 0;
        //自動回復(上限：負傷の値)
        if (BufGet(BufType.Poison))
        {
            HPRem(Data_Base.BufDGet(BufType.Poison).values[0] * Time.deltaTime,0,true);
            values.hpRegeneDelay = 0;
        }
        if (BufGet(BufType.Fire)) HPRem(Data_Base.BufDGet(BufType.Fire).values[0] * Time.deltaTime,0, true);
        var water = values.moveType != null && values.moveType.suffocation;
        if (PassiveLvGet(Passive.WaterReverce) > 0) water = !water;
        var mvd = MoveStateGet;
        if (water)
        {
            values.suffocationTime += Time.deltaTime * mvd.suffocationAddMult;
            values.suffocationDamCT -= Time.deltaTime * mvd.suffocationAddMult;
        }
        else
        {
            values.suffocationTime -= Time.deltaTime * mvd.suffocationRemMult;
        }
        values.suffocationTime = Mathf.Clamp(values.suffocationTime, 0, states.suffocationTime);
        if(values.suffocationTime >= states.suffocationTime && values.suffocationDamCT <= 0)
        {
            values.suffocationDamCT = 2f;
            Damage(10, 0, false, this, -10000, 0, 0);
        }
        if (BufGet(BufType.Laceration))
        {
            int moves;
            switch (values.now_MoveState)
            {
                case MoveState.Stop:moves = 0;break;
                case MoveState.Walk:moves = 1;break;
                default:moves = 2;break;
                case MoveState.Dash:moves = 3;break;
            }
            HPRem(Data_Base.BufDGet(BufType.Laceration).values[moves + 1] * Time.deltaTime, Data_Base.BufDGet(BufType.Laceration).values[0], true);
        }
        if (values.hpRegeneDelay > states.weit_RegeneDelay)
        {
            if (values.hpNow < values.hpInjury)
            {
                float regene = states.regeneHP * Time.deltaTime;

                values.hpNow += regene;
                values.hpNow = Mathf.Min(values.hpNow, values.hpInjury);
            }
        }
        else { values.hpRegeneDelay += Time.deltaTime / passc.regeneDelay_Multi; }
        values.noDamTime -= Time.deltaTime;
        //バフ時間
        for(int i = values.bufs.Count -1; i >= 0; i--)
        {
            var bufd = values.bufs[i];
            bufd.timeCurrent-=Time.deltaTime;
            if (bufd.timeCurrent <= 0) values.bufs.RemoveAt(i);
            else values.bufs[i] = bufd;
        }
    }


    /// <summary>
    /// ダメージ or 回復
    /// </summary>
    public void Damage(float val, float injustPer, bool headshot, Player_Manager atkPm,int lastAtkID,float damageScoreMult,float killScoreMult)
    {
        RPC_Damage(val,injustPer, headshot,atkPm,lastAtkID,damageScoreMult, killScoreMult);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Damage(float val,float injustPer, bool headshot,Player_Manager atkPm, int lastAtkID, float damageScoreMult, float killScoreMult)
    {
        //無敵時間
        if (values.noDamTime > 0 && val >= 0) return;
        //死亡中回復不可
        if (hpTotal <= 0 && val < 0) return;
        bool death = hpTotal <= 0;
        #region ダメージ or 回復の適応
        //ダメージ：負傷させる
        if (val > 0)
        {
            HPRem(val, injustPer);
        }
        //回復：オーバーヒール分を計算
        else if (val < 0)
        {
            float heal = -val;

            values.hpNow += heal;

            //負傷超過分をオーバーヒール化
            if (values.hpNow > values.hpInjury)
            {
                values.hpOver = Mathf.Max(values.hpOver, values.hpNow - values.hpInjury);
                values.hpNow = values.hpInjury;
            }
        }

        //回復の場合は即時自動回復開始
        if(val != 0)values.hpRegeneDelay = val > 0 ? 0f : states.weit_RegeneDelay;

        //値を丸める
        values.hpInjury = Mathf.Clamp(values.hpInjury, 0, states.max_HP);
        values.hpOver = Mathf.Clamp(values.hpOver, 0, states.max_HP);
        values.hpNow = Mathf.Clamp(values.hpNow, 0, values.hpInjury);

        #endregion
        #region 値の可視化(文字生成)
        bool kill = hpTotal <= 0;
        UI_Damage cdam = null;
        cdams.RemoveAll(x => x == null);
        for (int i = 0; i < cdams.Count; i++)
        {
            if (cdams[i].Check(val,headshot, kill))
            { cdam = cdams[i]; break; }
        }
        if (cdam == null)
        {
            Vector3 damPos = PosGet;
            damPos.x += Random.value - 0.5f;
            var damageIns = Instantiate(damageObj, damPos, Quaternion.identity);
            damageIns.transform.parent = objects.damageCanvas;
            damageIns.SetDamage(val, headshot, kill);
            cdams.Add(damageIns);
        }
        //値の送信
        else
        { cdam.SetDamage(val, headshot, kill); }
        #endregion
        values.lastAtkPm = atkPm;
        values.lastAtkID = lastAtkID;
        values.killHeadShot = headshot;
        values.killScoreMult = killScoreMult;
        if (death) return;
        var team = atkPm.states.teamID == states.teamID;
        if (val > 0)
        {
            values.takeDamages += val;
            atkPm.values.addDamages += val;
            atkPm.ScoreChange(val * Net_Value.NetValue.scoreMults[!team ? 0 : 2] * (!team ? 1 : -1) * damageScoreMult);
            if(!team && lastAtkID < (int)AtkID.Ult)atkPm.UltCharge(val * atkPm.passc.ultChargeAddDam_Multi * Data_Base.DB.ultCharge[1] * 0.01f);
            switch (atkPm.PlayerTypeGet)
            {
                case 0:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.AddDamageTotal_PL, val);
                    break;
                case 2:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.AddDamageTotal_CPU, val);
                    break;
            }
            switch (PlayerTypeGet)
            {
                case 0:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.TakeDamageTotal_PL, val);
                    break;
                case 2:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.TakeDamageTotal_CPU, val);
                    break;
            }
        }
        else if(val < 0)
        {
            atkPm.values.addHeals += -val;
            atkPm.ScoreChange(-val * 2.5f * Net_Value.NetValue.scoreMults[team ? 1 : 3] * (team ? 1 : -1) * damageScoreMult);
            if (team && lastAtkID < (int)AtkID.Ult) atkPm.UltCharge(-val * 2.5f * atkPm.passc.ultChargeAddDam_Multi * Data_Base.DB.ultCharge[1] * 0.01f);
            switch (atkPm.PlayerTypeGet)
            {
                case 0:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.AddHealTotal_PL, -val);
                    break;
                case 2:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.AddHealTotal_CPU, -val);
                    break;
            }
            switch (PlayerTypeGet)
            {
                case 0:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.TakeHealTotal_PL, val);
                    break;
                case 2:
                    UI_Statistics.AddValue(UI_Statistics.StatEnum.TakeHealTotal_CPU, val);
                    break;
            }
        }
    }
    void HPRem(float val,float injustPer,bool slip = false)
    {
        bool death = hpTotal <= 0;
        float dmg = val;
        //シールド
        if (values.hpShild > 0)
        {
            float shild_dmg = Mathf.Min(values.hpShild, dmg);
            values.hpShild -= shild_dmg;
            dmg -= shild_dmg;
        }
        //負傷
        values.hpInjury -= dmg * injustPer * 0.01f * passc.injust_Multi;
        //先にオーバーヒール分から減らす
        if (dmg > 0 && values.hpOver > 0)
        {
            float overHP_dmg = Mathf.Min(values.hpOver, dmg);
            values.hpOver -= overHP_dmg;
            dmg -= overHP_dmg;
        }
        //残りダメージを通常HPへ
        if (dmg > 0)
        {
            float chp_dmg = Mathf.Min(values.hpNow, dmg);
            values.hpNow -= chp_dmg;
            dmg -= chp_dmg;
            if(values.hpNow <= 0 && PassiveLvGet(Passive.InjuryLife) > 0 && values.hpInjury > 0)
            {
                values.hpInjury -= dmg;
                if(values.hpInjury > 0)values.hpNow = 1;
            }
        }
        if (slip)
        {
            values.hpNow = Mathf.Max(values.hpNow, 0.01f);
            values.hpInjury = Mathf.Max(values.hpInjury, 0.01f);
        }
        else if(val > 0 & !death)
        {
            UltCharge(val * passc.ultChargeHitDam_Multi * Data_Base.DB.ultCharge[2] * 0.01f);
        }
    }
    /// <summary>
    /// バフ付与
    /// </summary>
    public void BufChanges(BufAdd[] bufAdds)
    {
        if (bufAdds == null) return;
        for(int i = 0; i < bufAdds.Length; i++)
        {
            RPC_BufChange((int)bufAdds[i].buf, bufAdds[i].time);
        }
    }
    public void BufChange(BufType _buf, float _time)
    {
        RPC_BufChange((int)_buf, _time);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_BufChange(int _buf, float _time)
    {
        int id = values.bufs.FindIndex(x => x.buf == (BufType)_buf);
        if (id < 0)
        {
            values.bufs.Add(new BufSet { buf = (BufType)_buf, timeCurrent = _time, timeMax = _time });
        }
        else
        {
            var bufs = values.bufs[id];
            bufs.buf = (BufType)_buf;
            bufs.timeCurrent = Mathf.Max(bufs.timeCurrent, _time);
            bufs.timeMax = Mathf.Max(bufs.timeMax, _time);
            values.bufs[id] = bufs;
        }
    }

    public void KBSet(Vector2 vect)
    {
        RPC_KBSet(vect);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_KBSet(Vector2 vect)
    {
        vect *= passc.kb_Regist;
        objects.rb.AddForce(vect);
    }
    public void KillChange(byte type)
    {
        RPC_KillChange(type);
        Net_Value.NetValue.RPC_KillChange(states.teamID, type != 0);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_KillChange(byte type)
    {
        values.kill += type == 0 ? 1 : -1;
        switch (type)
        {
            case 0:
                switch (PlayerTypeGet)
                {
                    case 0:
                        UI_Statistics.AddValue(UI_Statistics.StatEnum.KillTotal_PL, 1);
                        break;
                    case 2:
                        UI_Statistics.AddValue(UI_Statistics.StatEnum.KillTotal_CPU, 1);
                        break;
                }
                break;
            default:
                switch (PlayerTypeGet)
                {
                    case 0:
                        UI_Statistics.AddValue(UI_Statistics.StatEnum.SelfDeathTotal_PL, 1);
                        break;
                    case 2:
                        UI_Statistics.AddValue(UI_Statistics.StatEnum.SelfDeathTotal_CPU, 1);
                        break;
                }
                break;
            case 2:
                switch (PlayerTypeGet)
                {
                    case 0:
                        UI_Statistics.AddValue(UI_Statistics.StatEnum.FFTotal_PL, 1);
                        break;
                    case 2:
                        UI_Statistics.AddValue(UI_Statistics.StatEnum.FFTotal_CPU, 1);
                        break;
                }
                break;
        }
    }
    public void ScoreChange(float val)
    {
        val *= passc.scoreAll_Multi;
        if (val == 0) return;
        Net_Value.NetValue.RPC_ScoreChange(states.teamID, val);
        RPC_ScoreChange(val);
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    void RPC_ScoreChange(float val)
    {
        values.score += val;
    }
    public void UltCharge(float val)
    {
        if (!Net_Connect.CanControl(Object)) RPC_UltCharge(val);
        else UltCharge_Lc(val);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_UltCharge(float val)
    {
        UltCharge_Lc(val);
    }
    void UltCharge_Lc(float val)
    {
        val *= passc.ultCharge_Multi;
        values.ultCharge += val;
        var ultd = Data_Base.DB.ults[states.ult_IndexNum];
        values.ultCharge = Mathf.Min(values.ultCharge, ultd.chargeValue);
    }
    public void Respawne()
    {
        var pos = Net_Spawnes.NetSpawnes.Points[states.teamID].position;
        TransGet.position = pos;
        values.hpInjury = states.max_HP;
        values.hpNow = states.max_HP;
        values.hpOver = 0;
        values.hpShild = 0;
        values.gun_bullet = magazinMax;
        values.melee_bullet = meleeMax;
        values.suffocationTime = 0;
        values.killcons = 0;
        values.killLight = false;
        values.bufs.Clear();
        if (PassiveLvGet(Passive.DeathPenarulty) < 0)
        {
            var scount = Data_Base.DB.gadgets[states.gadget_IndexNum].start_Retention;
            if(values.now_Retention < scount)
            {
                values.now_Retention = scount;
                values.now_GetTime = 0;
            }
        }
        values.noDamTime = 5;
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_KillEvent(Player_Manager kpm,bool head)
    {
        if (!Net_Connect.CanControl(Object)) return;
        values.killcons++;
        values.killcmax = Mathf.Max(values.killcons, values.killcmax);
        var headMult = !head ? 1 : passc.scoreHeadKill_Multi;
        if(values.killcons > 1)ScoreChange(50 * (values.killcons - 1) * passc.scoreKill_Multi * headMult * Net_Value.NetValue.scoreMults[5]);
        if (PassiveLvGet(Passive.Revenjer) > 0 && values.lastAtkKillPm == kpm)
        {
            ScoreChange(PassiveValGet(Passive.Revenjer,0) * passc.scoreKill_Multi * headMult * Net_Value.NetValue.scoreMults[4]);
        }
        values.lastAtkKillPm = null;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Resets()
    {
        values.kill = 0;
        values.killcons = 0;
        values.killcmax = 0;
        values.score = 0;

        values.addDamages = 0;
        values.takeDamages = 0;
        values.addHeals = 0;

        values.now_Retention = Data_Base.DB.gadgets[states.gadget_IndexNum].start_Retention;
        values.now_GetTime = 0;
        values.ultCharge = 0;
        if (values.now_CursorState == CursorState.Gadget) values.now_CursorState = CursorState.Shot;
        Respawne();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GameEnd()
    {
        values.team_back = states.teamID;
        values.kill_back = values.kill;
        values.killcmax_back = values.killcmax;
        values.score_back = values.score;

        values.addDamages_back = values.addDamages;
        values.takeDamages_back = values.takeDamages;
        values.addHeals_back = values.addHeals;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TeamSet(int team)
    {
        states.teamID = team;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ViewChange()
    {
        Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.PlayerIO, "プレイヤー観戦化", states.name);
        Destroy(gameObject);
    }
    /// <summary>
    /// 0=自プレイヤー,1=他プレイヤー,2=自CPU,3=他CPU
    /// </summary>
    public byte PlayerTypeGet
    {
        get
        {
            if (Obj_LocalObjects.MyPlayer == this) return 0;
            else if (Object.InputAuthority != PlayerRef.None) return 1;
            else if (HasStateAuthority) return 2;
            else return 3;
        }
    }
}
