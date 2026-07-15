using Fusion;
using UnityEngine;

public class Player_Ult : NetworkBehaviour
{
    [SerializeField] Player_Manager pm;
    void Update()
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (pm.StopMove)
        {
            pm.values.lastUltTime += Time.deltaTime;
        }
        //死亡
        if (pm.hpTotal <= 0 && pm.PassiveLvGet(Passive.Undeath) <= 0) return;
        if (pm.BufGet(BufType.Shock)) return;
        var ultd = Data_Base.DB.ults[pm.states.ult_IndexNum];
        if (pm.StopMove)
        {
            pm.UltCharge(Data_Base.DB.ultCharge[0] * Time.deltaTime * pm.passc.ultChargeTime_Multi);
        }
        //必殺
        if (pm.values.ultCharge < ultd.chargeValue) return;
        var stopuse = false;
        if (ultd.ultObject.TryGetComponent<Ult_TimeStop>(out _)) stopuse = true;
        if (!pm.StopMove && !stopuse) return;
        if (!pm.controlle.ult.trigger) return;
        pm.values.ultCharge = 0;
        pm.values.lastUltTime = 0;
        var pos = pm.objects.handTrans.position;
        if (ultd.posReticle) pos = pm.objects.targetPoint.position;
        Net_Log.NetLog.RPC_LogAddUlt(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.UltUse, pm,pm.states.ult_IndexNum);
        RPC_UltSet(pos,pm.TransGet.eulerAngles, pm.states.ult_IndexNum);
        Net_Value.SoundSet(pos, pm, ultd.soundRange * pm.passc.atkSound_Multi, ultd.soundTime, ultd.seAudio, ultd.seVolume, ultd.sePitch);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_UltSet(Vector3 pos,Vector3 rot, int ultid)
    {
        var ults = Data_Base.DB.ults[ultid];
        //ガジェットの生成
        Runner.Spawn
        (
        ults.ultObject,
        pos,
        Quaternion.Euler(rot),
        pm.Object.InputAuthority,
        onBeforeSpawned: (runner, obj) =>
        {
            Net_RigSync.NStartSet(obj,pos,rot, Vector3.zero);
            var ultb = obj.GetComponent<Ult_Base>();
            ultb.pm = pm;
            ultb.ultid = ultid;
        }
        );
    }
}
