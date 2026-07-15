using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SuzutenDCre/DataBase")]
public class Suzuten_DataBase : ScriptableObject
{
    #region 変数
    [Tooltip("キャラデータ")]
    public Suzuten_CharaData[] Charas;
    public Texture RandomCharaImage;
    [Tooltip("キャラステータス基準")]
    public Suzuten_CharaData CParaBase;
    [Tooltip("アイテムオブジェクト")]
    public GameObject[] ChaosItemObjs;
    public GameObject[] ItemObjs;
    [Tooltip("バフエフェクト")]
    public GameObject[] BufEffectObjs;
    [Tooltip("バフ色")]
    public Color[] BufColors;
    [Tooltip("ステージデータ")]
    public Suzuten_StageData[] StageDatas;
    public Sprite RandomStageImage;
    [Tooltip("効果音")]
    public SEPlayC NomalItemSE;
    public SEPlayC ChaosItemSE;
    [Tooltip("設定")]
    public LayerMask PlayerLayer;
    public AudioSource SEObj;



    public float RotSpeed = 0.02f;
    public float JDSPCost = 90;
    public float BoostSPCost = 6;
    public float FallHomSPCost = 2;
    public float GuardRefCost = 50f;

    public bool KeyBoardUse = true;
    #endregion
    #region クラス Enum
    public enum BufsE
    {
        ハイパーUnityちゃん=0,
        炎上=1,
        毒=2,
        過力=3,
        冷気=4,
        混乱=5,
        視界妨害=6,
        バリア=7,
        パワーアップ=8,
        スピードアップ=9,
        ハイフライ=10,
        巨大化=11,
        弾反射=12,
        はいバリアー=13,
        反撃=14,
        重力不定=15,
        混沌超弦 = 16,
        CT停滞=17,
        時間停止=18,
    }
    public enum BufOPE
    {
        付与,
        増加,
        上書き,
        解除,
        切り替え,
    }
    [System.Serializable]
    public class BufAddsC
    {
        [Tooltip("対象バフ")]
        public BufsE Buf;
        [Tooltip("バフ処理")]
        public BufOPE BufOP;
        [Tooltip("バフ時間")]
        public int BufTime;
        [Tooltip("バフ強度")]
        public int BufPower;
        [Tooltip("(増加用)最大バフ時間")]
        public int BufTimeMax;
        [Tooltip("(増加用)最大バフ強度")]
        public int BufPowerMax;
    }
    [System.Serializable]
    public class SEPlayC
    {
        [Tooltip("音")]
        public AudioClip SEFile;
        [Tooltip("複数再生仕様")]
        public SEOPE SEOP;
        [Tooltip("複数再生時最初から")]
        public bool SERePlay = false;
        [Tooltip("音量"), Range(0f, 4f)]
        public float Volume = 1;
        [Tooltip("音程"), Range(-3f, 3f)]
        public float Pitch = 1;
    }
    public enum SEOPE
    {
        単体最大音量,
        単体音量加算,
        複数再生,
        ボイス,
    }
    #endregion
    #region メソッド
    static public bool TimeChecks(int Time, Vector3Int CTimes)
    {
        return Time >= CTimes.x && Time <= CTimes.y && (Time - CTimes.x) % Mathf.Max(1, CTimes.z) == 0;
    }
    #endregion
}

