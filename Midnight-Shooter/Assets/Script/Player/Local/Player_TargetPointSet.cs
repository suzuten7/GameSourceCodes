using UnityEngine;

public class Player_TargetPointSet : MonoBehaviour
{
    [SerializeField] Player_Manager pm;
    [SerializeField] Transform targetPointTrans;
    [SerializeField] Transform recoilPointTrans;
    [SerializeField] GameObject targetPointObj;
    [SerializeField] GameObject recoilPointObj;
    int bimgid = -1;
    int bloadid = -1;

    Color tpcb = Color.clear;
    Color rpcb = Color.clear;
    void Update()
    {
        var change = false;

        var tploadid = UI_OptionManager.OptionGetInt("GP_Option 12", 0);
        if (tploadid > 0)
        {
            bimgid = -1;
            if (tploadid != bloadid)
            {
                change = true;
                bloadid = tploadid;
                var reticles = Data_Base.DB.reticleLoadsObj;
                if (targetPointObj != null) Destroy(targetPointObj);
                var tpimg = Instantiate(reticles, targetPointTrans);
                loadImgSet(tpimg, tploadid);
                targetPointObj = tpimg.gameObject;
                if (recoilPointObj != null) Destroy(recoilPointObj);
                var rpimg = Instantiate(reticles, recoilPointTrans);
                loadImgSet(rpimg, tploadid);
                recoilPointObj = rpimg.gameObject;
            }
        }
        else
        {
            var tpimgid = UI_OptionManager.OptionGetInt("GP_Option 06", 0);
            bloadid = -1;
            if (tpimgid != bimgid)
            {
                change = true;
                bimgid = tpimgid;
                var objbase = Data_Base.DB.reticles[tpimgid].reticleObj;
                if (targetPointObj != null) Destroy(targetPointObj);
                targetPointObj = Instantiate(objbase, targetPointTrans);
                if (recoilPointObj != null) Destroy(recoilPointObj);
                recoilPointObj = Instantiate(objbase, recoilPointTrans);
            }
        }
        var tpc = UI_OptionManager.OptionGetColor("GP_Option 07", new Color(1, 1, 1, 0.5f));
        if (change || tpcb != tpc)
        {
            tpcb = tpc;
            var tsps = targetPointObj.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < tsps.Length; i++)
            {
                tsps[i].color = tpc;
            }
        }
        var rpc = UI_OptionManager.OptionGetColor("GP_Option 08", new Color(1, 0, 0, 0.5f));
        if (change || rpcb != rpc)
        {
            rpcb = rpc;
            var rsps = recoilPointObj.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < rsps.Length; i++)
            {
                rsps[i].color = rpc;
            }
        }

        if(targetPointObj!=null) targetPointObj.transform.localScale = Vector3.one * UI_OptionManager.OptionGetFloat("GP_Option 13", 100) * 0.01f;
        if(recoilPointObj != null) recoilPointObj.transform.localScale = Vector3.one * UI_OptionManager.OptionGetFloat("GP_Option 14", 100) * 0.01f;
        var tdis = Vector2.Distance(pm.PosGet, pm.objects.targetPoint.position);
        pm.objects.targetDisTx.transform.localScale = Vector3.one * UI_OptionManager.OptionGetFloat("GP_Option 15", 100) * 0.01f;
        pm.objects.targetDisTx.color = UI_OptionManager.OptionGetColor("GP_Option 10", new Color(1, 1, 1, 0.5f));
        pm.objects.targetDisTx.text = tdis.ToString("F1") + "m";

        if (UI_OptionManager.OptionGetOnOff("GP_Option 16", true))
        {
            Data_Gun gund = null;
            switch (pm.values.now_CursorState)
            {
                case CursorState.Shot:
                    gund = Data_Base.DB.guns[pm.states.gun_IndexNum];
                    break;
                case CursorState.Melee:
                    gund = Data_Base.DB.melles[pm.states.melee_IndexNum];
                    break;
            }
            if (gund != null && gund.damages.distanceMode != DistanceMode.NoUse)
            {
                float dmin = float.MaxValue;
                float dmax = float.MinValue;
                foreach (var key in gund.damages.disMult.keys)
                {
                    if (key.value < dmin) dmin = key.value;
                    if (key.value > dmax) dmax = key.value;
                }
                var disMult = gund.damages.disMult.Evaluate(Mathf.InverseLerp(0, gund.damages.disMax, tdis));
                Color disCol;
                if (disMult < 1)
                {
                    float p = Mathf.InverseLerp(dmin, 1f, disMult);
                    disCol = Data_Base.DB.disGrad.Evaluate(p / 2f);
                }
                else
                {
                    float p = Mathf.InverseLerp(1f, dmax, disMult);
                    disCol = Data_Base.DB.disGrad.Evaluate(p / 2f + 0.5f);
                }
                pm.objects.targetDisTx.text += $"\n<size=70%><color=#{ColorUtility.ToHtmlStringRGB(disCol)}>(x{disMult:F2})</color></size>";
            }
        }

    }
    void loadImgSet(Player_ImgAnims imgAnim,int ID)
    {
        if (ID <= 0) return;
        var imgset = UI_ImageLoader_Base.ImgSets[ID - 1];
        imgAnim.imgAnims[0].imgs.Clear();
        imgAnim.imgAnims[0].paturnSpeed = imgset.speeds.Count > 0 ? imgset.speeds[0] : 1;
        for (int i = 0; i < imgset.datas.Count; i++)
        {
            imgAnim.imgAnims[0].imgs.Add(imgset.datas[i].TextureGet);
        }
    }
}
