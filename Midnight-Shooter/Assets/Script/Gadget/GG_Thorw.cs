
using UnityEngine;

public class GG_Thorw : GG_Base
{
    [SerializeField] float limitTime;
    protected Rigidbody2D rb;
    float time = 0;
    override public void Start()
    {
        Obj_LocalObjects.Gadgets.Add(this);
        now_HP = ggdata.max_HP;
    }
    override public void Activate(bool b)
    {
        rb = GetComponent<Rigidbody2D>();
        base.Activate(b);
        Library_ObjParentSet.ParentSet(gameObject, "Gadget's");
        rb.simulated = true;
        rb.AddForce(transform.rotation * Vector3.up * pm.values.now_throwPower
            * pm.values.max_throwRange * ggdata.throwSpeed);
    }
    override protected void Update()
    {
        StopSet();
        HPServe();
        if (!active && netactive)
        {
            active = true;
            Library_ObjParentSet.ParentSet(gameObject, "Gadget's");
            rb = GetComponent<Rigidbody2D>();
            rb.simulated = true;
        }
        if (Obj_LocalObjects.TimeStopd) return;
        if (Net_Connect.CanControl(Object) && active)
        {
            time += Time.deltaTime;
            if (time >= limitTime || (ggdata.max_HP > 0 && now_HP <= 0)) Delte();
        }
        Dels();
    }
}
