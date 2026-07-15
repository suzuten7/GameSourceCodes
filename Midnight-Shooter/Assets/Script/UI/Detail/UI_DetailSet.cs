using UnityEngine;

#region 現在のバージョン：V1.0
/* 内容
 * ・詳細ウィンドウの表示ターゲットと文字変更
 * ・このスクリプトがオブジェクトに無いとDetails_UIが検知しない
 */
#endregion

public class UI_DetailSet : MonoBehaviour
{
    #region 変数倉庫
    #region サブオプション
    [Header("◆サブオプション")]
    [Tooltip("詳細説明(タイトル)" +
        "\n※Details_UI.DetailsSelectがDefault以外だと" +
        "\n　自動で別のテキストに変更される"), TextArea(1, 1)]
    public string derailTitle;
    [Tooltip("詳細説明(メイン)" +
        "\n※Details_UI.DetailsSelectがDefault以外だと" +
        "\n　自動で別のテキストに変更される"), TextArea(1, 4)]
    public string derailMain;
    [Tooltip("場所の追加移動")]
    public Vector2 shiftAdd_Pos = Vector2.zero;


    [HideInInspector] public int achv_ID = -999; //この実績のID(自動更新)
    #endregion
    #endregion
}
