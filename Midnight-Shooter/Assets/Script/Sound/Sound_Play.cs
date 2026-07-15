using System.Collections.Generic;
using UnityEngine;

/* 内容
 * ・SE/BGMのテストボタン用
 */

#region 変数倉庫1
/// <summary>
/// 音のセット
/// </summary>
[System.Serializable]
public class Set_Sound
{
    [Tooltip("流す音")] public AudioClip set_Sound;

    [Tooltip("音量"), Range(0, 1)] public float volume = 1f;
    [Tooltip("ピッチ"), Range(-3, 3)] public float pitch = 1f;
}
#endregion

public class Sound_Play : MonoBehaviour
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("流す音")] List<Set_Sound> set_Sound;
    #endregion
    #endregion

    /// <summary>
    /// 指定のSystemSEを流す
    /// </summary>
    /// <param name="index"> 流したいSystemSEの配列番号 </param>
    public void SE_SystemPlay(int index)
    {
        Library_Sound.Sound_SystemSEPlay(set_Sound[index].set_Sound,
            set_Sound[index].volume, set_Sound[index].pitch);
    }

    /// <summary>
    /// 指定のGameSEを流す
    /// </summary>
    /// <param name="index"> 流したいGameSEの配列番号 </param>
    public void SE_GameSEPlay(int index)
    {
        Library_Sound.Sound_GameSEPlay(set_Sound[index].set_Sound,
            set_Sound[index].volume, set_Sound[index].pitch);
    }

    /// <summary>
    /// 指定のBGMを流す
    /// </summary>
    /// <param name="index"> 流したいBGMの配列番号 </param>
    public void BGM_Play(int index)
    {
        //Library_Sound.Sound_BGMPlay(set_Sound[index].set_Sound,
        //set_Sound[index].volume, set_Sound[index].pitch);
    }
}
