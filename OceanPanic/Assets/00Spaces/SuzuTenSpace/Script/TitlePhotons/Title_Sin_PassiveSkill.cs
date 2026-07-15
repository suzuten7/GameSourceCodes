using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class Title_Sin_PassiveSkill : MonoBehaviour
{
    #region 変数
    [SerializeField] Title_SkillSets SkillSetUI;
    public int SkillID;
    public Image BackIm;
    public Image NameBackIm;
    public TextMeshProUGUI SkillNatx;
    public TextMeshProUGUI SkillInfotx;
    public List<Title_Sin_PassLVs> PassLVUIs;
    #endregion
    public void LVChanges(int LV)
    {
        SkillSetUI.Bu_PassSetCh(SkillID,LV);
    }
}
