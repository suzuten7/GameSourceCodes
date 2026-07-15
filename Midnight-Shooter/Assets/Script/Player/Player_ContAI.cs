using System.Collections.Generic;
using UnityEngine;

public partial class Player_Controlle
{
    int ai_time_i;
    float ai_time_f;
    Vector2 ai_vect;
    Vector2 ai_vect2;
    float ai_v2pow = 0;
    Player_Manager ai_target;
    float ai_hideTime;
    bool ai_rotrev;
    float ai_rots;

    bool TimeICheck(int t,int off = 0)
    {
        return ai_time_i % t == (0 + off);
    }
    void InputCPU()
    {
        ai_v2pow = 0;
        ai_time_i++;
        ai_time_f += Time.deltaTime;
        if (Random.value <= 0.04f)
        {
            ai_time_i += Random.Range(0, 181);
            ai_time_f += Random.Range(0, 3f);
        }
        switch (pm.states.cpuMode)
        {
            case 1:RandMove();break;
            case 2: NMoves(0);break;
            case 3: NMoves(1); break;
            case 4: NMoves(2); break;
            case 5:Melles(); break;
            case 6:TestAI();break;
            case 7:DogeMode();break;
            case 8: HitMode();break;
            case 9: TestAIVer2(0);break;
            case 10: TestAIVer2(1); break;
            case 11: TestAIVer2(2); break;
            case 12: FTargetAI();break;
            case 13: AreaFlagOnly(); break;
            case 14: RootSet();break;
        }
    }
    void RandMove()
    {
        RootSet();
        dash.press = true;
    }
    void NMoves(int mode)
    {
        var check = NPCAim(30, 120);
        switch (mode)
        {
            case 0:
                if (pm.values.now_CursorState == CursorState.Melee) melee.trigger = true;
                break;
            case 1:
                if (pm.values.now_CursorState == CursorState.Shot) melee.trigger = true;
                break;
            case 2:
                gadget.trigger = check && TimeICheck(60);
                ult.trigger = check && TimeICheck(60);
                break;
        }
        shot.trigger = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        shot.press = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
    }
    void Melles()
    {
        if (pm.values.now_CursorState != CursorState.Melee) melee.trigger = true;
        var check = NPCAim(30,120);
        shot.trigger = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        shot.press = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        if (ai_target != null)
        {
            var tdis = Vector2.Distance(ai_target.PosGet, pm.PosGet);
            move = (ai_target.PosGet - pm.PosGet).normalized;
            if (tdis >= 1.0f)
            {
                dash.press = true;
            }
            else
            {
                move *= -1;
                dash.press = false;
            }
        }
    }
    void TestAI()
    {
        if (pm.values.now_CursorState == CursorState.Melee) melee.trigger = true;
        RootSet();
        dash.press = true;

        var check = NPCAim(30,40);
        shot.trigger = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        shot.press = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        gadget.trigger = check && TimeICheck(60);
        ult.trigger = check && TimeICheck(80);
    }
    void DogeMode()
    {
        var nb = NearBullet(false, out var dis);
        if (nb != null && dis <= 3)
        {
            var vc = (pm.PosGet - nb.position).normalized;
            var bf = nb.TryGetComponent<Rigidbody2D>(out var rb) ? (Vector3)rb.linearVelocity.normalized : Vector3.zero;
            if (bf.magnitude <= 0.1f) bf = vc;
            var cross = bf.x * vc.y - bf.y * vc.x;
            move = Quaternion.Euler(0, 0, cross >= 0 ? 110 : -110) * bf;
            dash.press = true;
        }
    }
    void HitMode()
    {
        var nb = NearBullet(false,out var dis);
        if (nb != null)
        {
            if (dis <= 2)
            {
                move = (nb.position - pm.PosGet).normalized;
            }
            else if (dis <= 6)
            {
                var vc = (pm.PosGet - nb.position).normalized;
                var bf = nb.TryGetComponent<Rigidbody2D>(out var rb) ? (Vector3)rb.linearVelocity.normalized : Vector3.zero;
                if (bf.magnitude <= 0.1f) bf = vc;
                var cross = bf.x * vc.y - bf.y * vc.x;
                move = Quaternion.Euler(0, 0, cross < 0 ? 110 : -110) * bf;
            }

            dash.press = true;
        }
    }
    void TestAIVer2(byte modeNo)
    {
        var check = NPCAim(30,40);
        var nb = NearBullet(true,out var bdis);
        if (nb != null && bdis <= 2.5f)
        {
            var vc = (pm.PosGet - nb.position).normalized;
            var bf = nb.TryGetComponent<Rigidbody2D>(out var rb) ? (Vector3)rb.linearVelocity.normalized : Vector3.zero;
            if (bf.magnitude <= 0.1f) bf = vc;
            var cross = bf.x * vc.y - bf.y * vc.x;
            move = Quaternion.Euler(0, 0, cross >= 0 ? 110 : -110) * bf;
            dash.press = true;
        }
        else
        {
            var nowFlag = SAreaFlags(out var afdis,out var naf);
            if (nowFlag)
            {
                dash.press = true;
                for (int i = 0; i < Obj_LocalObjects.Spawnes.Count; i++)
                {
                    var spawne = Obj_LocalObjects.Spawnes[i];
                    if (spawne == null) continue;
                    if (spawne.teamID != pm.states.teamID) continue;
                    ai_vect2 = spawne.transform.position - pm.PosGet;
                    ai_v2pow = 7;
                    break;
                }
            }
            else if (naf != null && afdis <= 20)
            {
                ai_vect2 = naf.transform.position - pm.PosGet;
                ai_v2pow = 3;
            }
            RootSet();
            if (ai_target != null)
            {
                var adis = AIDisGet();
                adis += Mathf.Sin(ai_time_f * 1.0f) * 1.5f;
                var tdis = Vector2.Distance(ai_target.PosGet, pm.PosGet);
                if (!nowFlag)
                {
                    var mv = (ai_target.PosGet - pm.PosGet).normalized;
                    if (tdis >= adis)
                    {
                        move = (move.normalized * 0.25f + (Vector2)mv * 0.75f).normalized;
                        dash.press = true;
                    }
                    else
                    {
                        move = (move.normalized * 0.25f - (Vector2)mv * 0.75f).normalized;
                        dash.press = false;
                    }
                }

                if (modeNo == 0 && TimeICheck(150)) DistanceWepChange(tdis);
            }
        }
        shot.trigger = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        shot.press = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        if (modeNo == 0)
        {
            melee.trigger = check && TimeICheck(60) && Random.value <= 0.4f;
        }
        else if (modeNo == 1 && pm.values.now_CursorState == CursorState.Melee) melee.trigger = true;
        else if (modeNo == 2 && pm.values.now_CursorState == CursorState.Shot) melee.trigger = true;
        gadget.trigger = check && TimeICheck(60);
        ult.trigger = check && TimeICheck(80);
    }
    void FTargetAI()
    {
        var check = NPCAim(30,40, true);
        var nb = NearBullet(true, out var bdis);
        if (nb != null && bdis <= 2.5f)
        {
            var vc = (pm.PosGet - nb.position).normalized;
            var bf = nb.TryGetComponent<Rigidbody2D>(out var rb) ? (Vector3)rb.linearVelocity.normalized : Vector3.zero;
            if (bf.magnitude <= 0.1f) bf = vc;
            var cross = bf.x * vc.y - bf.y * vc.x;
            move = Quaternion.Euler(0, 0, cross >= 0 ? 110 : -110) * bf;
            dash.press = true;
        }
        else
        {
            Player_Manager nearPl = null;
            var nearDis = float.MaxValue;
            for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
            {
                var pl = Obj_LocalObjects.Players[i];
                if (pl.states.teamID != pm.states.teamID) continue;
                if (pl.hpTotal <= 0) continue;
                if (pl.values.noDamTime > 0) continue;
                var dis = Vector2.Distance(pm.PosGet, pl.PosGet);
                if (nearDis > dis)
                {
                    nearDis = dis;
                    nearPl = pl;
                }
            }
            if (nearPl != null)
            {
                ai_v2pow = 5;
                ai_vect2 = nearPl.PosGet - pm.PosGet;
            }
            RootSet();
            if (ai_target != null)
            {
                var adis = AIDisGet();
                adis += Mathf.Sin(ai_time_f * 1.0f) * 1.5f;
                var tdis = Vector2.Distance(ai_target.PosGet, pm.PosGet);
                var mv = (ai_target.PosGet - pm.PosGet).normalized;
                if (tdis >= adis)
                {
                    move = (move.normalized * 0.25f + (Vector2)mv * 0.75f).normalized;
                    dash.press = true;
                }
                else
                {
                    move = (move.normalized * 0.25f - (Vector2)mv * 0.75f).normalized;
                    dash.press = false;
                }
                if (TimeICheck(150)) DistanceWepChange(tdis);
            }
        }

        melee.trigger = check && TimeICheck(60) && Random.value <= 0.4f;
        shot.trigger = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        shot.press = check && Mathf.Repeat(ai_time_f, 1.0f) <= 0.7f;
        gadget.trigger = check && TimeICheck(60);
        ult.trigger = check && TimeICheck(80);
    }
    void AreaFlagOnly()
    {
        var nowFlag = SAreaFlags(out var afdis, out var naf);
        if (nowFlag)
        {
            for (int i = 0; i < Obj_LocalObjects.Spawnes.Count; i++)
            {
                var spawne = Obj_LocalObjects.Spawnes[i];
                if (spawne == null) continue;
                if (spawne.teamID != pm.states.teamID) continue;
                ai_vect2 = spawne.transform.position - pm.PosGet;
                ai_v2pow = 7;
                break;
            }
        }
        else if (naf != null)
        {
            ai_vect2 = naf.transform.position - pm.PosGet;
            ai_v2pow = 3;
        }
        RootSet();
        dash.press = true;
    }
    bool NPCAim(float dismax,float rotSpd, bool friend = false)
    {
        Player_Manager npl = null;
        targetMoveMode = false;
        var ndis = dismax;
        for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
        {
            var pms = Obj_LocalObjects.Players[i];

            if (pms == null) continue;
            if (!friend && pms.states.teamID == pm.states.teamID) continue;
            if (friend && (pms.states.teamID != pm.states.teamID || pms == pm)) continue;
            if (pms.Visible) continue;
            if (pms.hpTotal <= 0) continue;
            if (pms.values.noDamTime > 0) continue;
            var dis = Vector2.Distance(pms.PosGet, pm.PosGet);
            if (ndis < dis) continue;
            var t1 = pm.PosGet;
            var t2 = pms.PosGet;
            if (!Obj_LocalObjects.ViewCheck(t2, pm)) continue;
            var hides = Obj_LocalObjects.HideCheck(t2,pms.passc.charaScale_Multi);
            if (pms.passc.lantan) hides = false;
            if(pms == ai_target)
            {
                if (hides)
                {
                    ai_hideTime += Time.deltaTime;
                    if (ai_hideTime > 3) ai_target = null;
                }
                else ai_hideTime = 0;
            }
            if (hides && pms != ai_target && dis > 2f)continue;
            ndis = dis;
            npl = pms;
        }
        if (npl != null)
        {
            if (ai_target != npl) ai_hideTime = 0;
            ai_target = npl;
            target_pos = pm.objects.camera.WorldToScreenPoint(ai_target.PosGet);
            return true;
        }
        else
        {

            var soundd = float.MaxValue;
            Transform nearSound= null;
            foreach (var sound in Obj_LocalObjects.Sounds)
            {
                if (sound == null) continue;
                if (sound.userTeam == pm.states.teamID) continue;
                var dis = Vector2.Distance(pm.PosGet, sound.transform.position);
                if (dis > sound.rangeMax * pm.passc.soundGet_Multi) continue;
                if (soundd > dis)
                {
                    soundd = dis;
                    nearSound = sound.transform;
                }
            }
            if (nearSound != null)
            {

                target_pos = pm.objects.camera.WorldToScreenPoint(nearSound.position);
                return true;
            }
        }
        if (TimeICheck(60)) ai_rotrev = Random.value >= 0.5f;
        ai_rots += rotSpd * Time.deltaTime * (!ai_rotrev ? 1 : -1);
        var rp = pm.PosGet + (Quaternion.Euler(0, 0, ai_rots) * Vector2.up);
        target_pos = pm.objects.camera.WorldToScreenPoint(rp);
        ai_target = null;
        return false;
    }
    Transform NearBullet(bool useView,out float ndis)
    {
        Transform nb = null;
        ndis = float.MaxValue;
        if (Obj_LocalObjects.TimeStopd) return null;
        for (int i = 0; i < Obj_LocalObjects.Bullets.Count; i++)
        {
            var bull = Obj_LocalObjects.Bullets[i];
            if (bull == null) continue;
            if (bull.pm == null) continue;
            if (bull.pm.states.teamID == pm.states.teamID) continue;
            if (useView && !Obj_LocalObjects.ViewCheck(bull.transform.position, pm)) continue;
            var dis = Vector2.Distance(pm.PosGet, bull.transform.position);
            if (ndis >= dis)
            {
                ndis = dis;
                nb = bull.transform;
            }
        }
        for (int i = 0; i < Obj_LocalObjects.Gadgets.Count; i++)
        {
            var ggb = Obj_LocalObjects.Gadgets[i];
            if(ggb == null || !ggb.active)continue;
            if(!ggb.TryGetComponent<GG_FragGrenade>(out var gre))continue;
            if(gre.max_Damage <= 0)continue;
            var dis = Vector2.Distance(ggb.transform.position, pm.PosGet);
            if (ndis >= dis)
            {
                ndis = dis;
                nb = ggb.transform;
            }
        }
        return nb;
    }
    bool SAreaFlags(out float ndis,out Transform ntrans)
    {
        ndis = float.MaxValue;
        ntrans = null;
        for (int i = 0; i < Obj_LocalObjects.Flags.Count; i++)
        {
            var fg = Obj_LocalObjects.Flags[i];
            if (fg == null) continue;
            if (!fg.flagUse) continue;
            if (fg.pm == pm) return true;
            if (fg.pm != null && fg.pm.states.teamID == pm.states.teamID) continue;
            if (fg.sarea.teamID == pm.states.teamID && !fg.gets) continue;
            var dis = Vector2.Distance(fg.transform.position, pm.PosGet) * 0.5f;
            if (ndis > dis)
            {
                ndis = dis;
                ntrans = fg.transform;
            }
        }
        for (int i = 0; i < Obj_LocalObjects.Areas.Count; i++)
        {
            var ar = Obj_LocalObjects.Areas[i];
            if (ar == null) continue;
            if (!Net_Value.NetCheck || !Net_Value.NetValue.options[3]) continue;
            if (ar.nowTeam == pm.states.teamID) continue;
            var dis = Vector2.Distance(ar.transform.position, pm.PosGet);
            if (ndis > dis)
            {
                ndis = dis;
                ntrans = ar.transform;
            }
        }
        return false;
    }
    void RootSet()
    {
        if (!TimeICheck(6))
        {
            move = ai_vect;
            return;
        }
        var skip = true;
        var mdis = 50f;
        var count = 16;
        var vects = new Vector2[count];
        var diss = new float[count];
        var dissm = new float[count];
        var m = 0f;
        var mtin = false;
        if (pm.values.moveType.damage > 0) mtin = true;
        if (pm.values.moveType.suffocation && pm.values.suffocationTime > pm.states.suffocationTime * 0.5f) mtin = true;
        if(mtin)skip = false;
        for (int i = 0; i < count; i++)
        {
            var rot = 360f / count * i * Mathf.Deg2Rad;
            var vect = new Vector2(Mathf.Sin(rot), Mathf.Cos(rot));
            var d = mdis;
            vects[i] = vect;

                foreach (var hit in Physics2D.CircleCastAll(pm.PosGet, 0.1f, vect, mdis))
                {
                    if (d < hit.distance) continue;
                    if (hit.rigidbody == null && !hit.collider.isTrigger)
                    {
                        d = hit.distance;
                        break;
                    }
                }

            Debug.DrawLine(pm.PosGet, (Vector2)pm.PosGet + vect.normalized * mdis,new Color(0,0,1,0.3f),0.3f);
            Debug.DrawLine(pm.PosGet, (Vector2)pm.PosGet + vect.normalized * d, new Color(1,1,0,0.3f), 0.3f);


            foreach (var mt in CheckMoves(pm.PosGet, vect, 20, d, 0.1f))
            {
                if (!mtin && d < mt.Item1) continue;
                var c = false;
                if (mt.Item2.damage > 0) c = true;
                if (mt.Item2.suffocation && pm.values.suffocationTime > pm.states.suffocationTime*0.5f) c = true;
                if (c == mtin) continue;
                    d = mt.Item1;
            }
            Debug.DrawLine(pm.PosGet, (Vector2)pm.PosGet + vect.normalized * d, new Color(0,1,0,0.3f), 0.3f);
            diss[i] = d;
            var ag = Vector2.Angle(vect, ai_vect);
            var mlt = Data_Base.DB.aiBackRotMult.Evaluate(ag / 180f);
            if(ai_v2pow > 0)
            {
                var ag2 = Vector2.Angle(vect, ai_vect2);
                mlt *= Mathf.Pow(Data_Base.DB.aiBackRotMult.Evaluate(ag2 / 180f),ai_v2pow);
            }
            dissm[i] = Mathf.Pow(d, Data_Base.DB.aiDisPow) * mlt;
            m += dissm[i];
        }
        var fv = ai_vect;
        if (mtin)
        {
            var n = mdis;

            for (int i = 0; i < count; i++)
            {
                if (diss[i] <= 0)continue;
                if (n < diss[i]) continue;
                n = diss[i];
                fv = vects[i];
            }
        }
        else
        {
            if (skip && !TimeICheck(120))
            {
                move = ai_vect;
                dash.press = false;
                return;
            } 
            var r = Random.Range(0, m);
            var p = 0f;
            for (int i = 0; i < count; i++)
            {
                p += dissm[i];
                if (r > p) continue;
                fv = vects[i];
                break;
            }
        }
        move = fv.normalized;
        Debug.DrawLine(pm.PosGet, (Vector2)pm.PosGet + fv.normalized * mdis, Color.red, 1.0f);
        ai_vect = fv.normalized;
        dash.press = !skip;
    }
    List<(float, Data_MoveType)> CheckMoves(Vector2 pos,Vector2 vect,int count,float mdis,float size)
    {
        var results = new List<(float, Data_MoveType)>();
        for (int i = 0; i < count; i++)
        {
            var sd = mdis / count * i;
            var po = pos + vect.normalized * sd;
            var mty = Data_Base.DB.moveTypeBase;
            var pr = int.MinValue;
            foreach (var hit in Physics2D.OverlapCircleAll(po, size))
            {
                if (!hit.TryGetComponent<Obj_MoveType>(out var mtype)) continue;
                if (mtype.priority <= pr) continue;
                mty = mtype.type;
                pr = mtype.priority;
            }
            results.Add((sd,mty));
        }
        return results;
    }
    void DistanceWepChange(float dis)
    {
        var gunD = Data_Base.DB.guns[pm.states.gun_IndexNum];
        var melleD = Data_Base.DB.melles[pm.states.melee_IndexNum];
        var change = false;
        switch (pm.values.now_CursorState)
        {
            case CursorState.Shot:
                if (gunD.ai_Range >= melleD.ai_Range && dis <= melleD.ai_Range) change = true;
                if (gunD.ai_Range < melleD.ai_Range && dis > melleD.ai_Range) change = true;
                break;
            case CursorState.Melee:
                if (gunD.ai_Range >= melleD.ai_Range && dis > gunD.ai_Range) change = true;
                if (gunD.ai_Range < melleD.ai_Range && dis <= gunD.ai_Range) change = true;
                break;
        }
        melee.trigger = change;
    }
    float AIDisGet(float otherDis = 4)
    {
        var adis = otherDis;
        switch (pm.values.now_CursorState)
        {
            case CursorState.Shot:
                adis = Data_Base.DB.guns[pm.states.gun_IndexNum].ai_Range;
                break;
            case CursorState.Melee:
                adis = Data_Base.DB.melles[pm.states.melee_IndexNum].ai_Range;
                break;
        }
        return adis;
    }
}
