using UnityEngine;

/* 内容
 * ・音関連の再生用
 */
/* メモ
 * ・S_Option 01：マスター音量
 * ・S_Option 02：BGM音量
 * ・S_Option 03：SE音量(システム)
 * ・S_Option 05：SE音量(ゲーム内)
 */

public class Library_Sound : MonoBehaviour
{
    #region 変数倉庫
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("SE")] AudioSource se_Source;
    float bgm_Vol = 1f;
    static public float bgm_VolMulti = 1f;
    static public float bgm_Pitch = 1f;
    #endregion
    #region その他
    static public Library_Sound library_sound;
    #endregion
    #endregion

    void Awake()
    {
        if (library_sound != null)
        {
            Destroy(gameObject);
            return;
        }
        library_sound = this;
        DontDestroyOnLoad(gameObject);
    }

    //void Update()
    //{        //(時間がなかったので簡易的に製作)
    //    //一部の値の更新がかかった時
    //    float volume = UI_OptionManager.OptionGetFloat("S_Option 01", 0.8f)
    //        * UI_OptionManager.OptionGetFloat("S_Option 02", 0.9f) * bgm_Vol * bgm_VolMulti;
    //    bgm_Source.volume = volume;
    //    bgm_Source.pitch = bgm_Pitch;
    //}

    /// <summary>
    /// 音(SystemSE)の再生
    /// </summary>
    /// <param name="set_SESound"> 流す音(SystemSE) </param>
    static public void Sound_SystemSEPlay(AudioClip set_SESound, float vol = 1f,
        float pitch = 1f)
    {
        if (set_SESound == null) return;

        //音ゼロ対策(初期リセット)
        if (vol <= 0) { vol = 1f; }

        //ピッチゼロ対策(初期リセット)
        if (pitch <= 0) { pitch = 1f; }

        var seObj = Instantiate(library_sound.se_Source,library_sound.transform);
        seObj.clip = set_SESound;
        seObj.volume = vol;
        seObj.pitch = pitch;
        seObj.loop = false;
        seObj.Play();
        Destroy(seObj.gameObject, set_SESound.length / pitch);
    }

    /// <summary>
    /// 音(GameSE)の再生
    /// </summary>
    /// <param name="set_SESound"> 流す音(GameSE) </param>
    static public void Sound_GameSEPlay(AudioClip set_SESound, float vol = 1f,
        float pitch = 1f)
    {
        if (set_SESound == null) return;

        //音ゼロ対策(初期リセット)
        if (vol <= 0) { vol = 1f; }

        //ピッチゼロ対策(初期リセット)
        if (pitch <= 0) { pitch = 1f; }

        float volume = UI_OptionManager.OptionGetFloat("S_Option 01", 0.8f)
            * UI_OptionManager.OptionGetFloat("S_Option 05", 0.9f) * vol;

        var seObj = new GameObject("GameSE");
        var audio = seObj.AddComponent<AudioSource>();
        audio.clip = set_SESound;
        audio.volume = volume;
        audio.pitch = pitch;
        audio.loop = false;
        audio.Play();
        Destroy(seObj, set_SESound.length / pitch);
    }

    /// <summary>
    /// 音(BGM)の再生
    /// </summary>
    /// <param name="set_BGMSound"> 流す音(BGM) </param>
    //static public void Sound_BGMPlay(AudioClip set_BGMSound, float vol = 1f,
    //    float pitch = 1f)
    //{
    //    if (set_BGMSound == null) return;

    //    //音ゼロ対策(初期リセット)
    //    if (vol <= 0) { vol = 1f; }

    //    //ピッチゼロ対策(初期リセット)
    //    if (pitch <= 0) { pitch = 1f; }

    //    library_sound.bgm_Vol = vol;
    //    bgm_Pitch = pitch;
    //    library_sound.bgm_Source.pitch = pitch;
    //    library_sound.bgm_Source.clip = set_BGMSound;
    //    library_sound.bgm_Source.Play();
    //}
}
