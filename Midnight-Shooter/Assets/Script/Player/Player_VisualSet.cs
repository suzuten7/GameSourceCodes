using Unity.VisualScripting;
using UnityEngine;

/* 内容
 * ・プレイヤー外見・アクティブ設定
 */

public class Player_VisualSet : MonoBehaviour
{
    [SerializeField]Player_Manager pm;
    GameObject imgObj;
    int wepID = -1;
    int charaImgID = -1;
    int loadhash = 0;
    bool colchange;
    SpriteRenderer sr;
    Player_ImgAnims imgAnim;
    void Update()
    {
        //見た目の色
        var ccol = Data_Base.TeamColorGet(pm.states.teamID);
        Color.RGBToHSV(ccol, out var h, out _, out _);
        var s = 1f;
        var v = 1f;
        if(pm != Obj_LocalObjects.MyPlayer)
        {
            s *= 0.5f;
        }
        if(pm.hpTotal <= 0)
        {
            v *= 0.3f;
        }
        if (pm.values.noDamTime > 0) v *= 0.6f + ((Mathf.Sin(Time.time * 4) + 1f) * 0.5f * 0.2f);
        var col = Color.HSVToRGB(h, s, v);
        if (pm.Visible)
        {
            if (Obj_LocalObjects.MyPlayer == null || pm.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID) col.a = 0.5f;
            else col.a = 0;
        }
        //自分自身の場合
        bool circle = false;
        bool vision = false;
        bool viewall = false;
        bool dif = false;
        bool name = false;
        bool hp = false;
        if (pm == Obj_LocalObjects.MyPlayer)
        {
            circle = pm.PassiveLvGet(Passive.Blind) <= 0;
            vision = !pm.BufGet(BufType.Darkness);
            dif = true;
            viewall = pm.PassiveLvGet(Passive.Cheat_View) > 0;
            if (pm.BufGet(BufType.WallHack)) viewall = true;
            pm.objects.dif_Mr.material.color = UI_OptionManager.OptionGetColor("UI_Option 07", new Color(0, 1, 0, 0.5f));
            pm.objects.dif_View.viewDistance = UI_OptionManager.OptionGetFloat("UI_Option 08", 2);

        }
        //チームメンバーの場合
        else if (Obj_LocalObjects.MyPlayer == null || pm.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID)
        {
            circle = true;
            hp = true;
            name = true;
        }
        //敵
        if (Obj_LocalObjects.MyPlayer != null && pm.states.teamID != Obj_LocalObjects.MyPlayer.states.teamID)
        {
            if (pm.passc.lantan) circle = true;
            if (!pm.Visible && Obj_LocalObjects.ViewCheck(pm.PosGet, Obj_LocalObjects.MyPlayer) && !Obj_LocalObjects.HideCheck(pm.PosGet,pm.passc.charaScale_Multi))
            {
                if(Obj_LocalObjects.MyPlayer != null && Obj_LocalObjects.MyPlayer.PassiveLvGet(Passive.NoEnemyHPUI) <= 0)hp = true;
                name = true;
            }
            if (pm.PassiveLvGet(Passive.HideHP) > 0) hp = false;
        }
        if (Obj_LocalObjects.MyPlayer != null && Obj_LocalObjects.MyPlayer.PassiveLvGet(Passive.Cheat_View) >= 2)
        {
            name = true;
            hp = true;
        }
        pm.objects.auraSr.color = col;
        //キャラ外見
        int loadimg;
        UI_ImageLoader_Base.ImgBase imgSet = null;
        int hash = 0;
        if (Net_Connect.CanControl(pm.Object))
        {
            loadimg = pm.states.loadImgID;
            if(loadimg > 0) imgSet = UI_ImageLoader_Base.ImgSets[loadimg - 1];
            hash = Library_FireBase.HashGet(imgSet);
        }
        else
        {
            loadimg = pm.objects.valueSync.loadImg_hash;
            if (loadimg != 0 && Player_ValueSync.loadImg_dics.TryGetValue(loadimg, out var img)) imgSet = img;
            hash = loadimg;
        }
        if (imgSet == null)
        {
            if (charaImgID != pm.states.charaImgID)
            {
                charaImgID = pm.states.charaImgID;
                loadhash = 0;
                if (pm.objects.charaImgObj != null) Destroy(pm.objects.charaImgObj);
                var imgd = Data_Base.DB.charaImgs[pm.states.charaImgID];
                if (imgd.imgObj != null) pm.objects.charaImgObj = Instantiate(imgd.imgObj, pm.objects.scaleTrans);
                imgAnim = pm.objects.charaImgObj.GetComponent<Player_ImgAnims>();
                colchange = !imgd.colorNoChange;
                if (imgAnim == null)
                {
                    var srs = pm.objects.charaImgObj.GetComponentsInChildren<SpriteRenderer>();
                    for (int i = 0; i < srs.Length; i++)
                    {
                        if (srs[i].gameObject.layer != LayerMask.GetMask("EditorView")) sr = srs[i];
                    }
                }
                else sr = imgAnim.sr;
            }
        }
        else
        {
            if (loadhash != hash)
            {
                loadhash = hash;
                charaImgID = -1;
                if (pm.objects.charaImgObj != null) Destroy(pm.objects.charaImgObj);
                var imgObj = Instantiate(Data_Base.DB.imgAnimObj, pm.objects.scaleTrans);
                pm.objects.charaImgObj = imgObj.gameObject;
                imgAnim = imgObj;
                imgAnim.imgAnims.Clear();
                for (int i = 0; i < imgSet.datas.Count; i++)
                {
                    var imgd = imgSet.datas[i];
                    var id = -1;
                    for (int k = 0; k < imgAnim.imgAnims.Count; k++)
                    {
                        var ianim = imgAnim.imgAnims[k];
                        if (ianim.animType == (Player_ImgAnims.AnimType)imgd.type)
                        {
                            id = k;
                            break;
                        }
                    }
                    if (id < 0)
                    {
                        imgAnim.imgAnims.Add
                            (
                            new Player_ImgAnims.imgAnim
                            {
                                animType = (Player_ImgAnims.AnimType)imgd.type,
                                paturnSpeed = imgd.type < imgSet.speeds.Count ? imgSet.speeds[imgd.type] : 1,
                                imgs = new(),
                            }
                            );
                        id = imgAnim.imgAnims.Count - 1;
                    }
                    imgAnim.imgAnims[id].imgs.Add(imgd.TextureGet);
                }
                sr = imgAnim.sr;
                colchange = false;
            }
        }
        if (Net_Connect.CanControl(pm.Object))
        {
            pm.values.atkAnimID = 0;
            var atkTimes = 1.2f;
            if (pm.values.lastAtkTime <= atkTimes) pm.values.atkAnimID = pm.values.now_CursorState != CursorState.Melee ? 1 : 2;
            if (pm.values.lastGGTime <= atkTimes) pm.values.atkAnimID = 3;
            if (pm.values.lastUltTime <= atkTimes) pm.values.atkAnimID = 4;
        }
        if (sr != null)
        {
            Color srcol;
            if (colchange) srcol = pm.objects.auraSr.color;
            else
            {
                srcol = sr.color;
                srcol.a = pm.objects.auraSr.color.a;
            }
            sr.color = srcol;
        }
        if (imgAnim != null)
        {
            imgAnim.moveAnimID = (int)pm.values.now_MoveState;
            imgAnim.atkAnimID = pm.values.atkAnimID;
            imgAnim.death = pm.hpTotal <= 0;
        }
        //装備外見
        var gid = -1;
        switch (pm.values.now_CursorState)
        {
            case CursorState.Shot:gid = pm.states.gun_IndexNum; break;
            case CursorState.Melee:gid = pm.states.melee_IndexNum + (int)AtkID.Melee; break;
        }
        if(wepID != gid)
        {
            wepID = gid;
            if(imgObj != null)Destroy(imgObj);
            GameObject imobj = null;
            if (gid < (int)AtkID.Gun) imobj = null;
            else if (gid < (int)AtkID.Gun + (int)AtkID.CSize) imobj = Data_Base.DB.guns[gid].imgObj;
            else if(gid < (int)AtkID.Melee + (int)AtkID.CSize) imobj = Data_Base.DB.melles[gid - (int)AtkID.Melee].imgObj;
            if (imobj != null) imgObj = Instantiate(imobj, pm.objects.handTrans);
        }
        //反映
        var circledis = 2 + pm.PassiveValGet(Passive.Lantan);
        if (pm.values.killLight)
        {
            circledis = Mathf.Max(circledis, 4);
            circle = true;
        }
        if (pm.BufGet(BufType.Fire))
        {
            circledis = Mathf.Max(circledis, Data_Base.BufDGet(BufType.Fire).values[1]);
            circle = true;
        }
        if (pm.BufGet(BufType.SpotLit))
        {
            circledis = Mathf.Max(circledis, Data_Base.BufDGet(BufType.SpotLit).values[1]);
            circle = true;
        }
        pm.objects.circle_Light.gameObject.SetActive(circle);
        pm.objects.circle_Light.pointLightOuterRadius = circledis;
        pm.objects.circle_Light.pointLightInnerRadius = circledis - 1f;
        pm.objects.circle_Mask.gameObject.SetActive(circle);
        pm.objects.circle_Mask.viewDistance = circledis;
        pm.objects.viewAll.gameObject.SetActive(viewall);
        pm.objects.vision_Light.gameObject.SetActive(vision);
        pm.objects.vision_Light.pointLightOuterAngle = pm.states.vision_Angle;
        pm.objects.vision_Light.pointLightInnerAngle = pm.states.vision_Angle - 5f;
        pm.objects.vision_Mask.gameObject.SetActive(vision);
        pm.objects.vision_Mask.viewAngle = pm.states.vision_Angle;
        pm.objects.dif_View.gameObject.SetActive(dif);
        pm.objects.HPUI.SetActive(hp);
        pm.objects.nameUI.SetActive(name);
    }
}
