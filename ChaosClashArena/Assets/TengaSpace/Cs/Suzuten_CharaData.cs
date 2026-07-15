using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_DataBase;
[CreateAssetMenu(menuName = "SuzutenDCre/CharaData")]
public class Suzuten_CharaData : ScriptableObject
{
    [Header("基本情報")]
    [Tooltip("キャラ名")]
    public string CharaName;
    [Tooltip("キャラ名改行")]
    public int CharaNameEnter;
    [Tooltip("キャラ画像")]
    public Texture CharaImage;
    [TextArea, Tooltip("キャラ説明")]
    public string CharaInfo;
    [Tooltip("キャラオブジェ(バトル時)")]
    public Suzuten_PlayerState CharaObj;
    [Tooltip("キャラモデル(選択時)")]
    public GameObject ModelObj;
    [Tooltip("ランダムで出現しない")]
    public bool RandomNoSelects;
    [Tooltip("使用禁止キャラ")]
    public bool UseBandChara;
    [Tooltip("Y座標変化")]
    public float YPos;
    [Tooltip("重力影響%")]
    public float GravPower;

    [Header("衝突攻撃エフェクト")]
    [Tooltip("衝突攻撃発生エフェクト")]
    public ParticleSystem PhisAtkEffect;
    [Tooltip("衝突攻撃命中エフェクト")]
    public ParticleSystem PhisHitEffect;
    [Tooltip("衝突攻撃速度トレール")]
    public TrailRenderer SpeedTrail;
    [Header("ステータス")]
    [Tooltip("最大HP")]
    public int MHP;
    [Tooltip("HP自動回復速度")]
    public float HPRegene = 0.01f;
    [Tooltip("スタン値")]
    public int StanRegist;
    [Tooltip("スタン値回復速度")]
    public float STRegene = 0.1f;
    [Tooltip("最大SP")]
    public int MSP;
    [Tooltip("SP自動回復速度")]
    public float SPRegene = 1f;
    [Tooltip("初期SP%")]
    public float StartSPPer = 50f;
    [Tooltip("ダメージSP増加率%")]
    public float DamageSPPer = 100f;
    [Tooltip("攻撃力")]
    public int Atk;
    [Tooltip("最大MP")]
    public int MMP;
    [Tooltip("MP自動回復速度")]
    public float MPRegene = 1f;
    [Tooltip("衝突攻撃力")]
    public float PhisPow;
    [Tooltip("衝突攻撃範囲")]
    public float PhisRange;
    [Tooltip("衝突攻撃スタン時間")]
    public int PhisStanTime;
    [Tooltip("地上移動速度")]
    public float GroundSpeed;
    [Tooltip("空中移動速度")]
    public float AirSpeed;
    [Tooltip("ブースト速度")]
    public float BoostSpeed;
    [Tooltip("ジャンプ力")]
    public float JumpPower;
    [Tooltip("落下力")]
    public float DownPower;
    [Tooltip("アクション")]
    public Suzuten_ActionData[] Actions;

    public SEPlayC SelectSE;
    public SEPlayC StartSE;
    public SEPlayC WINSE;
    public SEPlayC LoseSE;
    public SEPlayC BoostSE;
    public SEPlayC GuardSE;
    public SEPlayC JumpSE;
    public SEPlayC FallSE;
    public SEPlayC PhisAtkSE;
    public SEPlayC StanSE;
    public SEPlayC DamageSE;
    public SEPlayC DamGuardSE;
    public SEPlayC DownSE;



}
