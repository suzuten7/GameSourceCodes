using UnityEngine;

public class Obj_Flow : Obj_MoveType
{
    [SerializeField] bool flowCenter;
    [SerializeField] Vector2 flowPow;
    public void Flows(Rigidbody2D rig)
    {
        if (rig == null) return;
        float frot;
        if (!flowCenter) frot = transform.eulerAngles.z;
        else
        {
            var vc = transform.position - rig.transform.position;
            frot = Mathf.Atan2(vc.y, vc.x) * Mathf.Rad2Deg;
        }
        rig.AddForce(Quaternion.Euler(0, 0, frot) * flowPow);
    }
}
