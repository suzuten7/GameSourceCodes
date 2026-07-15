using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/* 内容
 * ・ガジェットのデータ
 */

/// <summary>
/// ガジェットのデータ
/// </summary>
[CreateAssetMenu(fileName = "GadgetData", menuName = "ScriptableObjects/Gadget")]
public class Data_Gadget : ScriptableObject
{
    [Header("◆メインオプション")]
    [Header("- メイン -")]
    [Tooltip("名前")] public string name;
    [Tooltip("説明"), TextArea] public string info;
    [Tooltip("カテゴリー分け")] public GadgetCategory category;
    [Tooltip("アイコン")] public Texture icon;
    [Tooltip("アイコン色")] public Color iconColor = Color.white;
    [Tooltip("ガジェットオブジェクト")] public GameObject gadgetObj;

    [Header("- ベース -")]
    [Tooltip("体力")] public int max_HP = 5;
    [Tooltip("初期保持数")] public int start_Retention = 1;
    [Tooltip("最大保持数")] public int max_Retention = 2;
    [Tooltip("取得までにかかる時間")]
    public float get_Time = 5f;
    [Tooltip("使用するまでにかかる時間\n0だと即時")]
    public float use_Time = 0f;

    [Header("- 使用中 -")]
    [Tooltip("移動速度ペナルティ(倍率)"), Range(0, 1)]
    public float move_SPDPenalty = 0.8f;
    [Tooltip("回転速度/秒")]
    public float spinSpeed = 180f;
    [Tooltip("true:構える動作無し\nfalse:構える動作あり")]
    public bool instance = false;
    [Tooltip("true:投てきの強さ調節可能\nfalse:投てきの強さ100%固定(instanceがtrueの時はこっち)")]
    public bool can_ThrowPowChange;
    [Tooltip("最大投てき距離(m)")]
    public float throwDisMax = 1;
    [Tooltip("最低投てき距離(m)")]
    public float throwDisMin = 1;
    [Tooltip("投てき速度")]
    public float throwSpeed = 27.5f;

    [Tooltip("シュミレーションを変えるタイミング")]
    public SimulatedType simulatedType;
    [Tooltip("投てき距離の可視化(instanceがtrueの時のみ)")] public bool visualiDistance = true;

    [Header("使用音")]
    [Tooltip("使用音最大距離")] public float soundRange;
    [Tooltip("使用音時間")] public float soundTime;
    [Tooltip("使用SEファイル")] public AudioClip seAudio;
    [Tooltip("使用SE音量"), Range(0, 1)] public float seVolume = 1;
    [Tooltip("使用SEピッチ"), Range(-3, 3)] public float sePitch = 1;
    [Header("破壊音")]
    [SerializeField, Tooltip("破壊音最大距離")] public float breakSoundRange;
    [SerializeField, Tooltip("破壊音時間")] public float breakSoundTime;
    [SerializeField, Tooltip("破壊SEファイル")] public AudioClip breakSeAudio;
    [SerializeField, Tooltip("破壊SE音量"), Range(0, 1)] public float breakSeVolume = 1;
    [SerializeField, Tooltip("破壊SEピッチ"), Range(-3, 3)] public float breakSePitch = 1;
    /// <summary>
    /// カテゴリー
    /// </summary>
    [System.Serializable]
    public enum GadgetCategory
    {
        Damage,   //ダメージ
        Recovery, //回復
        Set,//設置物
        Another   //その他
    }

    /// <summary>
    /// シュミレーションを変えるタイミング
    /// </summary>
    [System.Serializable]
    public enum SimulatedType
    {
        None,
        Stance,
        use
    }

    public string InfosGet()
    {
        var infostr = LocalizSystem.LocailzString("GadgetInfo",name,false, info);
        if (infostr != "") infostr += "\n";
        infostr += $"{LocalizSystem.LocailzSCInfo("カテゴリー")}{category}";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("保有増加")}{get_Time}{LocalizSystem.LocailzSCInfo("秒")}(Max" + max_Retention + ")";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("使用待機時間")}{use_Time}{LocalizSystem.LocailzSCInfo("秒")}";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("移動速度")}{move_SPDPenalty * 100}";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("回転速度/秒")}{spinSpeed}";
        if (can_ThrowPowChange) infostr += $"\n{LocalizSystem.LocailzSCInfo("投擲距離")}{throwDisMin}～{throwDisMax}";
        infostr += $"\n{LocalizSystem.LocailzSCInfo("音")}{soundRange}m({soundTime}{LocalizSystem.LocailzSCInfo("秒")})";
        var gins = gadgetObj != null ? gadgetObj.GetComponent<GG_Base>() : null;
        if (gins != null)
        {
            var gininfo = gins.InfoGet();
            if (gininfo != "") infostr += "\n" + gininfo;
        }
        if(max_HP > 0) infostr += $"\n{LocalizSystem.LocailzSCInfo("耐久値")}{max_HP}";
        return infostr;
    }
}
