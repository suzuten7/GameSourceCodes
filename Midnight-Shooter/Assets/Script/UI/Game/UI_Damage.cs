using TMPro;
using UnityEngine;

/* 内容
 * ・被ダメージまたは与ダメージのUI
 */

public class UI_Damage : MonoBehaviour
{
    [SerializeField, Tooltip("ダメージ用のテキスト")]
    TextMeshProUGUI text;
    [SerializeField] float speed;
    [SerializeField] float time;
    [SerializeField] float stackTime;
    float t;
    float dam;
    bool head;
    bool kill;
    public bool Check(float damage,bool headShot, bool killFlag)
    {
        var check = false;
        if (dam > 0 && damage > 0) check = true;
        if (dam < 0 && damage < 0) check = true;
        if (dam == 0 && damage == 0) check = true;
        return t <= stackTime && headShot == head && killFlag == kill;
    }
    /// <summary>
    /// 文字の色出力
    /// </summary>
    /// <param name="damage"> 被ダメージまたは与ダメージ </param>
    /// <param name="headShot_Flag"> ヘッドショット判定 </param>
    /// <param name="kill_Flag"> キル判定 </param>
    public void SetDamage(float damage, bool headShot_Flag, bool kill_Flag)
    {
        dam += damage;
        head = headShot_Flag;
        kill = kill_Flag;

        string back_Text = "";
        string colorCode = "FFFFFF";

        //ダメージ
        if (damage > 0)
        {
            //ヘッドショット時は最後に「!」を付ける
            back_Text = headShot_Flag ? "!" : "";

            if (kill_Flag) colorCode = UI_OptionManager.OptionGetString("UI_Option 15", "FFFF00");
            else if (headShot_Flag) colorCode = UI_OptionManager.OptionGetString("UI_Option 14", "FFFF00");
            else colorCode = UI_OptionManager.OptionGetString("UI_Option 13", "FFFFFF");
        }
        //回復
        else if (damage < 0) { colorCode = UI_OptionManager.OptionGetString("UI_Option 16", "00FF00"); }
        else colorCode = "888888";
        text.text = $"<color=#{colorCode}>{Mathf.Abs(dam).ToString("F0") + back_Text}</color>";
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
        t += Time.deltaTime;
        if (t >= time) Destroy(gameObject);
    }
}
