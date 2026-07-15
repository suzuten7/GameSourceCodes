using Fusion;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

/* 内容
 * ・攻撃判定
 */

public class Bullet_Hit : NetworkBehaviour
{

    [Networked] public Player_Manager pm { get; set; }
    public int gunID;
    public Data_Gun gunData;
    public Player_Manager headShotTarget;
    public int hp;
    Vector2 startPos;
    Vector2 targetPoint;
    Rigidbody2D rb;
    float diss = 0f;
    float time = 0f;

    float deltime = 0;

    float delRange = 0;
    [SerializeField] Collider2D hitCollider;
    List<(GameObject, float)> cts = new();
    Net_RigSync rigSync;
    public void SetUp(Player_Manager pmg, int gid, Data_Gun gunD, Vector3 point)
    {
        pm = pmg;
        //頭にカーソルがあるかチェック
        foreach (var hits in Physics2D.OverlapCircleAll(point, 0.1f))
        {
            if (hits.TryGetComponent<Player_Hit>(out var ph))
            {
                headShotTarget = ph.pm;
            }
        }


        gunID = gid;
        gunData = gunD;
        hp = gunData.bullets.HP;
        startPos = transform.position;
        targetPoint = point;
        if(gunData.bullets.targetPosRem)delRange = Vector2.Distance(transform.position, point);
    }
    private void Start()
    {
        Library_ObjParentSet.ParentSet(gameObject, "bullet's");
        Obj_LocalObjects.Bullets.Add(this);
        hitCollider = gameObject.GetComponent<Collider2D>();
        if(hitCollider != null)
        {
            bool hitcheck = false;
            if (Net_Connect.CanControl(Object) && gunData.bullets.hitStartTime <= 0) hitcheck = true;
            hitCollider.enabled = hitcheck;
        }

    }
    private void FixedUpdate()
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (pm == null)
        {
            deltime += Time.deltaTime;
            if (deltime > 5) Destroy(gameObject);
            return;
        }
        else deltime = 0;
        if (Obj_LocalObjects.TimeStopd)
        {
            if (!gunData.bullets.noStop)
            {
                if (hitCollider != null) hitCollider.enabled = false;
                return;
            }
            else
            {
                if (rigSync == null) rigSync = GetComponent<Net_RigSync>();
                if (rigSync != null) rigSync.noStop = true;
            }
        }
        var disUse = false;
        if (gunData.damages.distanceMode == DistanceMode.Vect_Time) disUse = true;
        if(gunData.bullets.targetPosRem)disUse = true;
        if (disUse)
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (rb != null) diss += rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        }
        if(gunData.bullets.hitCT > 0)
        for (int i = cts.Count - 1; i >= 0; i--)
        {
            var ctd = cts[i];
            ctd.Item2 -= Time.deltaTime;
            if (ctd.Item2 <= 0) cts.RemoveAt(i);
            else cts[i] = ctd;
        }
        time += Time.deltaTime;
        if (hitCollider != null)
        {
            bool hitcheck = true;
            if (gunData.bullets.hitStartTime > 0 && time < gunData.bullets.hitStartTime) hitcheck = false;
            if (gunData.bullets.hitEndTime > 0 && time > gunData.bullets.hitEndTime) hitcheck = false;
            hitCollider.enabled = hitcheck;
        }
        if (time >= gunData.bullets.time)
        {
            ShotRem();
        }
        if (gunData.bullets.targetPosRem && diss >= delRange)
        {
            ShotRem();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (pm == null) return;
        if (gunData.bullets.hitCT > 0) return;
        hits(collision);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (pm == null) return;
        if (gunData.bullets.hitCT <= 0) return; 
        hits(collision);
    }

    void hits(Collider2D collision)
    {
        if (gunData.bullets.hitStartTime > 0 && time < gunData.bullets.hitStartTime) return;
        if (gunData.bullets.hitEndTime > 0 && time > gunData.bullets.hitEndTime) return;
        if (Obj_LocalObjects.TimeStopd && !gunData.bullets.noStop) return;
        var check = true; 
        for (int i = 0; i < cts.Count; i++)
        {
            if (cts[i].Item1 == collision.gameObject)
            {
                check = false;
                break;
            }
        }
        if (!check) return;
        cts.Add((collision.gameObject, gunData.bullets.hitCT > 0 ? gunData.bullets.hitCT : float.PositiveInfinity));
        //攻撃が当たった
        if (collision.gameObject.TryGetComponent<Player_Hit>(out var ph))
        {
            /* メモ
             * ・ph有 受け側
             * ・ph無 射撃側
             */
            bool hit = false;

            //自分自身
            if (gunData.bullets.self_Hit && pm == ph.pm) hit = true;
            //味方
            if (gunData.bullets.team_Hit && pm != ph.pm && pm.states.teamID == ph.pm.states.teamID) hit = true;
            //敵
            if (gunData.bullets.enemy_Hit && pm.states.teamID != ph.pm.states.teamID) hit = true;
            if (ph.pm.values.noDamTime > 0) return;
            if (hit)
            {
                bool headshot = ph.pm == headShotTarget;
                var hdis = Vector2.Distance(ph.pm.PosGet, targetPoint);
                if (hdis <= ph.pm.passc.charaScale_Multi) headshot = true;
                var damScore = gunData.damageScoreMult * pm.passc.scoreDamage_Multi;
                var killScore = gunData.killScoreMult * pm.passc.scoreKill_Multi;
                if (headshot) killScore *= pm.passc.scoreHeadKill_Multi;
                //ダメージの適応
                ph.pm.Damage(damageVal(ph.pm.PosGet, headshot), gunData.damages.injustPer, headshot, pm, gunID, damScore, killScore);
                ph.pm.BufChanges(gunData.bufAdds);
                if(gunData.kbPow != 0)
                {
                    Vector2 kbVect;
                    if (gunData.kbCenter) kbVect = transform.position - ph.pm.PosGet;
                    else kbVect = transform.up;
                    ph.pm.KBSet(kbVect.normalized * gunData.kbPow);
                }
                if (gunData.bullets.perce == BullectPerce.No) ShotRem();
            }
        }
        //プレイヤー回避
        else if(collision.gameObject.TryGetComponent<Player_Doges>(out var pdg))
        {
            //敵
            if (gunData.bullets.enemy_Hit && pm.states.teamID != pdg.pm.states.teamID)pdg.DogeAdd();
        }
        //弾同士の衝突
        else if (collision.gameObject.TryGetComponent<Bullet_Hit>(out var bh))
        {
            if (gunData.bullets.bullet_Hit && bh.gunData.bullets.bullet_Hit && pm.states.teamID != bh.pm.states.teamID)
            {
                if (hp < 0 || bh.hp < 0) return;
                var chp = hp;
                hp -= bh.hp;
                bh.hp -= chp;
                if (hp <= 0) ShotRem();
                if (bh.hp <= 0) bh.ShotRem();
            }

        }
        //ガジェットへの衝突
        else if (collision.gameObject.TryGetComponent<GG_Base>(out var ggb))
        {
            if (gunData.bullets.gadget_Hit)
            {
                ggb.Damage(damageVal(ggb.transform.position, false));
                if (gunData.bullets.perce == BullectPerce.No) ShotRem();
            }
        }
        //破壊可能の壁
        else if (collision.gameObject.TryGetComponent<Obj_Wall>(out var wall))
        {
            wall.Damage(damageVal(wall.transform.position, false) * gunData.damages.wallMult);
            if (gunData.bullets.perce != BullectPerce.Wall) ShotRem();
        }
        //オブジェクトに衝突
        else if (!collision.isTrigger && gunData.bullets.perce != BullectPerce.Wall) ShotRem();
    }
    float damageVal(Vector2 hitPos,bool head)
    {
        var val = gunData.damages.damageBase;
        if (head) val *= gunData.damages.headMulti*pm.passc.damageHeadAdd_Multi;
        val *= pm.passc.damageAllAdd_Multi;
        if(gunID < (int)AtkID.Gun + (int)AtkID.CSize)val *= pm.passc.damageGunAdd_Multi;
        else if(gunID < (int)AtkID.Melee + (int)AtkID.CSize) val *= pm.passc.damageMeleeAdd_Multi;
        var dis = 0f;
        switch (gunData.damages.distanceMode)
        {
            default:
                return val;
            case DistanceMode.PositionStart_Hit:
                dis = Vector2.Distance(startPos, hitPos);
                break;
            case DistanceMode.Vect_Time:
                dis = diss;
                break;
        }
        var dist = Mathf.Clamp01(dis / gunData.damages.disMax);
        val *= gunData.damages.disMult.Evaluate(dist);

        return val;
    }
    void ShotRem()
    {
        AddShots(gunData.bullets.remAddShot);
        Destroy(gameObject);
    }
    public void AddShots(Data_Gun addShotD)
    {
        if (addShotD == null) return;
        for(int i = 0; i < addShotD.shots.pallet; i++)
        {
            Vector3 rot = transform.eulerAngles;
            var dif = addShotD.shots.stopDiffusion.hip_Diffusion;
            rot.z += Random.Range(-dif / 2, dif / 2);
            var qrot = Quaternion.Euler(rot);
            float speed = addShotD.shots.speed * pm.passc.bulletSpeed_Multi * Random.Range(addShotD.shots.speed_Min, 1);
            var vect = qrot * Vector2.up * speed;
            var bullet_Obj = Runner.Spawn
                (
                addShotD.shots.bulletObj,
                transform.position,
                qrot,
                PlayerRef.None,
                onBeforeSpawned: (runner, obj) =>
                {
                    Net_RigSync.NStartSet(obj, vect);
                }
                ).gameObject;

            Bullet_Hit b = bullet_Obj.GetComponent<Bullet_Hit>();
            b.SetUp(pm,gunID, addShotD, transform.position);
        }
        Net_Value.SoundSet(transform.position, pm, addShotD.shots.soundRange, addShotD.shots.soundTime,addShotD.shots.seAudio,addShotD.shots.seVolue,addShotD.shots.sePitch);
    }
}
