using UnityEngine;
using Fusion;
/* 内容
 * ・ガジェットの読み取り用
 */

public class GG_Base : NetworkBehaviour
{
    [Networked] public Player_Manager pm { get; set; }
    public bool active;
    public float now_HP = -999f;
    public Data_Gadget ggdata;
    [Networked] public int gid { get; set; }
    [Networked] public bool netactive { get; set; }
    [Networked] float nethp { get; set; }

    float bhp;

    bool del = false;
    float deltime = 0;
    Net_RigSync rigSync;
    /// <summary>
    /// アクティブ化
    /// </summary>
    virtual public void Activate(bool set_Active)
    {
        active = set_Active;
        RPC_ActiveSet();
    }
    virtual public string InfoGet()
    {
        return "";
    }
    virtual public void Start()
    {
        Obj_LocalObjects.Gadgets.Add(this);
    }
    virtual protected void Update()
    {
        Dels();
    }
    public override void Spawned()
    {
        ggdata = Data_Base.DB.gadgets[gid];
    }
    public void StopSet()
    {
        if (rigSync == null) rigSync = GetComponent<Net_RigSync>();
        if (rigSync == null) return;
        if (pm == null || active) rigSync.noStop = false;
        else rigSync.noStop = pm.StopMove;
    }
    /// <summary>
    /// ダメージ(小数点以下切り上げ)
    /// </summary>
    public void Damage(float val)
    {
        if (val <= 0) return;
        RPC_Damage(val);
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    void RPC_Damage(float val)
    {
        now_HP -= Mathf.Ceil(val);
        now_HP = Mathf.Clamp(now_HP, 0,ggdata.max_HP);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_ActiveSet()
    {
        netactive = true;
    }
    protected void Dels()
    {
        if (Object.HasStateAuthority)
        {
            if (pm == null)
            {
                deltime += Time.deltaTime;
                if (deltime >= 10) Destroy(gameObject);
            }
            else deltime = 0;
        }
        if (Obj_LocalObjects.TimeStopd) return;
        if (Net_Connect.CanControl(Object))
        {
            if (!active && pm.values.now_CursorState != CursorState.Gadget)
            {
                RPC_Delte();
                Destroy(gameObject);
            }
        }
    }
    protected void HPServe()
    {
        if (Object == null) return;
        if (Net_Connect.CanControl(Object))
        {
            if(bhp != now_HP)
            {
                bhp = now_HP;
                RPC_NetHPSet(now_HP);
            }
        }
        else
        {
            now_HP = nethp;
        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_NetHPSet(float val)
    {
        nethp = val;
    }

    protected void Delte()
    {
        Net_Value.SoundSet(transform.position, pm, ggdata.breakSoundRange, ggdata.breakSoundTime, ggdata.breakSeAudio, ggdata.breakSeVolume, ggdata.breakSePitch);
        RPC_Delte();
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    void RPC_Delte()
    {
        Destroy(gameObject);
    }

}
