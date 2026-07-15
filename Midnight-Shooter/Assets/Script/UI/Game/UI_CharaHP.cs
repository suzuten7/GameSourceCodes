using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharaHP : MonoBehaviour
{
    [SerializeField] Player_Manager pm;
    [SerializeField] TextMeshProUGUI nameTx;
    [SerializeField] TextMeshProUGUI textHP;
    [SerializeField, Tooltip("通常HP時の文字色")] Color hp_TextCol;
    [SerializeField] Image injury_Bar;
    [SerializeField] Image overHP_Bar;
    [SerializeField] Image nowHP_Bar;

    void LateUpdate()
    {
        Vector3 pos = pm.PosGet;
        transform.position = pos;
        transform.rotation = Quaternion.identity;

        nameTx.text = pm.states.name;
        nameTx.color = Data_Base.TeamColorGet(pm.states.teamID);
        injury_Bar.fillAmount = pm.values.hpInjury / pm.states.max_HP;
        overHP_Bar.fillAmount = pm.hpTotal / pm.states.max_HP;
        nowHP_Bar.fillAmount = pm.values.hpNow / pm.states.max_HP;

        string colorCode = ColorUtility.ToHtmlStringRGB(hp_TextCol);

        textHP.text = $"{Library_UI.FormatNum((int)pm.hpTotal, colorCode)} / " +
            $"{Library_UI.FormatNum((int)pm.values.hpInjury, colorCode)}";
    }
}
