using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Statics;
public class UI_Sin_BossUI : UI_State
{
    public RawImage Icon;
    [SerializeField]TextMeshProUGUI DistanceTx;
    [SerializeField] RectTransform PosArrow;
    override protected void LateUpdate()
    {

    }
    public void DisSet(Vector3 Pos,Camera Cam)
    {
        var DisStr = HoriDistance(Sta.PosGet(), Pos).ToString("F1") + "μm";
        if (DistanceTx.text != DisStr) DistanceTx.text = DisStr;
        Vector3 dirToTarget = (Sta.PosGet() - Pos).normalized;
        Vector3 camForward = Cam.transform.forward;
        Vector3 camRight = Cam.transform.right;
        dirToTarget.y = 0;
        camForward.y = 0;
        camRight.y = 0;

        float angle = Mathf.Atan2(
            Vector3.Dot(dirToTarget.normalized, camForward.normalized),
            Vector3.Dot(dirToTarget.normalized, camRight.normalized)
        ) * Mathf.Rad2Deg;
        var Rot = PosArrow.eulerAngles = new Vector3(0, 0, (int)angle);
        if (PosArrow.eulerAngles != Rot) PosArrow.eulerAngles = Rot;
    }
}
