using UnityEngine;
[CreateAssetMenu(fileName = "MoveTypeData", menuName = "ScriptableObjects/MoveType")]
public class Data_MoveType : ScriptableObject
{
    [Tooltip("名前")] public string name;
    [Tooltip("説明"),TextArea(1,5)] public string info;
    [Tooltip("色")] public Color col;
    [Tooltip("移動速度")] public float moveSpeed;
    [Tooltip("摩擦")] public float damp;
    [Tooltip("音距離倍率")] public float soundRange;
    [Tooltip("音時間倍率")] public float soundTime;
    [Tooltip("SEファイル")] public AudioClip seAudio;
    [Tooltip("SE音量"), Range(0, 1)] public float seVolue = 1;
    [Tooltip("SEピッチ"), Range(-3, 3)] public float sePitch = 1;
    [Tooltip("低速エリア")] public bool slowArea;
    [Tooltip("窒息")] public bool suffocation;
    [Tooltip("ダメージ/秒")] public float damage;
    [Tooltip("負傷%")] public float injuryPer;
    [Tooltip("状態付与")] public BufAdd[] bufAdds;
    [Tooltip("飛行無効")] public bool flyNo;
}
