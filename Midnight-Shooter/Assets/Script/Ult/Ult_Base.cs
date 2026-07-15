using Fusion;
using UnityEngine;

public class Ult_Base : NetworkBehaviour
{
    [Networked]public Player_Manager pm { get; set; }
    [Networked] public int ultid { get; set; }
    [SerializeField] float limitTime;
    protected Data_Ult ultd;
    protected float time = 0;
    float delt = 0;
    bool del = false;

    Net_RigSync rigSync;
    Ult_TimeStop ts;
    virtual public void Start()
    {
        Obj_LocalObjects.Ults.Add(this);
        rigSync = GetComponent<Net_RigSync>();
        ts = GetComponent<Ult_TimeStop>();
    }
    virtual public void Update()
    {
        ParentSet();
        TimeDel();
    }
    virtual public string InfoGet()
    {
        return "";
    }
    public void TimeDel()
    {
        if (Object.HasStateAuthority)
        {
            if (pm == null)
            {
                delt += Time.deltaTime;
                if (delt >= 10) Destroy(gameObject);
            }
            else delt = 0;
        }
        if (Obj_LocalObjects.TimeStopd && ts == null) return;
        if (Net_Connect.CanControl(Object))
        {
            time += Time.deltaTime;
            if (time >= limitTime) Delte();
        }
    }
    public void ParentSet()
    {
        if (Net_Connect.CanControl(Object))
        {
            if (ultd.parentSet)
            {
                if(transform.parent != pm.objects.handTrans)
                {
                    transform.parent = pm.objects.handTrans;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
            else if(transform.parent == null) Library_ObjParentSet.ParentSet(gameObject, "Ult's");
        }
        else if (transform.parent == null)
        {
            Library_ObjParentSet.ParentSet(gameObject, "Ult's");
        }
        if (rigSync != null && pm != null)
        {
            rigSync.noStop = pm.StopMove;
        }
    }
    public override void Spawned()
    {
        ultd = Data_Base.DB.ults[ultid];
    }
    protected void Delte()
    {
        if (del) return;
        del = true;
        RPC_Delte();
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Delte()
    {
        Destroy(gameObject);
    }
}
