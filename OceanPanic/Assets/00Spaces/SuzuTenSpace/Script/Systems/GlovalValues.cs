using System.Collections.Generic;
using UnityEngine;

public class GlovalValues
{

    static public Dictionary<int,int> Pass_Fugi = new Dictionary<int, int>();
    static public Dictionary<int, int> Pass_OniP = new Dictionary<int, int>();
    static public Dictionary<int, int> Pass_OniV = new Dictionary<int, int>();
    /// <summary>画面設定値</summary>
    static public int[] DispOptions = new int[1];
    static public int[] DispBase = { 100 };
    /// <summary>音量設定値</summary>
    static public int[] SoundOptions = new int[3];
    static public int[] SoundBase = { 100,100,100 };
    /// <summary>設定保存</summary>
    static public void OptionSave()
    {
        for (int i = 0; i < DispOptions.Length; i++)
        {
            PlayerPrefs.SetInt("DispOptions_" + i, DispOptions[i]);
        }
        for (int i = 0; i < SoundOptions.Length; i++)
        {
            PlayerPrefs.SetInt("SoundOptions_" + i, SoundOptions[i]);
        }

    }
    /// <summary>設定読み込み</summary>
    static public void OptionLoad()
    {
        for (int i = 0; i < DispOptions.Length; i++)
        {
            DispOptions[i] = PlayerPrefs.GetInt("DispOptions_" + i, DispBase[i]);
        }
        for (int i = 0; i < SoundOptions.Length; i++)
        {
            SoundOptions[i] = PlayerPrefs.GetInt("SoundOptions_" + i, SoundBase[i]);
        }
    }

}
