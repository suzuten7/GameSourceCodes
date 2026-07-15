using System.Collections.Generic;
using UnityEngine;

public class Obj_LocalObjects : MonoBehaviour
{
    static Obj_LocalObjects localObjs;
    public static Obj_LocalObjects LocalObjects => localObjs;
    [SerializeField] float clearCT;

    public Player_JoinObj myJoin;
    static public Player_JoinObj MyJoin
    {
        get{return localObjs.myJoin;}
        set{localObjs.myJoin = value;}
    }
    public List<Player_JoinObj> joinPls;
    static public List<Player_JoinObj> JoinPls => localObjs.joinPls;
    public Player_ViewMode viewObj;
    static public Player_ViewMode ViewObj
    {
        get { return localObjs.viewObj; }
        set { localObjs.viewObj = value; }
    }
    public Player_Manager myPlayer;
    static public Player_Manager MyPlayer
    {
        get { return localObjs.myPlayer; }
        set { localObjs.myPlayer = value; }
    }
    public List<Player_Manager> players;
    static public List<Player_Manager> Players => localObjs.players;
    public List<Bullet_Hit> bullets;
    static public List<Bullet_Hit> Bullets => localObjs.bullets;
    public List<GG_Base> gadgets;
    static public List<GG_Base> Gadgets => localObjs.gadgets;
    public List<Ult_Base> ults;
    static public List<Ult_Base> Ults => localObjs.ults;
    public List<Obj_Wall> walls;
    static public List<Obj_Wall> Walls => localObjs.walls;
    public List<Obj_Sound> sounds;
    static public List<Obj_Sound> Sounds => localObjs.sounds;
    public List<Player_Marker> marks;
    static public List<Player_Marker> Marks => localObjs.marks;
    float ctime;
    public List<SpotMask> viewMasks;
    static public List<SpotMask> ViewMasks => localObjs.viewMasks;
    public List<Ult_TimeStop> timeStops;
    static public List<Ult_TimeStop> TimeStops => localObjs.timeStops;
    public List<Obj_Dragger> draggers;
    static public List<Obj_Dragger> Draggers => localObjs.draggers;

    public List<Obj_SpawneArea> spawnes;
    static public List<Obj_SpawneArea> Spawnes => localObjs.spawnes;

    public List<Obj_ScoreArea> areas;
    static public List<Obj_ScoreArea> Areas => localObjs.areas;

    public List<Obj_Flag> flags;
    static public List<Obj_Flag> Flags => localObjs.flags;

    public List<GameObject> uis;
    static public void UIAdd(GameObject uiobj)
    {
        if(LocalObjects != null)LocalObjects.uis.Add(uiobj);
    }
    static public bool UIOpenCheck
    {
        get
        {
            var check = false;
            if (LocalObjects != null)
                for (int i = 0; i < LocalObjects.uis.Count; i++)
                {
                    var ui = LocalObjects.uis[i];
                    if (ui == null) continue;
                    if (!ui.activeInHierarchy) continue;
                    check = true;
                    break;
                }
            return check;
        }
    }

    static public bool ViewCheck(Vector2 pos,Player_Manager pm)
    {
        var view = false;
        var cvp = pm.objects.camera.WorldToViewportPoint(pos);
        if (cvp.x < 0 || cvp.x > 1 || cvp.y < 0 || cvp.y > 1) return false;
        if (pm.PassiveLvGet(Passive.Cheat_View) > 0 || pm.BufGet(BufType.WallHack)) view = true;
        if (!view)
            for (int i = 0; i < Players.Count; i++)
            {
                var pl = Players[i];
                if (pl == null) continue;
                var dir = pos - (Vector2)pl.PosGet;
                var check = false;
                var dmUse = pl.states.teamID == pm.states.teamID;
                var dm = 2f;
                if(pl.PassiveLvGet(Passive.Lantan) > 0)
                {
                    dm += pl.PassiveValGet(Passive.Lantan);
                    dmUse = true;
                }
                if (pm == pl)
                {
                    if (pl.PassiveLvGet(Passive.Blind) > 0) dmUse = false;
                    var rot = Vector2.Angle(pl.TransGet.up, dir) * 2f;
                    if (!pl.BufGet(BufType.Darkness) && rot <= pl.states.vision_Angle) check = true;
                }
                if (pl.values.killLight)
                {
                    dm = Mathf.Max(dm, 4);
                    dmUse = true;
                }
                if (pl.BufGet(BufType.Fire))
                {
                    dm = Mathf.Max(dm, Data_Base.BufDGet(BufType.Fire).values[1]);
                    dmUse = true;
                }
                if (pl.BufGet(BufType.SpotLit))
                {
                    dm = Mathf.Max(dm, Data_Base.BufDGet(BufType.SpotLit).values[1]);
                    dmUse = true;
                }
                var dis = Vector2.Distance(pos, pl.PosGet);
                if (dmUse && dis <= dm) check = true;
                if (!check) continue;
                var hit = Physics2D.Raycast(pl.PosGet, dir, dis, LayerMask.GetMask("Wall"));
                if (hit.collider != null) continue;
                view = true;
            }
        if (!view)
        for (int i = 0; i < ViewMasks.Count; i++)
        {
            var vm = ViewMasks[i];
            if (vm == null) continue;
            if (!vm.gameObject.activeInHierarchy) continue;
            var dir = pos - (Vector2)vm.transform.position;
            var rot = Vector2.Angle(vm.transform.right, dir) * 2f;
            if (rot > vm.viewAngle) continue;
            var dis = Vector2.Distance(vm.transform.position, pos);
            if (dis > vm.viewDistance) continue;
            var hit = Physics2D.Raycast(vm.transform.position, dir, dis, vm.wallLayer);
            if (hit.collider != null) continue;
            view = true;
        }
        return view;
    }
    static public bool HideCheck(Vector2 pos,float disMult = 1f,int count = 8)
    {
        var hidec = 0;
        for (int k = 0; k < count; k++)
        {
            var rot = 360f / count * k * Mathf.Deg2Rad;
            var off = new Vector2(Mathf.Sin(rot), Mathf.Cos(rot));
            var p = pos + off * 0.3f * disMult;
            bool hid = false;
            foreach (var rayhit in Physics2D.OverlapCircleAll(p, 0.01f, LayerMask.GetMask("Hide")))
            {
                hid = true;
                break;
            }
            //Debug.DrawLine(p - Vector2.up * 0.05f, p + Vector2.up * 0.05f, hid ? Color.red : Color.green);
            //Debug.DrawLine(p - Vector2.right * 0.05f, p + Vector2.right * 0.05f, hid ? Color.red : Color.green);
            if (hid) hidec++;
        }
        return hidec >= count;
    }

    bool timeStop = false;
    static public bool TimeStopd => localObjs.timeStop;

    static public bool[] TeamUsed => localObjs.teamUsed;
    public bool[] teamUsed = new bool[5];
    private void Start()
    {
        localObjs = this;
    }
    void Update()
    {
        ctime += Time.deltaTime;
        if (ctime >= clearCT)
        {
            ctime = 0;
            joinPls.RemoveAll(x => x == null);
            players.RemoveAll(x => x == null);
            bullets.RemoveAll(x => x == null);
            gadgets.RemoveAll(x => x == null);
            ults.RemoveAll(x => x == null);
            walls.RemoveAll(x => x == null);
            sounds.RemoveAll(x => x == null);
            viewMasks.RemoveAll(x => x == null);
            uis.RemoveAll(x => x == null);
            timeStops.RemoveAll(x => x == null);
            draggers.RemoveAll(x => x == null);
            spawnes.RemoveAll(x => x == null);
            areas.RemoveAll(x => x == null);
            flags.RemoveAll(x => x == null);
        }
        for (int i = 0; i < teamUsed.Length; i++) teamUsed[i] = false;
        for(int i = 0; i < players.Count; i++)
        {
            var p = players[i];
            teamUsed[p.states.teamID] = true;
        }
        timeStop = false;
        for(int i = 0; i < timeStops.Count; i++)
        {
            var ts = timeStops[i];
            if (ts == null) continue;
            if (ts.Object == null) continue;
            if (ts.pm == null) continue;
            if (ts.pm.hpTotal <= 0 && ts.pm.PassiveLvGet(Passive.Undeath) < 0) continue;
            timeStop = true;
            break;
        }
    }
}
