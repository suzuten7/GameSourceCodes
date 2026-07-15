using UnityEngine;

public class GG_AnchorMove : GG_Thorw
{
    [SerializeField] float movePow;
    [SerializeField] float waitTimes;
    [SerializeField] float moveTimes;
    [SerializeField] float endRange;
    float anctime;

    protected override void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object) || !active) return;
        if (Obj_LocalObjects.TimeStopd) return;
        if (rb.linearVelocity.magnitude <= 0.05f)
        {
            anctime += Time.deltaTime;
            if (anctime >= waitTimes)
            {
                var vect = transform.position - pm.PosGet;
                pm.objects.rb.AddForce(vect.normalized * movePow *Time.deltaTime);
                if (vect.magnitude <= endRange || anctime >= moveTimes) Delte();
            }
        }
        else anctime = 0;
    }
}
