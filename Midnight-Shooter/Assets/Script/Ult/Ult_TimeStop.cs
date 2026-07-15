using UnityEngine;

public class Ult_TimeStop : Ult_Base
{
    public override void Start()
    {
        base.Start();
        Obj_LocalObjects.TimeStops.Add(this);
    }
}
