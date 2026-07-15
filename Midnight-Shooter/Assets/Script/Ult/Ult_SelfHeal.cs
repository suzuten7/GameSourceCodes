using UnityEngine;

public class Ult_SelfHeal : Ult_Base
{
    [SerializeField] float addHP;
    [SerializeField] float addInjust;
    [SerializeField] float addShild;
    bool used = false;
    public override string InfoGet()
    {
        var istr = "";
        if (addHP != 0) istr += $"{LocalizSystem.LocailzSCInfo("HP回復")}{addHP}";
        if (addInjust != 0)
        {
            if (istr != "") istr += "\n";
            istr += $"{LocalizSystem.LocailzSCInfo("負傷回復")}{addInjust}";
        }
        if (addShild != 0)
        {
            if (istr != "") istr += "\n";
            istr += $"{LocalizSystem.LocailzSCInfo("シールド")}{addShild}";
        }
            return istr;
    }
    public override void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object)) return;
        if (!used)
        {
            used = true;
            pm.values.hpInjury += addInjust;
            if (addHP != 0) pm.Damage(-addHP, 0, false, pm, ultid, 1, 1);
            pm.values.hpShild = Mathf.Max(pm.values.hpShild, addShild);
        }
    }
}
