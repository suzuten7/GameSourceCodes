using System;
using System.Collections.Generic;
using UnityEngine;
/* 内容
 * ・銃のデータ
 */

/// <summary>
/// 銃のデータ
/// </summary>
[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/Gun")]
public class Data_Gun : ScriptableObject
{
    [Header("◆メインオプション")]
    [Header("- メイン -")]
    [Tooltip("名前")] public string name;
    [Tooltip("説明"), TextArea] public string info;
    [Tooltip("カテゴリー分け")] public GunCategory category;
    [Tooltip("アイコン")] public Texture icon;
    [Tooltip("アイコン色")] public Color iconColor = Color.white;
    [Tooltip("外見オブジェクト")]public GameObject imgObj;

    [Header("- マガジン -")]
    [Tooltip("マガジン容量\n0未満の場合無限とする")] public int capacity = 18;
    [Tooltip("タクティカルリロード可能")] public bool tacticalUse;
    [Tooltip("リロード時間(通常)")] public float nReloadTime = 2.2f;
    [Tooltip("リロード時間(タクティカル)")] public float tReloadTime = 2.0f;
    [Tooltip("リロード音最大距離")] public float reloadSoundRange;
    [Tooltip("リロード音時間")] public float reloadSoundTime;
    [Tooltip("リロードSEファイル")] public AudioClip reloadSeAudio;
    [Tooltip("リロードSE音量"), Range(0, 1)] public float reloadSeVolume = 1;
    [Tooltip("リロードSEピッチ"), Range(-3, 3)] public float reloadSePitch = 1;
    [Tooltip("切り替え待機時間")] public float changeWaitTime = 0.5f;
    [Tooltip("移動速度ペナルティ[腰撃ち](倍率)"), Range(0, 1)]
    public float hip_SPDPenalty = 1f;
    [Tooltip("移動速度ペナルティ[ADS](倍率)"), Range(0, 1)]
    public float ads_SPDPenalty = 0.8f;
    [Tooltip("回転速度/秒[腰撃ち](倍率)")]
    public float hip_SpinSpeed = 180f;
    [Tooltip("回転速度/秒[ADS](倍率)")]
    public float ads_SpinSpeed = 90f;
    [Tooltip("AI適正射程")] public float ai_Range;


    public GunShots shots;
    public GunBullets bullets;
    public GunDamages damages;
    [Tooltip("ダメージスコア倍率")] public float damageScoreMult = 1;
    [Tooltip("キルスコア倍率")] public float killScoreMult = 1;
    [Tooltip("バフ付与")] public BufAdd[] bufAdds;
    [Tooltip("ノックバック強さ")] public float kbPow;
    [Tooltip("ノックバック中央")] public bool kbCenter;

    public string InfosGet(bool melee)
    {
        var infostr = LocalizSystem.LocailzString(!melee ? "GunInfo" : "MeleeInfo", name, false, info);
        if (infostr != "") infostr += "\n";
        infostr += LocalizSystem.LocailzSCInfo("カテゴリー") + category;
        infostr += $"\n{LocalizSystem.LocailzSCInfo("移動速度")}({LocalizSystem.LocailzSCInfo("通常")}{hip_SPDPenalty * 100}/{LocalizSystem.LocailzSCInfo("ADS")}{ads_SPDPenalty * 100})";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("回転速度/秒")}({LocalizSystem.LocailzSCInfo("通常")}{hip_SpinSpeed}/{LocalizSystem.LocailzSCInfo("ADS")}{ads_SpinSpeed})";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("切り替え時間")}{changeWaitTime}{LocalizSystem.LocailzSCInfo("秒")}";
        if (capacity >= 0)
        {
            infostr += $"\n{LocalizSystem.LocailzSCInfo("弾数")}{capacity}";
            infostr += $"\n{LocalizSystem.LocailzSCInfo("リロード")}{nReloadTime}{LocalizSystem.LocailzSCInfo("秒")}";
            if (tacticalUse) infostr += $"<color=#BBBB00>({tReloadTime}{LocalizSystem.LocailzSCInfo("秒")})</color>";
        }
        infostr += $"\n{LocalizSystem.LocailzSCInfo("RPS")}{shots.RPS}({1 / shots.RPS:F2}{LocalizSystem.LocailzSCInfo("秒")})";
        infostr += "\n" + InfoDams();
        infostr += "\n" + InfoSound();
        if (bufAdds.Length > 0) infostr += "\n" + Data_Base.BufInfoStr(bufAdds);
        if (bullets.remAddShot != null)
        {
            infostr += $"\n<size=70%>{LocalizSystem.LocailzSCInfo("追加")}\n{bullets.remAddShot.InfoDams()}";
            infostr += "\n" + bullets.remAddShot.InfoSound();
            if (bullets.remAddShot.bufAdds.Length > 0) infostr += "\n" + Data_Base.BufInfoStr(bullets.remAddShot.bufAdds);
            infostr += "</size>";
        }
        if (damageScoreMult != 1) infostr += $"\n{LocalizSystem.LocailzSCInfo("ダメージスコア")}{damageScoreMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (killScoreMult != 1) infostr += $"\n{LocalizSystem.LocailzSCInfo("キルスコア")}{killScoreMult}{ LocalizSystem.LocailzSCInfo("倍")}";
        return infostr;
    }
    public string InfoDams()
    {
        var infostr = "";
        if (shots.pallet > 1) infostr += $"{LocalizSystem.LocailzSCInfo("発射数")}{shots.pallet}\n";
        infostr += $"{LocalizSystem.LocailzSCInfo("弾速")}{shots.speed}m/s";
        var sd = shots.stopDiffusion;
        var md = shots.moveDiffusion;
        var randMax = Mathf.Max(sd.ads_Diffusion,sd.hip_Diffusion,md.ads_Diffusion,md.hip_Diffusion);
        var randMin = Mathf.Min(sd.ads_Diffusion, sd.hip_Diffusion, md.ads_Diffusion, md.hip_Diffusion);
        if (randMin > 0 || randMax > 0)
        {
            infostr += $"\n{LocalizSystem.LocailzSCInfo("拡散角度")}:" + randMin;
            if (randMin != randMax) infostr += "～" + randMax;
        }
        infostr += $"\n{LocalizSystem.LocailzSCInfo("基礎")}{LocalizSystem.LocailzSCInfo(damages.damageBase >= 0 ? "ダメージ" : "回復")}";
        infostr += $"{MathF.Abs(damages.damageBase)}({LocalizSystem.LocailzSCInfo("HS")}{damages.headMulti}{LocalizSystem.LocailzSCInfo("倍")})";
        if (damages.injustPer > 0) infostr += $"\n{LocalizSystem.LocailzSCInfo("負傷割合")}{damages.injustPer}%";
        if (damages.distanceMode != DistanceMode.NoUse) infostr += "\n" + Data_Base.DisInfoStr(damages.disMult, damages.disMax);
        if (damages.wallMult != 1f) infostr += $"\n{LocalizSystem.LocailzSCInfo("壁倍率")}{damages.wallMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (kbPow != 0) infostr += $"\n{LocalizSystem.LocailzSCInfo("ノックバック力")}{Mathf.Abs(kbPow)}";
        return infostr;
    }
    public string InfoSound()
    {
        return $"{LocalizSystem.LocailzSCInfo("音")}{shots.soundRange}m({shots.soundTime}{LocalizSystem.LocailzSCInfo("秒")})";
    }
}
/// <summary>
/// 銃のカテゴリー分け
/// </summary>
public enum GunCategory
{
    HG,     //ハンドガン(ピストル)
    AR,     //アサルトライフル
    SMG,    //サブマシンガン
    LMG,    //ライトマシンガン
    SR,     //スナイパーライフル
    SG,     //ショットガン
    SP,      //特別枠

    Punch=1000,
    Kife,
    Sword,
    Hammer,
    Syringe,
    Cyce,
}
public enum BullectPerce
{
    No,//非貫通
    Pl,//プレイヤー貫通
    Wall,//壁貫通
}
public enum DistanceMode
{
    NoUse,
    PositionStart_Hit,
    Vect_Time,
}
[System.Serializable]
public class GunDamages
{
    [Tooltip("基礎ダメージ")] public float damageBase;
    [Tooltip("距離モード")] public DistanceMode distanceMode;
    [Tooltip("ダメージ距離変化")] public AnimationCurve disMult;
    [Tooltip("ダメージ距離最大")] public float disMax = 20;
    [Tooltip("ヘッドショットダメージ\n※0倍の場合はヘッドショットは無しになる")]
    public float headMulti = 1.25f;
    [Tooltip("負傷%")] public float injustPer = 20f;
    [Tooltip("壁倍率")] public float wallMult = 1f;
}
[System.Serializable]
public class GunShots
{
    [Tooltip("弾プレファブ")] public GameObject bulletObj;
    [Tooltip("連射速度(rps)")] public float RPS = 6.7f;
    [Tooltip("false : フルオート\ntrue:セミオート")] public bool semiAuto = false;
    [Header("弾道")]
    [Tooltip("同時弾数(パレット)")] public int pallet = 1;
    [Tooltip("弾速")] public float speed = 35f;
    [Tooltip("最低弾速%"), Range(0, 1)] public float speed_Min = 1.0f;
    [Header("拡散")]
    [Tooltip("停止時\n※移動速度0%")] public GunDiffusion stopDiffusion;
    [Tooltip("移動時\n※移動速度100%")] public GunDiffusion moveDiffusion;
    [Header("調整")]
    [Tooltip("前位置オフセット(全体)")] public float offSetFront_All;
    [Tooltip("前位置オフセット(弾数)")] public float offSetFront_Bullet;
    [Tooltip("横位置オフセット(弾数)")] public float offSetSide_Bullet;
    [Tooltip("角度オフセット(弾数)")] public float offSetRot_Bullet;
    [Tooltip("弾レティクル方向")] public bool bulletRot_Reticle;
    [Header("- 反動 -")]
    [Tooltip("反動パターン")] public List<RecoilPattern> recoil_Pattern;
    [Tooltip("追加のランダム反動\n計算式: <-値 / 2 ～ 値 / 2>のランダムな数字を加算")]
    public Vector2 recoil_Random;
    [Tooltip("反動戻時間倍率")] public float recoil_Time;
    [Tooltip("ADS時の反動軽減倍率"), Range(0, 1)]
    public float adsRecoil_Multi;
    [Header("音")]
    [Tooltip("音最大距離")] public float soundRange;
    [Tooltip("音時間")] public float soundTime;
    [Tooltip("SEファイル")] public AudioClip seAudio;
    [Tooltip("SE音量"),Range(0,1)] public float seVolue = 1;
    [Tooltip("SEピッチ"), Range(-3, 3)] public float sePitch = 1;
}
[System.Serializable]
public class GunBullets
{
    [Tooltip("弾時間")] public float time = 10;
    [Tooltip("レティクル地点消滅")] public bool targetPosRem;
    [Tooltip("命中開始時間")] public float hitStartTime;
    [Tooltip("命中終了時間")] public float hitEndTime;
    [Tooltip("多段ヒット時間")] public float hitCT;
    [Tooltip("弾HP")] public int HP;
    [Tooltip("貫通")] public BullectPerce perce = BullectPerce.No;
    [Tooltip("時止め無効")] public bool noStop;
    [Header("命中対象")]
    [Tooltip("自分自身に当たるか")] public bool self_Hit;
    [Tooltip("味方に当たるか")] public bool team_Hit;
    [Tooltip("敵に当たるか")] public bool enemy_Hit;
    [Tooltip("弾同士当たるか")] public bool bullet_Hit;
    [Tooltip("ガジェットに当たるか")] public bool gadget_Hit;
    public Data_Gun remAddShot;
}
/// <summary>
/// 銃の反動パターン
/// </summary>
[System.Serializable]
public class RecoilPattern
{
    [Tooltip("何発目から適応するか")] public int activeNum;
    [Tooltip("反動方向(上基準)")] public Vector2 pattern;
}

/// <summary>
/// 銃の拡散範囲
/// </summary>
[System.Serializable]
public class GunDiffusion
{
    [Tooltip("腰撃ち時の拡散"), Range(0, 360)] public float hip_Diffusion;
    [Tooltip("ADS時の拡散"), Range(0, 360)] public float ads_Diffusion;
}

