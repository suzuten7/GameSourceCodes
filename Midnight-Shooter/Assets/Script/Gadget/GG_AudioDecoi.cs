using UnityEngine;

public class GG_AudioDecoi : GG_Thorw
{
    [SerializeField] float size;
    [SerializeField] float soundRange;
    [SerializeField] float soundTime;
    float sct = 0;
    public override string InfoGet()
    {
        var sinfo = base.InfoGet();
        sinfo += $"{LocalizSystem.LocailzSCInfo("音")}{soundRange}m{soundTime}{LocalizSystem.LocailzSCInfo("秒")}";
        return sinfo;
    }
    override protected void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object) || !active) return;
        if (Obj_LocalObjects.TimeStopd) return;

        var mtype = Data_Base.DB.moveTypeBase;
        var p = int.MinValue;
        var hits = Physics2D.OverlapCircleAll(transform.position, size, LayerMask.GetMask("Hide", "Floor"));
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            if (hit.TryGetComponent<Obj_MoveType>(out var h))
            {
                if (h.type != null &&p < h.priority)
                {
                    p = h.priority;
                    mtype = h.type;
                }
            }
        }

        sct -= Time.deltaTime;
        if (sct <= 0)
        {
            var stime = soundTime * mtype.soundTime;
            sct = stime * 0.5f;
            Net_Value.SoundSet(transform.position, pm, soundRange * mtype.soundRange, stime, mtype.seAudio,mtype.seVolue,mtype.sePitch);
        }
    }
}
