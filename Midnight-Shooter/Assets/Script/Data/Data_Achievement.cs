using System.Collections.Generic;
using UnityEngine;

/* 内容
 * ・実績一覧
 */

#region 変数倉庫1
[System.Serializable]
public class ACHVData
{
    [Tooltip("実績の名称"), TextArea(1, 2)]
    public string achv_Name;
    [Tooltip("実績の文字"), TextArea(1, 5)]
    public string achv_Exp;
    [Tooltip("実績条件の説明"), TextArea(1, 5)]
    public string achv_GetExp;
    [Tooltip("実績条件の説明(シークレット)"), TextArea(1, 5)]
    public string achv_SecretGetExp;

    [Tooltip("実績の画像")]
    public Sprite achvSprite;
    [Tooltip("実績の難易度")]
    public int achv_Level = 1;
    [Tooltip("シークレット判定")]
    public bool achv_SecretFlag;
    [HideInInspector] public bool achv_ClearFlag;
}

[System.Serializable]
public class ACHVStarData
{
    [Tooltip("表記のみ")]
    public string star_Name;
    [Tooltip("星の色")]
    public Color star_Color;
}
#endregion

[CreateAssetMenu(fileName = "AchievementData",
    menuName = "ScriptableObjects/Achievement")]
public class Data_Achievement : ScriptableObject
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    [Tooltip("実績一覧")]
    public List<ACHVData> achvList = new List<ACHVData>();
    [Tooltip("実績のランク")]
    public List<ACHVStarData> achv_StarData = new List<ACHVStarData>();
    #endregion
    #region サブオプション
    [Header("◆サブオプション")]
    [Tooltip("クリア時の色")]
    public Color achvClear_Color;
    [Tooltip("未クリア時の色")]
    public Color achvNotClear_Color;
    [Tooltip("シークレット未クリア時の色")]
    public Color achvSecretNoClear_Color;
    [Tooltip("総実績数のバーのグラデーション")]
    public Gradient achv_Gradient;

    [Tooltip("最大難易度")]
    public int achv_MaxLevel = 3;
    [Tooltip("難易度表記の文字(中塗り)")]
    public char achv_LevelChar = '★';
    [Tooltip("難易度表記の文字(中塗り無し)")]
    public char achv_LevelCharSpace = '☆';
    [Tooltip("後ろの画像を登場させる難易度")]
    public int spawnPattern_Level = 3;


    [Tooltip("RPS(反時計回り)\n※RPS = <回転量>/s")]
    public float rotate_Speed;
    #endregion
    #endregion
}
