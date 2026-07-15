using UnityEngine;
using UnityEngine.Rendering;
using static BattleManager;
using static Statics;
public class Player_Target : MonoBehaviour
{
    [SerializeField] Player_Cont PCont;
    [SerializeField] State_Base Sta;
    [SerializeField] Camera Cam;
    [SerializeField] int TargetTimes;
    [SerializeField] GameObject TargetObj;
    public bool TargetOff = false;
    private void Update()
    {
        if (PCont.Target_Enter) TargetOff = !TargetOff;
    }
    void FixedUpdate()
    {
        if (Sta.TargetHit==null || (!TargetOff && PCont.Look.magnitude>=0.1f))
        {
            TargetSet();
        }
        if (Sta.TargetHit != null && (Sta.TargetHit.Sta.HP <= 0 || !Sta.TargetHit.gameObject.activeInHierarchy))
        {
            Sta.TargetHit = null;
            TargetSet();
        }
    }
    void TargetSet()
    {
        float NearDis = -1;
        foreach (var Hits in BTManager.HitList)
        {
            if (Hits == null) continue;
            if (!TeamCheck(Sta, Hits.Sta)) continue;
            if (Hits.Sta.HP <= 0) continue;
            if (!Hits.Sta.gameObject.activeInHierarchy) continue;
            var CPos = Cam.WorldToViewportPoint(Hits.PosGet());
            if (CPos.z <= 0) continue;
            if (CPos.x < 0f || CPos.x > 1f) continue;
            if (CPos.y < 0f || CPos.y > 1f) continue;
            CPos.x -= 0.5f;
            CPos.y -= 0.5f;
            float Dis = new Vector2(CPos.x, CPos.y).magnitude;
            if (NearDis < 0 || NearDis > Dis)
            {
                NearDis = Dis;
                Sta.TargetHit = Hits;
            }
        }
    }
    private void LateUpdate()
    {
        if (Sta.TargetHit != null)
        {
            TargetObj.SetActive(true);
            TargetObj.transform.position = Sta.TargetHit.PosGet();
        }
        else TargetObj.SetActive(false);
    }
}
