using Fusion;
using UnityEngine;

/* 内容
 * ・ガジェットの使用処理
 */

public class Player_GadgetUse : NetworkBehaviour
{
    [SerializeField] Player_Manager pm;

    Data_Gadget gadget;
    [Networked] GG_Base ggb { get; set; }

    void Start()
    {
        if (!Net_Connect.CanControl(Object)) return;
        gadget = Data_Base.DB.gadgets[pm.states.gadget_IndexNum];

        pm.values.now_Retention = gadget.start_Retention;
        pm.values.now_GetTime = 0f;
        pm.values.now_UseTime = 0f;

        pm.objects.throwPowNow.gameObject.SetActive(false);
        pm.objects.throwPowMax.gameObject.SetActive(false);
        pm.objects.throwPowMin.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (!pm.StopMove) return;
        pm.values.lastGGTime += Time.deltaTime;
        //死亡
        if (pm.hpTotal <= 0 && pm.PassiveLvGet(Passive.Undeath) <= 0) return;
        if (pm.BufGet(BufType.Shock)) return;
        //キャンセル処理
        if (pm.values.now_CursorState != CursorState.Gadget)
        {
            pm.values.now_UseTime = 0f;
            pm.objects.throwPowMax.gameObject.SetActive(false);
            pm.objects.throwPowNow.gameObject.SetActive(false);
            pm.objects.throwPowMin.gameObject.SetActive(false);
        }
        gadget = Data_Base.DB.gadgets[pm.states.gadget_IndexNum];
        //ガジェットの補充
        if (pm.values.now_Retention < gadget.max_Retention + pm.passc.ggStockAdd)
        {
            pm.values.now_GetTime += Time.deltaTime / pm.passc.ggCoolTime_Multi;

            if (pm.values.now_GetTime >= gadget.get_Time)
            {
                pm.values.now_Retention++;
                pm.values.now_GetTime = 0f;
            }
        }
        else if (pm.values.now_Retention <= 0) return;

        #region ガジェット使用処理
        //ガジェット使用
        if (pm.values.now_Retention > 0 && pm.values.now_CursorState != CursorState.Gadget
            && pm.controlle.gadget.trigger)
        {
            pm.values.now_CursorState = CursorState.Gadget;
            RPC_GGSet(pm.objects.handTrans.position, pm.states.gadget_IndexNum);

        }
        //ガジェット使用中
        else if (pm.values.now_CursorState == CursorState.Gadget
            && !pm.controlle.gadget.press)
        { pm.values.now_UseTime += Time.deltaTime; }
        //ガジェット使用キャンセル
        else if (pm.values.now_CursorState == CursorState.Gadget && pm.controlle.gadget.trigger && pm.values.now_throwPower == 0f)
        {
            pm.values.now_CursorState = CursorState.Shot;
        }
        if (ggb != null)
        {
            if(!ggb.active)pm.values.lastGGTime = 0;
            //親化
            if (ggb.transform.parent == null)
            {
                ggb.transform.SetParent(pm.objects.handTrans);
                ggb.transform.position = pm.objects.handTrans.position;
                ggb.transform.rotation = transform.rotation;
            }
            //ガジェットを使う
            if (pm.values.now_CursorState == CursorState.Gadget && pm.values.now_UseTime >= gadget.use_Time * pm.passc.ggUseTime_Mult)
            {
                pm.objects.throwPowMin.gameObject.SetActive(true);
                Use_Gadget();
            }
        }

        #endregion
    }

    /// <summary>
    /// ガジェットの使用
    /// </summary>
    void Use_Gadget()
    {
        Rigidbody2D rb = ggb.gameObject.GetComponent<Rigidbody2D>();

        //構えを挟む
        if (!gadget.instance)
        {
            var shot = pm.controlle.shot.press;

            //シュミレーションの変更(タイミング：「構え」)
            if (shot && gadget.simulatedType == Data_Gadget.SimulatedType.Stance) { rb.simulated = true; }

            //構え(強さ指定)
            if (shot && gadget.can_ThrowPowChange)
            {
                if (gadget.visualiDistance) { pm.objects.throwPowNow.gameObject.SetActive(true); }
                pm.values.now_throwPower = throwPowerSet();
                return;
            }
            //構え(強さ固定)
            else if (shot && !gadget.can_ThrowPowChange) { pm.values.now_throwPower = 1; return; }
            //入力待機
            else if (!shot && pm.values.now_throwPower == 0f)
            {
                pm.objects.throwPowNow.gameObject.SetActive(false);
                return;
            }
        }
        //即時使用
        else
        {
            pm.values.now_throwPower = 1;

            //シュミレーションの変更(タイミング：「構え」)
            if (gadget.simulatedType == Data_Gadget.SimulatedType.Stance) { rb.simulated = true; }
        }

        //シュミレーションの変更(タイミング：「使う」)
        if (gadget.simulatedType == Data_Gadget.SimulatedType.use) { rb.simulated = true; }

        Net_Value.SoundSet(pm.PosGet, pm, gadget.soundRange*pm.passc.atkSound_Multi, gadget.soundTime,gadget.seAudio, gadget.seVolume, gadget.sePitch);
        //ガジェット使用
        ggb.Activate(true);
        pm.objects.throwPowMax.gameObject.SetActive(false);
        pm.objects.throwPowNow.gameObject.SetActive(false);
        pm.objects.throwPowMin.gameObject.SetActive(false);

        pm.values.now_UseTime = 0f;
        pm.values.now_throwPower = 0f;
        pm.values.now_Retention--;
        pm.values.now_CursorState = CursorState.Shot;
    }

    /// <summary>
    /// ガジェットの投てき距離
    /// </summary>
    /// <returns> 0 or Min～1が帰ってくる </returns>
    float throwPowerSet()
    {
        if (gadget.visualiDistance) { pm.objects.throwPowMax.gameObject.SetActive(true); }

        Vector2 center = pm.objects.throwPowMax.transform.position;
        float distance = Vector2.Distance(center, pm.objects.targetPoint.transform.position);

        float maxRadius = gadget.throwDisMax * 0.5f * pm.passc.ggRange_Multi;
        float minRadius = gadget.throwDisMin * 0.5f;

        pm.values.max_throwRange = maxRadius;

        //パワー計算
        float pow = Mathf.Clamp01(distance / maxRadius);

        pm.objects.throwPowMax.transform.localScale = Vector3.one * maxRadius * 2;
        pm.objects.throwPowNow.transform.localScale = Vector3.one * pow * maxRadius * 2;
        pm.objects.throwPowMin.transform.localScale = Vector3.one * minRadius * 2;
        //色
        pm.objects.throwPowNow.color = UI_OptionManager.OptionGetColor("UI_Option 31", new Color(1,1,1,0.1f));
        pm.objects.throwPowMax.color = UI_OptionManager.OptionGetColor("UI_Option 32", new Color(0.9f, 0.9f, 0, 0.1f));
        pm.objects.throwPowMin.color = UI_OptionManager.OptionGetColor("UI_Option 33", new Color(0.9f, 0, 0, 0.25f));
        //Minよりも小さい場合無かったことに
        if (distance <= minRadius)
        {
            pm.objects.throwPowMax.gameObject.SetActive(false);
            pow = 0f;
        }

        return pow;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_GGSet(Vector3 pos, int gid)
    {
        var gadgets = Data_Base.DB.gadgets[gid];
        //ガジェットの生成
        Runner.Spawn
        (
        gadgets.gadgetObj,
        pos,
        transform.rotation,
        pm.Object.InputAuthority,
        onBeforeSpawned: (runner, obj) =>
        {
            Net_RigSync.NStartSet(obj, Vector3.zero);
            ggb = obj.GetComponent<GG_Base>();
            ggb.pm = pm;
            ggb.gid = gid;
        }
        );
    }
}
