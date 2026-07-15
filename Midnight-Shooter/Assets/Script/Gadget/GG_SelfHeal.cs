using UnityEngine;

public class GG_SelfHeal : GG_Base
{
    [SerializeField] float HPAdd;
    [SerializeField] float InjustAdd;
    [SerializeField] float ShildAdd;
    public override string InfoGet()
    {
        var istr = "";
        if(HPAdd != 0)istr += $"{LocalizSystem.LocailzSCInfo("HP回復")}{HPAdd}";
        if (InjustAdd != 0)
        {
            if (istr != "") istr += "\n";
            istr += $"{LocalizSystem.LocailzSCInfo("負傷回復")}{InjustAdd}";
        }
        if (ShildAdd != 0)
        {
            if (istr != "") istr += "\n";
            istr += $"{LocalizSystem.LocailzSCInfo("シールド")}{ShildAdd}";
        }
        return istr;
    }
    public override void Activate(bool set_Active)
    {
        pm.values.hpInjury = Mathf.Min(pm.values.hpInjury + InjustAdd,pm.states.max_HP);
        if(HPAdd!=0)pm.Damage(-HPAdd, 0, false, pm, gid, 1, 1);
        pm.values.hpShild = Mathf.Max(pm.values.hpShild, ShildAdd);
        Delte();
    }

}
