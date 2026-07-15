using UnityEngine;
using static GameInfos;
public class McEvent_TimeRem : McEventBase
{
    [SerializeField] int NomalTimeRems;
    [SerializeField] string RampMessage;
    [SerializeField] int RampTimeRems;
    public override void McEvent()
    {
        if (GInfo.Ramps)
        {
            GInfo.GameTime += RampTimeRems;
            Ev_Message_Send(RampMessage, MessageE.全員);
        }
        else
        {
            GInfo.GameTime += NomalTimeRems;
            Ev_Message_Send(EventMessage, MessageE.全員);
        }
    }
}
