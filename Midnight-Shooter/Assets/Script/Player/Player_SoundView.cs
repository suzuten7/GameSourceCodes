using System.Collections.Generic;
using UnityEngine;

public class Player_SoundView : MonoBehaviour
{
    [SerializeField] Player_Manager pm;
    [SerializeField] List<GameObject> soundViews;

    void Update()
    {
        for(int i = 0; i < Mathf.Max(Obj_LocalObjects.Sounds.Count,soundViews.Count); i++)
        {
            if(i >= Obj_LocalObjects.Sounds.Count)
            {
                soundViews[i].SetActive(false);
                continue;
            }
            if(i >= soundViews.Count)
            {
                soundViews.Add(Instantiate(soundViews[0], soundViews[0].transform.parent));
            }
            var so = Obj_LocalObjects.Sounds[i];
            var sv = soundViews[i];
            if(so == null || so.my)
            {
                sv.gameObject.SetActive(false);
                continue;
            }
            var dis = Vector2.Distance(pm.PosGet, so.transform.position);
            var range = so.rangeMax*pm.passc.soundGet_Multi;
            if(dis > range)
            {
                sv.gameObject.SetActive(false);
                continue;
            }
            else sv.gameObject.SetActive(true);
            var alpha = Mathf.Clamp01(Mathf.Lerp(1, 0.3f, dis / range) * so.AlphaGet * 1.5f);
            var sr = sv.GetComponentInChildren<SpriteRenderer>();
            var color = so.userTeam == pm.states.teamID ?
                UI_OptionManager.OptionGetColor("UI_Option 26", new Color(0,0,1,0.5f)):
                UI_OptionManager.OptionGetColor("UI_Option 27", Color.red);
            color.a *= alpha;
            sr.color = color;

            var vect = so.transform.position - pm.PosGet;
            sv.transform.position = pm.PosGet + vect.normalized * UI_OptionManager.OptionGetFloat("UI_Option 30", 3);
            var rot = Mathf.Atan2(vect.y,vect.x) * Mathf.Rad2Deg;
            sv.transform.rotation = Quaternion.Euler(0, 0, rot - 90);

            var size = so.userTeam == pm.states.teamID ?
                UI_OptionManager.OptionGetFloat("UI_Option 28", 100) :
                UI_OptionManager.OptionGetFloat("UI_Option 29", 100);
            sv.transform.localScale = Vector3.one * size * 0.01f;
        }
    }
}
