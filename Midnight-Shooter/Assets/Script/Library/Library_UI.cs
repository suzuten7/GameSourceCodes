using UnityEngine;

/* 内容
 * ・UI全体の使うメソッド置き場
 */

public static class Library_UI
{
    /// <summary>
    /// 使われていない「0」の色を薄くする
    /// ・整数部分：先頭の不要な0を薄くする
    /// ・小数部分：末尾の不要な0を薄くする
    /// ※整数部が全て0の場合は最後の1桁だけ通常色にする
    /// </summary>
    /// <param name="value"> 表示する値 </param>
    /// <param name="high_DigitCount"> 整数部分の桁数(「3」だとD3(001)) </param>
    /// <param name="low_DigitCount"> 小数点以下の桁数(「3」だとF3(0.001)) </param>
    /// <param name="colorCodeRGB"> 今回使う文字色 </param>
    /// <returns> 色を変えた文字を返す </returns>
    public static string FormatNum
        (float value, string colorCodeRGB = "FFFFFF", int high_DigitCount = 3, int low_DigitCount = 0)
    {
        //色変換
        int alpha = Mathf.RoundToInt(0.25f * 255f);
        string colorCodeA = alpha.ToString("X2");

        #region 整数部分の処理
        string raw = value.ToString($"F{low_DigitCount}");
        string[] split = raw.Split('.');

        string high_Str = Mathf.Abs(int.Parse(split[0])).ToString($"D{high_DigitCount}");
        int high_ZeroCount = 0;

        for (int i = 0; i < high_Str.Length; i++)
        {
            if (high_Str[i] == '0')
                high_ZeroCount++;
            else
                break;
        }

        //全部0なら最後だけ残す
        if (high_ZeroCount == high_Str.Length) high_ZeroCount--;

        string high_Left =
            high_Str.Substring(0, high_ZeroCount);

        string high_Right =
            high_Str.Substring(high_ZeroCount);
        #endregion
        #region 小数点以下の変換
        string low_Left = "";
        string low_Right = "";

        if (low_DigitCount > 0)
        {
            string low_Str = split.Length > 1 ? split[1] : "";

            int low_ZeroCount = 0;

            //小数点より後ろの0を数える
            for (int i = low_Str.Length - 1; i >= 0; i--)
            {
                if (low_Str[i] == '0') low_ZeroCount++;
                else break;
            }

            int visibleLength = low_Str.Length - low_ZeroCount;
            visibleLength = Mathf.Max(0, visibleLength);

            low_Right = low_Str.Substring(visibleLength);
            low_Left = low_Str.Substring(0, visibleLength);
        }

        //小数点以下が全部0なら小数点も薄くする
        string dotColor = string.IsNullOrEmpty(low_Left)
            ? $"{colorCodeRGB}{colorCodeA}" : colorCodeRGB;
        #endregion

        #region 文字変換
        string result =
            $"<color=#{colorCodeRGB}{colorCodeA}>{high_Left}</color>" +
            $"<color=#{colorCodeRGB}>{high_Right}</color>";

        //小数点以下の処理
        if (low_DigitCount > 0)
        {
            result += $"<color=#{dotColor}>.</color>" +
                $"<color=#{colorCodeRGB}>{low_Left}</color>" +
                $"<color=#{colorCodeRGB}{colorCodeA}>{low_Right}</color>";
        }
        return result;
        #endregion
    }
}
