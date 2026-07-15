using System.Collections.Generic;
using UnityEngine;

public class UI_MapSets : MonoBehaviour
{
    [SerializeField] List<UI_MapPointer> plPointers;
    [SerializeField] List<UI_MapPointer> markPointers;
    [SerializeField] List<UI_MapPointer> areaPointers;
    [SerializeField] List<UI_MapPointer> flagPointers;
    void Update()
    {
        var mapSize = 30 * UI_OptionManager.OptionGetFloat("UI_Option 09", 1f);
        Camera cam = null;
        if (Obj_LocalObjects.MyPlayer != null)
        {
            Obj_LocalObjects.MyPlayer.objects.mapCam.orthographicSize = mapSize;
            cam = Obj_LocalObjects.MyPlayer.objects.mapCam;
        }
        if (Obj_LocalObjects.ViewObj != null)
        {
            Obj_LocalObjects.ViewObj.mapCam.orthographicSize = mapSize;
            cam = Obj_LocalObjects.ViewObj.mapCam;
        }
        if (cam == null) return;

        for (int i = 0; i < Mathf.Max(Obj_LocalObjects.Players.Count,plPointers.Count); i++)
        {
            if(i >= Obj_LocalObjects.Players.Count)
            {
                plPointers[i].gameObject.SetActive(false);
                continue;
            }
            if(i >= plPointers.Count)
            {
                plPointers.Add(Instantiate(plPointers[0], plPointers[0].transform.parent));
            }
            var pl = Obj_LocalObjects.Players[i];
            var po = plPointers[i];
            if (pl == null || pl.Object == null)
            {
                po.gameObject.SetActive(false);
                continue;
            }
            var view = Obj_LocalObjects.MyPlayer == null;
            if (!view && pl.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID) view = true;
            if (!view && pl.values.killLight) view = true;
            if (!view && Obj_LocalObjects.MyPlayer.PassiveLvGet(Passive.Cheat_GPS) > 0) view = true;
            if (!view && (Obj_LocalObjects.ViewCheck(pl.PosGet, Obj_LocalObjects.MyPlayer)))
            {
                if (!Obj_LocalObjects.HideCheck(pl.PosGet,pl.passc.charaScale_Multi))view = true;
            }
            if (!view && pl.passc.transmitter > 0)
            {
                var dis = Vector2.Distance(Obj_LocalObjects.MyPlayer.PosGet, pl.PosGet);
                if(dis <= pl.passc.transmitter)view = true;
            }
            if(!view && Obj_LocalObjects.MyPlayer.PassiveLvGet(Passive.Revenjer) > 0 && Obj_LocalObjects.MyPlayer.values.lastAtkKillPm == pl)
            {
                view = true;
            }
            if(!view)
            {
                po.gameObject.SetActive(false);
                continue;
            }
            po.gameObject.SetActive(true);
            Vector3 pos = cam.WorldToViewportPoint(pl.PosGet);
            po.rect.anchorMin = pos;
            po.rect.anchorMax = pos;
            po.rect.anchoredPosition = Vector2.zero;

            var col = Color.white;
            var size = 100f;
            if (Obj_LocalObjects.MyPlayer != null)
            {
                if (pl == Obj_LocalObjects.MyPlayer)
                {
                    col = UI_OptionManager.OptionGetColor("UI_Option 10", new Color(0.75f, 0.75f, 0, 1));
                    size = UI_OptionManager.OptionGetFloat("UI_Option 20", 100);
                }
                else if (pl.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID)
                {
                    col = UI_OptionManager.OptionGetColor("UI_Option 11", new Color(0, 0.75f, 0, 1));
                    size = UI_OptionManager.OptionGetFloat("UI_Option 21", 100);
                }
                else
                {
                    col = UI_OptionManager.OptionGetColor("UI_Option 12", new Color(0.75f, 0, 0, 1));
                    size = UI_OptionManager.OptionGetFloat("UI_Option 22", 100);
                }
            }
            else
            {
                col = Data_Base.TeamColorGet(pl.states.teamID);
                size = UI_OptionManager.OptionGetFloat("UI_Option 22", 100);
            }
            po.img.color = col;
            po.nameTx.text = pl.states.name;
            po.nameTx.color = pl.hpTotal > 0 ? Color.white : Color.red;
            po.transform.localScale = Vector3.one * size * 0.01f;
        }
        for (int i = 0; i < Mathf.Max(Obj_LocalObjects.Marks.Count, markPointers.Count); i++)
        {
            if (i >= Obj_LocalObjects.Marks.Count)
            {
                markPointers[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= markPointers.Count)
            {
                markPointers.Add(Instantiate(markPointers[0], markPointers[0].transform.parent));
            }
            var mk = Obj_LocalObjects.Marks[i];
            var po = markPointers[i];
            if(mk == null || mk.Object == null || Obj_LocalObjects.MyPlayer == null || mk.pm.states.teamID != Obj_LocalObjects.MyPlayer.states.teamID)
            {
                po.gameObject.SetActive(false);
                continue;
            }
            po.gameObject.SetActive(true);

            var pos = cam.WorldToViewportPoint(mk.transform.position);
            po.rect.anchorMin = pos;
            po.rect.anchorMax = pos;
            po.rect.anchoredPosition = Vector2.zero;

            var col = Color.white;
            var size = 100f;
            if (mk.pm == Obj_LocalObjects.MyPlayer)
            {
                col = UI_OptionManager.OptionGetColor("UI_Option 05",new Color(0.75f,0.75f,0,0.5f));
                size = UI_OptionManager.OptionGetFloat("UI_Option 23", 100);
            }
            else
            {
                col = UI_OptionManager.OptionGetColor("UI_Option 06", new Color(0, 0.75f, 0, 0.5f));
                size = UI_OptionManager.OptionGetFloat("UI_Option 24", 100);
            } 

            po.img.color = col;
            po.nameTx.text = mk.pm.states.name;
            po.transform.localScale = Vector3.one * size * 0.01f;
        }
        for(int i=0;i<Mathf.Max(Obj_LocalObjects.Areas.Count,areaPointers.Count);i++)
        {
            if(i >= Obj_LocalObjects.Areas.Count)
            {
                areaPointers[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= areaPointers.Count)areaPointers.Add(Instantiate(areaPointers[0], areaPointers[0].transform.parent));
            var ar = Obj_LocalObjects.Areas[i];
            var po = areaPointers[i];
            if (!Net_Value.NetCheck || !Net_Value.NetValue.options[3])
            {
                po.gameObject.SetActive(false);
                continue;
            }
            po.gameObject.SetActive(true);
            var pos = cam.WorldToViewportPoint(ar.transform.position);
            po.rect.anchorMin = pos;
            po.rect.anchorMax = pos;
            po.rect.anchoredPosition = Vector2.zero;

            var size = UI_OptionManager.OptionGetFloat("UI_Option 38",150);
            var colc = Data_Base.TeamColorGet(ar.nowTeam);
            var colt = Data_Base.TeamColorGet(ar.chTeam);
            var alpha = UI_OptionManager.OptionGetFloat("UI_Option 39", 50) * 0.01f;
            colc.a = alpha;
            colt.a = alpha;

            po.img.color = colc;
            po.nameTx.text = LocalizSystem.LocailzSCInfo("エリア") + ar.areaName + "\n" + (ar.areaTime / ar.areaMaxTime * 100).ToString("F0") + "%";
            po.nameTx.color = colt;
            po.transform.localScale = Vector3.one * size * 0.01f;
        }
        for (int i = 0; i < Mathf.Max(Obj_LocalObjects.Flags.Count, flagPointers.Count); i++)
        {
            if (i >= Obj_LocalObjects.Flags.Count)
            {
                flagPointers[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= flagPointers.Count)flagPointers.Add(Instantiate(flagPointers[0], flagPointers[0].transform.parent));
            var fg = Obj_LocalObjects.Flags[i];
            var po = flagPointers[i];
            if (!fg.flagUse)
            {
                po.gameObject.SetActive(false);
                continue;
            }
            po.gameObject.SetActive(true);
            var pos = cam.WorldToViewportPoint(fg.transform.position);
            po.rect.anchorMin = pos;
            po.rect.anchorMax = pos;
            po.rect.anchoredPosition = Vector2.zero;

            var size = UI_OptionManager.OptionGetFloat("UI_Option 40", 150);
            var col = Data_Base.TeamColorGet(fg.sarea.teamID);
            col.a = UI_OptionManager.OptionGetFloat("UI_Option 41", 50) * 0.01f;

            po.img.color = col;
            po.nameTx.text = "";
            po.transform.localScale = Vector3.one * size * 0.01f;
            po.transform.rotation = fg.transform.rotation;
        }
    }
}
