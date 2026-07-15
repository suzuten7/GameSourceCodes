using UnityEngine;

public class Ult_Buf : Ult_Base
{
    [SerializeField, Tooltip("バフ付与")] BufAdd[] bufAdds;
    bool used = false;
    public override string InfoGet()
    {
        return Data_Base.BufInfoStr(bufAdds);
    }
    public override void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object) || pm == null) return;
        if (!used)
        {
            used = true;
            pm.BufChanges(bufAdds);
        }
    }
}
