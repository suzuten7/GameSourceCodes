using UnityEngine;

/* 内容
 * ・プレイヤーの移動
 * ・プレイヤーの回転
 */

public class Player_Move : MonoBehaviour
{
    [SerializeField]Player_Manager pm;
    [SerializeField] float moveCheckScale;
    float soundCT = 0;
    float mtypeCT = 0;
    //プレイヤーの移動
    void Update()
    {
        if (!Net_Connect.CanControl(pm.Object)) return;
        if (!pm.StopMove) return;
        //移動タイプ
        pm.values.moveType = pm.PassiveLvGet(Passive.Fly) <= 0 ? Data_Base.DB.moveTypeBase : Data_Base.DB.moveTypeFly;
        Obj_MoveType mtobj = null;
        var p = int.MinValue;
        var hits = Physics2D.OverlapCircleAll(pm.PosGet, pm.passc.charaScale_Multi * moveCheckScale, LayerMask.GetMask("Hide", "Floor"));
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            if (hit.TryGetComponent<Obj_MoveType>(out var mto))
            {
                if (mto.type == null) continue;
                if (p >= mto.priority) continue;
                p = mto.priority;
                if (pm.PassiveLvGet(Passive.Fly) <= 0 || mto.type.flyNo)
                {
                    pm.values.moveType = mto.type;
                    mtobj = mto;
                }
                else pm.values.moveType = Data_Base.DB.moveTypeFly;
            }
        }
        if (mtobj != null && mtobj.TryGetComponent<Obj_Flow>(out var flow)) flow.Flows(pm.objects.rb);
        var mtype = pm.values.moveType;
        mtypeCT -= Time.deltaTime;
        if(mtypeCT <= 0)
        {
            var check = false;
            if(mtype.damage != 0)
            {
                check = true;
                var pmuse = pm;
                var damID = -10001;
                if(mtobj != null)
                {
                    if(mtobj.gg != null && mtobj.gg.pm != null)
                    {
                        pmuse = mtobj.gg.pm;
                        damID = mtobj.gg.gid + (int)AtkID.Gadget;
                    }
                }
                var dam = mtype.damage;
                if (dam > 0) dam *= pm.passc.damageArea_Regist;
                pm.Damage(dam, mtype.injuryPer, false,pmuse,damID, 0, 0);
            }
            if(mtype.bufAdds != null && mtype.bufAdds.Length > 0)
            {
                check = true;
                pm.BufChanges(mtype.bufAdds);
            }
            if (check) mtypeCT = 1f;
        }

        pm.objects.rb.linearDamping = mtype.damp;
        //死亡
        if (pm.hpTotal <= 0 && pm.PassiveLvGet(Passive.Undeath) <= 0) return;
        Vector2 mv = pm.controlle.move;
        if (mv.magnitude > 1) mv = mv.normalized;
        MoveStat(mv);
        var mvd = pm.MoveStateGet;
        float spdMulti = mvd.spd_Multi;
        var spinSpd = 180f;
        if (pm.hpTotal <= 0) spdMulti = pm.PassiveValGet(Passive.Undeath,1);
        //武器毎のペナルティ
        switch (pm.values.now_CursorState)
        {
            default:
                var gund = Data_Base.DB.guns[pm.states.gun_IndexNum];
                spdMulti *= pm.controlle.ads.press ? gund.ads_SPDPenalty: gund.hip_SPDPenalty;
                spinSpd = pm.controlle.ads.press ? gund.ads_SpinSpeed : gund.hip_SpinSpeed;
                break;
            case CursorState.Melee:
                var melled = Data_Base.DB.melles[pm.states.melee_IndexNum];
                spdMulti *= pm.controlle.ads.press ? melled.ads_SPDPenalty : melled.hip_SPDPenalty;
                spinSpd = pm.controlle.ads.press ? melled.ads_SpinSpeed : melled.hip_SpinSpeed;
                break;
            case CursorState.Gadget:
                var gadgetd = Data_Base.DB.gadgets[pm.states.gadget_IndexNum];
                spdMulti *= gadgetd.move_SPDPenalty;
                spinSpd = gadgetd.spinSpeed;
                break;
        }

        spdMulti *= pm.passc.moveSpeed_Multi;

        if(mtype.slowArea)
        {
            if (pm.passc.slowArea_Regist <= 1) spdMulti *= 1f - (1f - mtype.moveSpeed) * pm.passc.slowArea_Regist;
            else spdMulti *= Mathf.Pow(mtype.moveSpeed,pm.passc.slowArea_Regist);
            spdMulti *= pm.PassiveValGet(Passive.BadRoadOwner, 1);
        }
        else spdMulti *= mtype.moveSpeed;

        pm.objects.rb.AddForce(mv * pm.states.base_SPD * spdMulti * Time.deltaTime);

        //向きの回転
        Vector2 dir = pm.objects.recoilPoint.position - transform.position;
        var rd = Vector2.SignedAngle(transform.up, dir);
        var rot = transform.eulerAngles;
        rot.z += (rd > 0 ? 1f : -1f) * Mathf.Min(Mathf.Abs(rd),spinSpd * pm.passc.spinSpeed_Multi * Time.deltaTime);
        transform.eulerAngles = rot;


        //移動音
        soundCT -= Time.deltaTime;
        if (soundCT <= 0)
        {
            if (mvd.move_soundRange > 0)
            {
                var stime = mvd.move_soundTime * mtype.soundTime;
                Net_Value.SoundSet(pm.PosGet, pm, mvd.move_soundRange * mtype.soundRange*pm.passc.moveSound_Multi,stime,
                    mtype.seAudio,mvd.seVolue * mtype.seVolue,mvd.sePitch * mtype.sePitch);
                soundCT = stime*0.5f;
            }
        }

        if (pm.BufGet(BufType.Gravitation))
        {
            var ndis = float.MaxValue;
            Player_Manager npl = null; ;
            for(int i = 0; i < Obj_LocalObjects.Players.Count; i++)
            {
                var pl = Obj_LocalObjects.Players[i];
                if (pl == null) continue;
                if (pl.states.teamID == pm.states.teamID) continue;
                var dis = Vector2.Distance(pl.PosGet, pm.PosGet);
                if(ndis > dis)
                {
                    ndis = dis;
                    npl = pl;
                }
            }
            if(npl != null)
            {
                var vect = npl.PosGet - pm.PosGet;
                pm.objects.rb.AddForce(vect.normalized * Data_Base.BufDGet(BufType.Gravitation).values[0] * Time.deltaTime);
            }
        }
    }

    //カメラの追従
    void LateUpdate()
    {
        if (!Net_Connect.CanControl(pm.Object)) return;
        Vector3 pos = transform.position;
        pos.z = -15;
        pm.objects.camera.transform.position = pos;
    }

    /// <summary>
    /// MoveStateの変更
    /// </summary>
    void MoveStat(Vector2 mv)
    {
        //停止
        if (mv.magnitude < 0.01f)
        { pm.values.now_MoveState = MoveState.Stop; }
        else
        {
            //走る
            if (pm.controlle.dash.press)
            { pm.values.now_MoveState = MoveState.Dash; }
            //歩く
            else if (pm.controlle.walk.press)
            { pm.values.now_MoveState = MoveState.Walk; }
            //移動
            else
            { pm.values.now_MoveState = MoveState.Move; }
        }
    }
}
