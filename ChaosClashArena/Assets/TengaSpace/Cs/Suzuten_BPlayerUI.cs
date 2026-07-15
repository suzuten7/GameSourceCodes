using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Suzuten_PlayerSets;
public class Suzuten_BPlayerUI : MonoBehaviour
{
    [SerializeField] Suzuten_PlayerState PS;
    [SerializeField] RawImage StanEffect;
    [SerializeField] RawImage PinchEffect;
    [SerializeField] RawImage BlindEffect;
    [SerializeField] GameObject ChaosEffect;
    void FixedUpdate()
    {
        float MaxAlpha;
        #region スタン
        Color StanCol = StanEffect.color;
        MaxAlpha = 0.4f;
        if (PS.ActionTime < 0 && PS.HP > 0) StanCol.a = Mathf.Clamp(StanCol.a + (MaxAlpha*0.02f), 0, MaxAlpha);
        else StanCol.a = Mathf.Clamp(StanCol.a - (MaxAlpha*0.008f), 0, MaxAlpha);
        StanEffect.color = StanCol;
        #endregion
        #region ピンチ
        Color PinchCol = PinchEffect.color;
        MaxAlpha = 0.5f;
        if (PS.HP <= (PS.CD.MHP * BOP_HMSP[0]*0.01)*0.25f) PinchCol.a = Mathf.Clamp(PinchCol.a + (MaxAlpha*0.02f), 0, MaxAlpha);
        else PinchCol.a = Mathf.Clamp(PinchCol.a - (MaxAlpha*0.004f), 0, MaxAlpha);
        PinchEffect.color = PinchCol;
        #endregion
        #region 暗闇
        Color BlindCol = BlindEffect.color;
        MaxAlpha = 0.8f;
        if (PS.Bufs.ContainsKey((int)Suzuten_DataBase.BufsE.視界妨害)) BlindCol.a = Mathf.Clamp(BlindCol.a + (MaxAlpha*0.02f), 0, MaxAlpha);
        else BlindCol.a = Mathf.Clamp(BlindCol.a - (MaxAlpha*0.004f), 0, MaxAlpha);
        BlindEffect.color = BlindCol;
        #endregion
        #region 混沌超弦
        ChaosEffect.SetActive(PS.Bufs.ContainsKey((int)Suzuten_DataBase.BufsE.混沌超弦));
        #endregion
    }
}
