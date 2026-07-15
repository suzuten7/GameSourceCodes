using UnityEngine;
[CreateAssetMenu(menuName ="DataCre/DataBase")]
public class DataBase : ScriptableObject
{
    #region DataBase取得用
    static DataBase DBIn;
    static public DataBase DB
    {
        get
        {
            if (DBIn == null) DBIn = (DataBase)Resources.Load("DataBase");
            return DBIn;
        }
    }
    #endregion
    #region パッシブスキル
    [EnumIndex(typeof(Fugi_PassE))]
    public Data_Player_Passive[] Fugitive_Pass;
    [EnumIndex(typeof(Oni_Pilot_PassE))]
    public Data_Player_Passive[] Oni_Pilot_Pass;
    [EnumIndex(typeof(Oni_Visit_PassE))]
    public Data_Player_Passive[] Oni_Visit_Pass;
    [EnumIndex(typeof(SkillTypeE))]
    public Color[] SkillTypeColors;

    public Data_Stage[] Stages;
    #endregion
    #region Enum
    public enum SkillTypeE
    {
        体力探知,
        移動,
        時間装置,
        スキル,
        他強化,
        デバッグ,
        EX,
    }
    public enum Fugi_PassE
    {
        プランクトン増加,
        再生,
        加速,
        抵抗,
        機械高速化,
        持久,
        ステルス,
        パニック,
        低重力,
        復活,
        やくしろ,
        Debug_ジェットスピード,
        Debug_CT90短縮,
        Debug_リペアース,
        Debug_不死,
        Debug_オーバーコスト,
        プランクトンファイナル,
    }
    public enum Oni_Pilot_PassE
    {
        全力暴走,
        強噛,
        オートサーチ,
        加速,
        時間延長,
        粉砕飲み込み,
        ホエールBOT,
        視界良好,
        Debug_音速移動,
        Debug_ファーストCT,
        Debug_Uクリア,
        Debug_オーバーコスト,
        Debug_クイックスタート,
        極限暴走,
    }
    public enum Oni_Visit_PassE
    {
        座標探知,
        スーパーサーチ,
        マシン破損,
        時間延長,
        牢獄マーカー,
        麻痺の視線,
        妨害エリア出現,
        アイテムクリエイター,
        Debug_マーカーアタック,
        Debug_オーバーコスト,
        Debug_クイックスタート,
        操縦者からの離脱,
    }
    public enum AddCTsE
    {
        パニック,
        復活,
        視界開放,
        オートサーチ,
        牢獄マーカー,
        麻痺の視線,
        妨害エリア出現,
        アイテムクリエイター,
        復活待ち,
        プランクトンファイナル,
    }
    public enum DameTypeE
    {
        他,
        接触,
        遠隔,
    }
    public enum TypesE
    {
        逃げ,
        鬼操縦者,
        鬼監視者,
        他,
    }
    #endregion
}
