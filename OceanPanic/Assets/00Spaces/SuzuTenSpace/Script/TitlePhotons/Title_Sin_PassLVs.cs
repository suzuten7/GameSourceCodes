using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Title_Sin_PassLVs : MonoBehaviour
{
    public Title_Sin_PassiveSkill PassSkillUI;
    public int LV;
    public Image BackImage;
    public TextMeshProUGUI CostTx;

    public void LVSelect()
    {
        PassSkillUI.LVChanges(LV);
    }
}
