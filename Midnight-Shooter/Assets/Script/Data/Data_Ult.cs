using UnityEngine;
[CreateAssetMenu(fileName = "SPData", menuName = "ScriptableObjects/SP")]
public class Data_Ult : ScriptableObject
{
    [Tooltip("名前")] public string name;
    [Tooltip("説明"), TextArea] public string info;
    [Tooltip("アイコン")] public Texture icon;
    [Tooltip("アイコン色")] public Color iconColor = Color.white;
    public UltCategory[] category;
    [Tooltip("チャージ要求値")] public float chargeValue;
    [Tooltip("必殺プレファブ")] public GameObject ultObject;
    [Tooltip("生成位置レティクル化")] public bool posReticle;
    [Tooltip("親子化")] public bool parentSet;
    [Header("音")]
    [Tooltip("音最大距離")] public float soundRange;
    [Tooltip("音時間")] public float soundTime;
    [Tooltip("SEファイル")] public AudioClip seAudio;
    [Tooltip("SE音量"), Range(0, 1)] public float seVolume = 1;
    [Tooltip("SEピッチ"), Range(-3, 3)] public float sePitch = 1;
    public enum UltCategory
    {
        Heal,
        Atk,
        Buf,
        Debuf,
        Other,

        HP,
        Move,
        View,
        Shot,
        Damage,
        Gadget,
        Score,
        Sound,
    }
    public string InfoGet()
    {
        var istr = LocalizSystem.LocailzString("UltInfo",name,false,info);
        istr += $"\n{LocalizSystem.LocailzSCInfo("チャージ")}{chargeValue}";
        if(ultObject.TryGetComponent<Ult_Base>(out var ult))
        {
            istr += "\n" + ult.InfoGet();
        }
        return istr;
    }
}
