using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
using static GameInfos;
public class Player_UI_Plancton : MonoBehaviour
{
    #region 変数
    [SerializeField] Player_States PSta;
    [SerializeField] TextMeshProUGUI HPtxs;
    [SerializeField] Slider HPbar;
    [SerializeField] Image DamageUI;
    [SerializeField] GameObject DeathObj;
    #endregion
    private void Start()
    {
        #region UI初期化
        DamageUI.gameObject.SetActive(true);
        var DamColor = DamageUI.color;
        DamColor.a = 0;
        DamageUI.color = DamColor;
        #endregion
    }
    void LateUpdate()
    {
        if (!PSta.GostModes)
        {
            #region 非ゴーストモード
            HPtxs.text = PSta.HP + "/" + PSta.MHP + "億体";
            HPtxs.color = Color.white;
            HPbar.value = PSta.HP / Mathf.Max(1f, PSta.MHP);
            var DamColor = DamageUI.color;
            if (PSta.Damages > 0) DamColor.a += 0.05f;
            else DamColor.a -= 0.01f;
            DamColor.a = Mathf.Clamp01(DamColor.a);
            DamageUI.color = DamColor;
            #endregion
        }
        else
        {
            HPbar.value = 0f;
            Color DamC = DamageUI.color;
            #region 復活待ち
            if (PSta.AddCTs.TryGetValue(AddCTsE.復活, out var RevT) && RevT > 60 * 9990)
            {
                int RTimed = RevT - 60 * 9990;
                HPtxs.text = "復活まで" +(RTimed /60).ToString("D0")+"秒";
                HPtxs.color = Color.yellow;
                DamC.a = 1;
            }
            #endregion
            #region ゴーストモード
            else
            {
                HPtxs.text = "ゴーストモード";
                HPtxs.color = Color.red;
                DamC.a = 0;
            }
            DamageUI.color = DamC;
            #endregion
        }
        DeathObj.SetActive(PSta.HP <=0&&!PSta.GostModes);
    }
    public void GostModes()
    {
        PSta.GostModes = true;
    }
}
