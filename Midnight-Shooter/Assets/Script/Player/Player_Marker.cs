using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class Player_Marker : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [Networked]public Player_Manager pm { get; set; }
    void Start()
    {
        Obj_LocalObjects.Marks.Add(this);
        Obj_LocalObjects.Marks.RemoveAll(x => x == null);
        sr.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (pm != null)
        {
            var team = pm.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID;
            sr.gameObject.SetActive(team);
            if (team)
            {
                var col =Color.white;
                if (Obj_LocalObjects.MyPlayer == null)
                {
                    col = Data_Base.TeamColorGet(pm.states.teamID);
                    col.a = 0.5f;
                }
                else if (pm == Obj_LocalObjects.MyPlayer) col = UI_OptionManager.OptionGetColor("UI_Option 05", new Color(0.75f, 0.75f, 0, 0.75f));
                else col = UI_OptionManager.OptionGetColor("UI_Option 06", new Color(0, 0.75f, 0, 0.5f));
                sr.color = col;
            }
        }
        if (!Net_Connect.CanControl(Object)) return;
        if (pm == null) Destroy(gameObject);
        
    }
}
