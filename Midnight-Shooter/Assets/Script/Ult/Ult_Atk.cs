using System.Collections.Generic;
using UnityEngine;

public class Ult_Atk : Ult_Base
{
    public float damageBase;
    [SerializeField] float injustPer;
    [SerializeField] float hitTimeWait;
    [SerializeField] float hitTimeEnd;
    [SerializeField] float hitCT;
    [SerializeField] float lenghtMax;
    [SerializeField] AnimationCurve lenghtDamCurve;

    [SerializeField] float kbPow;
    [SerializeField] bool kbToCenter;

    [SerializeField] bool selfHit;
    [SerializeField] bool teamHit;
    [SerializeField] bool enemyHit;
    [SerializeField] float wallMult;

    [SerializeField] float damageScoreMult;
    [SerializeField] float killScoreMult;
    [SerializeField, Tooltip("バフ付与")] BufAdd[] bufAdds;
    Collider2D hitCollider;
    List<(GameObject, float)> cts = new();
    public override string InfoGet()
    {
        var istr = $"{LocalizSystem.LocailzSCInfo("基礎ダメージ")}{damageBase}";
        if (injustPer > 0) istr += $"\n{LocalizSystem.LocailzSCInfo("負傷割合")}{injustPer}%";
        if (lenghtMax > 0) istr += "\n" + Data_Base.DisInfoStr(lenghtDamCurve, lenghtMax);
        if (wallMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("壁倍率")}{wallMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (bufAdds.Length > 0) istr += "\n" + Data_Base.BufInfoStr(bufAdds);
        if (damageScoreMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("ダメージスコア")}{damageScoreMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (killScoreMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("キルスコア")}{killScoreMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (kbPow != 0) istr += $"\n{LocalizSystem.LocailzSCInfo("ノックバック力")}{Mathf.Abs(kbPow)}";
        return istr;
    }
    public override void Start()
    {
        base.Start();
        hitCollider = gameObject.GetComponent<Collider2D>();
        if (hitCollider != null)
        {
            bool hitcheck = false;
            if (Net_Connect.CanControl(Object) && hitTimeWait <= 0) hitcheck = true;
            hitCollider.enabled = hitcheck;
        }
    }
    override public void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object)) return;
        if (Obj_LocalObjects.TimeStopd)
        {
            if (hitCollider != null) hitCollider.enabled = false;
            return;
        }
        if (hitCT > 0)
        for(int i = cts.Count - 1; i >= 0; i--)
        {
            var ctd = cts[i];
            ctd.Item2 -= Time.deltaTime;
            if(ctd.Item2 <= 0)cts.RemoveAt(i);
            else cts[i] = ctd;
        }
        if (hitCollider != null)
        {
            bool hitcheck = true;
            if (hitTimeWait > 0 && time < hitTimeWait) hitcheck = false;
            if (hitTimeEnd > 0 && time > hitTimeEnd) hitcheck = false;
            hitCollider.enabled = hitcheck;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Net_Connect.CanControl(Object)) return;
        hit(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!Net_Connect.CanControl(Object)) return;
        hit(other);
    }

    void hit(Collider2D other)
    {
        if (hitTimeWait > 0 && time < hitTimeWait) return;
        if (hitTimeEnd > 0 && time > hitTimeEnd) return;
        if (Obj_LocalObjects.TimeStopd) return;
        var check = true;
        for (int i = 0; i < cts.Count; i++)
        {
            if (cts[i].Item1 == other.gameObject)
            {
                check = false;
                break;
            }
        }

        if (!check) return;
        cts.Add((other.gameObject, hitCT > 0 ? hitCT : float.PositiveInfinity));
        //プレイヤー
        if (other.TryGetComponent<Player_Hit>(out var phit))
        {
            var hc = false;
            if (selfHit && phit.pm == pm) hc = true;
            if (selfHit && phit.pm != pm && phit.pm.states.teamID == pm.states.teamID) hc = true;
            if (enemyHit && phit.pm.states.teamID != pm.states.teamID) hc = true;
            if (hc)
            {
                if (damageBase != 0) phit.pm.Damage(damVal(phit.pm.PosGet), injustPer, false, pm, ultid + (int)AtkID.Ult, damageScoreMult, killScoreMult);
                phit.pm.BufChanges(bufAdds);
                if (kbPow != 0)
                {
                    var vect = !kbToCenter ? transform.up : (phit.pm.PosGet - transform.position);
                    phit.pm.KBSet(vect.normalized * kbPow);
                }
            }
        }
        //プレイヤー回避
        else if (other.TryGetComponent<Player_Doges>(out var pdg))
        {
            if (enemyHit && pdg.pm.states.teamID != pm.states.teamID) pdg.DogeAdd();
        }
        //壁
        else if (wallMult > 0 && other.TryGetComponent<Obj_Wall>(out var wall))
        {
            wall.Damage(damVal(wall.transform.position) * wallMult);
        }
    }
    float damVal(Vector2 pos)
    {
        var dam = damageBase;
        if(lenghtMax > 0)
        {
            var dis = Vector2.Distance(transform.position, pos);
            dam *= lenghtDamCurve.Evaluate(Mathf.Clamp01(dis / lenghtMax));
        }
        return dam;
    }
}
