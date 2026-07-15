using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static Suzuten_DataBase;
[CreateAssetMenu(menuName ="SuzutenDCre/Action")]
public class Suzuten_ActionData : ScriptableObject
{
    [Tooltip("アクション名")]
    public string ACName;
    [TextArea,Tooltip("アクション説明")]
    public string ACInfo;
    [Tooltip("SPアクション")]
    public bool SPAC;
    [Tooltip("アニメーションID")]
    public int AnimID;
    [Header("アクション内容")]
    [Header("制限無視")]
    public StintE[] Stints;
    [Header("弾発射")]
    public ShotsC[] Shots;
    [Header("移動")]
    public MovesC[] Moves;
    [Header("自己バフ")]
    public MyBufsC[] MyBufs;
    [Header("自己数値変動")]
    public MyParametorsC[] MyParametors;
    [Header("効果音")]
    public SEsC[] SEs;
    [Header("条件アクション時間変化")]
    public IfTimeBranchC[] IfTimeBranch;
    [Header("アクション数値")]
    [Tooltip("SP条件")]
    public int SPCost;
    [Tooltip("アクション終了時間")]
    public int EndTime;
    [Tooltip("スタン耐性")]
    public int StanRegist;
    [Tooltip("アクションCT(秒)")]
    public float CT;
    [Tooltip("アクションイメージ動画")]
    public VideoClip Video;
    public enum StintE
    {
        移動可 =0,
        ジャンプ可=1,
        ダッシュ可=2,
        落下可 = 3,
        ガード可 =4,
    }
    [System.Serializable]
    public class ShotsC
    {
        [Tooltip("発射オブジェクト")]
        public GameObject ShotObj;
        [Tooltip("発射設定")]
        public Shot_FiringsC[] Firings;
        [Tooltip("ダメージ設定")]
        public Shot_DamagesC Damages;
        [Tooltip("ノックバック設定")]
        public KBsC[] KBs;
        [Tooltip("バフ")]
        public BufAddsC[] AddBufs;
        [Tooltip("使用者にも当たる")]
        public bool OwnerHits;
    }

    public enum Shot_RotBaseE
    {
        弾_敵方向,
        使用者_敵方向,
        固定,
        速度方向,
    }
    [System.Serializable]
    public class Shot_FiringsC
    {
        [Tooltip("アクション時間がx～yの間にzの間隔で発射")]
        public Vector3Int ShotTimes;
        [Tooltip("追加条件")]
        public IfsC[] Ifs;
        [Tooltip("x～yの速度")]
        public Vector2 ShotSpeed;
        [Tooltip("発射数")]
        public int ShotCount = 1;
        [Tooltip("角度基準")]
        public Shot_RotBaseE RotBase;
        [Tooltip("水平化")]
        public bool HorizontalFixed;
        [Tooltip("座標ズレ")]
        public Vector3 PosOffSet;
        [Tooltip("Yズレ")]
        public float YOffSet;
        [Tooltip("角度ズレ")]
        public Vector3 RotChange;
        [Tooltip("角度ブレ")]
        public Vector3 RotRand;
        [Tooltip("拡散角度")]
        public Vector3 RotWay;
    }
    [System.Serializable]
    public class Shot_DamagesC
    {
        [Tooltip("威力\n(攻撃力*威力=基礎ダメージ)")]
        public float Damage;
        [Tooltip("ヒット数ダメージ倍率変動\n(ダメージ/(ヒット数+1)で変化)")]
        public float HitsDam;
        [Tooltip("ヒット変動回数上限")]
        public int HitMax;
        [Tooltip("経過時間ダメージ倍率変動%\n(0より大きい場合は弾経過時間秒*X%)\n(0より小さい場合は100-弾経過時間秒*X%)\nのダメージになる")]
        public float TimeDam;
        [Tooltip("経過時間変動下限 (X%～100%)")]
        public float TimeMin;
        [Tooltip("ガード軽減率% -だと増加")]
        public float GuardReg = 70f;
        [Tooltip("HP吸収%")]
        public float HPDrain = 0f;
        [Tooltip("x=スタン値,y=スタン時間")]
        public Vector2Int StanPow = new Vector2Int(100, 50);
        [Tooltip("連続ヒット間隔,0だと一回のみ")]
        public int HitCT;
        [Tooltip("ヒットエフェクト")]
        public ParticleSystem HitEffect;
        [Tooltip("ヒットストップ(秒)")]
        public float HitStopTime;
    }
    [System.Serializable]
    public class KBsC
    {
        [Tooltip("ノックバック方向基準")]
        public KB_RotBaseE KBRotBase;
        [Tooltip("KB力(x=左右,y=上下,z=前後)")]
        public Vector3 KBPow;
        [Tooltip("ノックバック力Y")]
        public float KBPowY;
    }
    public enum KB_RotBaseE
    {
        弾向き,
        弾速度,
        使用者向き,
        使用者速度,
        敵弾中心向き,
    }

    [System.Serializable]
    public class MovesC
    {
        [Tooltip("アクション時間がx～yの間にzの間隔で速度を与える")]
        public Vector3Int MoveTimes;
        [Tooltip("追加条件")]
        public IfsC[] Ifs;
        [Tooltip("角度基準")]
        public Move_RotBaseE RotBase;
        [Tooltip("水平化")]
        public bool HorizontalFixed;
        [Tooltip("速度(x=左右,y=上下,z=前後)")]
        public Vector3 Pows;
        [Tooltip("垂直速度")]
        public float YPower;

    }
    public enum Move_RotBaseE
    {
        使用者_敵方向,
        固定,
        速度方向,
    }
    [System.Serializable]
    public class MyBufsC
    {
        [Tooltip("アクション時間がx～yの間にzの間隔でバフを与える")]
        public Vector3Int BufTimes;
        [Tooltip("追加条件")]
        public IfsC[] Ifs;
        [Tooltip("バフ")]
        public BufAddsC[] Bufs;
    }
    [System.Serializable]
    public class MyParametorsC
    {
        [Tooltip("アクション時間がx～yの間にzの間隔で数値増減")]
        public Vector3Int ParaTimes;
        [Tooltip("追加条件")]
        public IfsC[] Ifs;
        [Tooltip("対象パラメータ")]
        public ParametorE Parametor;
        [Tooltip("増減値")]
        public float Val;
    }
    public enum ParametorE
    {
        HP_SP無変=-1,
        HP,
        MP,
        SP,
        キャラスタン値,
        アクションスタン値,
        アクション1CT,
        アクション2CT,
        アクション3CT,
        アクション4CT,
        アクションSPCT,
    }
    [System.Serializable]
    public class SEsC
    {
        [Tooltip("アクション時間がx～yの間にzの間隔で効果音を再生")]
        public Vector3Int SETimes;
        [Tooltip("追加条件")]
        public IfsC[] Ifs;
        [Tooltip("効果音")]
        public SEPlayC[] SEPlays;
    }
    [System.Serializable]
    public class IfTimeBranchC
    {
        [Tooltip("アクション時間がx～yの間")]
        public Vector2Int Times;
        [Tooltip("条件")]
        public IfsC[] Ifs;
        [Tooltip("変更後アクション時間")]
        public int TimeChange;
        [Tooltip("アクションCT変動")]
        public float CTChange;
    }
    [System.Serializable]
    public class IfsC
    {
        public IfsE Ifs;
        public Vector2 Val;
    }
    public enum IfsE
    {
        ボタン未入力,
        ボタン入力時,
        ボタン入力中,
        ターゲット距離_ValX_以上,
        ターゲット距離_ValX_以下,
        ターゲット水平距離_ValX_以上,
        ターゲット水平距離_ValX_以下,
        ターゲット垂直距離_ValX_以上,
        ターゲット垂直距離_ValX_以下,
        HP割合_ValX_以上,
        HP割合_ValX_以下,
        状態ID_ValX_である場合,
        状態ID_ValX_でない場合,
        状態ID_ValX_が_ValY_段階以上,
        状態ID_ValX_が_ValY_段階未満,
    }
    static public bool IfsCheck(IfsC Ifs,Suzuten_PlayerState PS)
    {
        Vector2 V1;
        Vector2 V2;
        switch (Ifs.Ifs)
        {
            case IfsE.ボタン未入力:return !PS.Inputs.ACInput_Stay[PS.ActionID];
            case IfsE.ボタン入力中: return PS.Inputs.ACInput_Stay[PS.ActionID];
            case IfsE.ボタン入力時:return PS.Inputs.ACInput_Enter[PS.ActionID];
            case IfsE.ターゲット距離_ValX_以上: return Vector3.Distance(PS.PosGet(), PS.Target.PosGet()) >= Ifs.Val.x;
            case IfsE.ターゲット距離_ValX_以下: return  Vector3.Distance(PS.PosGet(), PS.Target.PosGet()) <= Ifs.Val.x;
            case IfsE.ターゲット水平距離_ValX_以上:
                V1 = new Vector2(PS.RigObj.position.x, PS.RigObj.position.z);
                V2 = new Vector2(PS.Target.RigObj.position.x, PS.Target.RigObj.position.z);
                return Vector2.Distance(V1,V2) >= Ifs.Val.x;
            case IfsE.ターゲット水平距離_ValX_以下:
                V1 = new Vector2(PS.RigObj.position.x, PS.RigObj.position.z);
                V2 = new Vector2(PS.Target.RigObj.position.x, PS.Target.RigObj.position.z);
                return Vector2.Distance(V1, V2) <= Ifs.Val.x;
            case IfsE.ターゲット垂直距離_ValX_以上: return Mathf.Abs(PS.PosGet().y - PS.Target.PosGet().y) >= Ifs.Val.x;
            case IfsE.ターゲット垂直距離_ValX_以下: return Mathf.Abs(PS.PosGet().y - PS.Target.PosGet().y) >= Ifs.Val.x;
            case IfsE.HP割合_ValX_以上: return 100f * PS.HP / Mathf.Max(1, PS.CD.MHP) >= Ifs.Val.x;
            case IfsE.HP割合_ValX_以下: return 100f * PS.HP / Mathf.Max(1, PS.CD.MHP) <= Ifs.Val.x;
            case IfsE.状態ID_ValX_である場合: return PS.Bufs.ContainsKey(Mathf.RoundToInt(Ifs.Val.x));
            case IfsE.状態ID_ValX_でない場合: return !PS.Bufs.ContainsKey(Mathf.RoundToInt(Ifs.Val.x));

            case IfsE.状態ID_ValX_が_ValY_段階以上:
                if(PS.Bufs.TryGetValue(Mathf.RoundToInt(Ifs.Val.x),out var bufs_up))return bufs_up.BufPower >= Ifs.Val.y;
                else return false;
            case IfsE.状態ID_ValX_が_ValY_段階未満:
                if (PS.Bufs.TryGetValue(Mathf.RoundToInt(Ifs.Val.x), out var bufs_down)) return bufs_down.BufPower < Ifs.Val.y;
                else return true;
        }
        return false;
    } 
}
