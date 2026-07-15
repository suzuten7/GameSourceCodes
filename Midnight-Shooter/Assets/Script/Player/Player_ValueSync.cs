using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
public class Player_ValueSync : NetworkBehaviour
{
    [SerializeField]Player_Manager pm;
    [SerializeField]float syncCT;
    #region 同期用変数
    string back_name = "";
    [Networked] NetworkString<_64> net_name { get; set; }
    int back_team = -1;
    [Networked] int net_team { get; set; }
    int back_charaImg = -1;
    [Networked] int net_charaImg { get; set; }
    public int loadImg_hash;
    static public Dictionary<int, UI_ImageLoader_Base.ImgBase> loadImg_dics = new();
    static Dictionary<int,bool> loadImg_waits = new();
    int back_loadImg_hash;
    int back_loadImg_h;
    [Networked]int net_loadImg_hash { get; set; }
    float back_hpMax = -1;
    [Networked] float net_hpMax { get; set; }
    float back_hpNow = -1;
    [Networked] float net_hpNow { get; set; }
    float back_hpOver = -1;
    [Networked] float net_hpOver { get; set; }
    float back_hpShild = -1;
    [Networked] float net_hpShild { get; set; }
    float back_hpInjury = -1;
    [Networked] float net_hpInjury { get; set; }
    int back_kill = -1;
    [Networked] int net_kill { get; set; }
    int back_killcons = -1;
    [Networked] int net_killcons { get; set; }
    bool back_killLight = false;
    [Networked] bool net_killLight { get; set; }
    int back_killcmax = -1;
    [Networked] int net_killcmax { get; set; }
    float back_score = -1;
    [Networked] float net_score { get; set; }
    int back_kill_b = -1;
    [Networked] int net_kill_b { get; set; }
    int back_killcmax_b = -1;
    [Networked] int net_killcmax_b { get; set; }
    float back_score_b = -1;
    [Networked] float net_score_b { get; set; }
    int back_team_b = -1;
    [Networked] int net_team_b { get; set; }

    float back_addDamages_b = -1;
    [Networked] float net_addDamages_b { get; set; }
    float back_takeDamages_b = -1;
    [Networked] float net_takeDamages_b { get; set; }
    float back_addHeals_b = -1;
    [Networked] float net_addHeals_b { get; set; }

    int back_cusol;
    [Networked] int net_cusol { get; set; }
    int back_gunID;
    [Networked] int net_gunID { get; set; }
    int back_meleeID;
    [Networked] int net_meleeID { get; set; }

    int back_moveID;
    [Networked] int net_moveID { get; set; }
    int back_atkAnimID;
    [Networked] int net_atkAnimID { get; set; }

    bool back_noView = false;
    [Networked] bool net_noView { get; set; }
    float back_noDamTime;
    [Networked] float net_noDamTime { get; set; }

    [Networked,Capacity(100)]NetworkLinkedList<Vector2Int> net_pass => default;
    List<Vector2Int> back_pass = new();
    [Networked, Capacity(20)] NetworkLinkedList<BufSet> net_bufs => default;
    List<BufSet> back_bufs = new();
    #endregion
    float sct = 0;
    Vector3 rpos;
    private void Update()
    {
        if (Net_Connect.NoOwnerCheck(Object) && HasStateAuthority)
        {
            Runner.Despawn(Object);
            return;
        }
        if (Net_Connect.InsRunner.GameMode != GameMode.Single)
        {
            if (Net_Connect.CanControl(Object))
            {
                NetSyncs_Owner();
            }
            else NetSyncs_Other();
        }

        gameObject.name = "Player" + pm.states.name;
    }

    enum SyncType
    {
        Name,
        Team,
        CharaImg,
        LoadImg,
        HP_Max,
        HP_Now,
        HP_Over,
        HP_Shild,
        HP_Injury,

        Kill,
        Killcons,
        KillLight,
        Killcmax,
        Score,

        Team_b,
        Kill_b,
        Killcmax_b,
        Score_b,

        AddDamages_b,
        TakeDamages_b,
        AddHeals_b,

        Cusol,
        GunID,
        MelleID,

        MoveID,
        AtkAnimID,

        NoView,
        NoDamTime,
    }
    void NetSyncs_Owner()
    {
        sct -= Time.unscaledDeltaTime;
        if (sct > 0)return;
        sct = syncCT;
        SyncSet(SyncType.Name, pm.states.name,ref back_name);
        SyncSet(SyncType.Team, pm.states.teamID,ref back_team);
        SyncSet(SyncType.CharaImg, pm.states.charaImgID, ref back_charaImg);

        SyncSet(SyncType.HP_Max, pm.states.max_HP,ref back_hpMax);
        SyncSet(SyncType.HP_Now, pm.values.hpNow,ref back_hpNow);
        SyncSet(SyncType.HP_Over, pm.values.hpOver,ref back_hpOver);
        SyncSet(SyncType.HP_Shild, pm.values.hpShild, ref back_hpShild);
        SyncSet(SyncType.HP_Injury, pm.values.hpInjury,ref back_hpInjury);

        SyncSet(SyncType.Kill, pm.values.kill, ref back_kill);
        SyncSet(SyncType.Killcons, pm.values.killcons, ref back_killcons);
        SyncSet(SyncType.KillLight, pm.values.killLight, ref back_killLight);
        SyncSet(SyncType.Killcmax, pm.values.killcmax, ref back_killcmax);
        SyncSet(SyncType.Score, pm.values.score, ref back_score);

        SyncSet(SyncType.Team_b, pm.values.team_back, ref back_team_b);
        SyncSet(SyncType.Kill_b, pm.values.kill_back, ref back_kill_b);
        SyncSet(SyncType.Killcmax_b, pm.values.killcmax_back, ref back_killcmax_b);
        SyncSet(SyncType.Score_b, pm.values.score_back, ref back_score_b);

        SyncSet(SyncType.AddDamages_b, pm.values.addDamages_back, ref back_addDamages_b);
        SyncSet(SyncType.TakeDamages_b, pm.values.takeDamages_back, ref back_takeDamages_b);
        SyncSet(SyncType.AddHeals_b, pm.values.addHeals_back, ref back_addHeals_b);

        SyncSet(SyncType.Cusol, (int)pm.values.now_CursorState, ref back_cusol);
        SyncSet(SyncType.GunID, pm.states.gun_IndexNum, ref back_gunID);
        SyncSet(SyncType.MelleID, pm.states.melee_IndexNum, ref back_meleeID);

        SyncSet(SyncType.MoveID, (int)pm.values.now_MoveState, ref back_moveID);
        SyncSet(SyncType.AtkAnimID, pm.values.atkAnimID, ref back_atkAnimID);

        SyncSet(SyncType.NoView, pm.values.noView,ref back_noView);
        SyncSet(SyncType.NoDamTime, pm.values.noDamTime, ref back_noDamTime);
        //パッシブ同期
        var pas = pm.states.passives;
        bool pcheck = pas.Count != back_pass.Count;
        if(!pcheck)
        for (int i = 0; i < pas.Count; i++)
        {
                if (back_pass[i] != pas[i])
                {
                    pcheck = true;
                    break;
                }
        }
        if (pcheck)
        {
            back_pass = pm.states.passives.ToList();
            RPC_SyncPassList(pm.states.passives.ToArray());
        }
        //バフ同期
        var bufs = pm.values.bufs;
        bool bcheck = bufs.Count != back_bufs.Count;
        if (!bcheck)
            for (int i = 0; i < bufs.Count; i++)
            {
                if (back_bufs[i].buf != bufs[i].buf)
                {
                    bcheck = true;
                    break;
                }
            }
        if (bcheck)
        {
            back_bufs = pm.values.bufs.ToList();
            RPC_SyncBufList(pm.values.bufs.ToArray());
        }
        //カスタム外見同期
        if (UI_OptionManager.OptionGetOnOff("GP_Option 11", false))
        {
            _ = FBSet_Img2D_Save();
        }
        else
        {
            loadImg_hash = 0;
            back_loadImg_hash = 0;
        }
        SyncSet(SyncType.LoadImg, loadImg_hash, ref back_loadImg_h);
    }
    void NetSyncs_Other()
    {
        if (Object == null) return;
        pm.states.name = net_name.Value;
        pm.states.teamID = net_team;
        pm.states.charaImgID = net_charaImg;

        pm.states.max_HP = net_hpMax;
        pm.values.hpNow = net_hpNow;
        pm.values.hpOver = net_hpOver;
        pm.values.hpShild = net_hpShild;
        pm.values.hpInjury = net_hpInjury;

        pm.values.kill = net_kill;
        pm.values.killcons = net_killcons;
        pm.values.killLight = net_killLight;
        pm.values.killcmax = net_killcmax;
        pm.values.score = net_score;

        pm.values.team_back = net_team_b;
        pm.values.kill_back = net_kill_b;
        pm.values.killcmax_back = net_killcmax_b;
        pm.values.score_back = net_score_b;

        pm.values.addDamages_back = net_addDamages_b;
        pm.values.takeDamages_back = net_takeDamages_b;
        pm.values.addHeals_back = net_addHeals_b;


        pm.values.now_CursorState = (CursorState)net_cusol;
        pm.states.gun_IndexNum = net_gunID;
        pm.states.melee_IndexNum = net_meleeID;

        pm.values.now_MoveState = (MoveState)net_moveID;
        pm.values.atkAnimID = net_atkAnimID;

        pm.values.noView = net_noView;
        pm.values.noDamTime = net_noDamTime;
        pm.states.passives = net_pass.ToList();
        pm.values.bufs = net_bufs.ToList();


        //カスタム外見同期
        if (UI_OptionManager.OptionGetOnOff("GP_Option 11", false))
        {
            loadImg_hash = net_loadImg_hash;
            if (loadImg_hash != 0) _ = FBSet_Img2D_Load();
        }
        else
        {
            loadImg_hash = 0;
            back_loadImg_hash = 0;
        }
    }
    void SyncSet(int type, System.Object val)
    {
        switch ((SyncType)type)
        {
            case SyncType.Name: net_name = (string)val; break;
            case SyncType.Team: net_team = (int)val; break;
            case SyncType.CharaImg:net_charaImg = (int)val; break;
            case SyncType.LoadImg:net_loadImg_hash = (int)val;break;

            case SyncType.HP_Max: net_hpMax = (float)val; break;
            case SyncType.HP_Now: net_hpNow = (float)val; break;
            case SyncType.HP_Over: net_hpOver = (float)val; break;
            case SyncType.HP_Shild: net_hpShild = (float)val; break;
            case SyncType.HP_Injury: net_hpInjury = (float)val; break;

            case SyncType.Kill: net_kill = (int)val; break;
            case SyncType.Killcons: net_killcons = (int)val; break;
            case SyncType.KillLight:net_killLight = (bool)val;break;
            case SyncType.Killcmax: net_killcmax = (int)val; break;
            case SyncType.Score: net_score = (float)val; break;

            case SyncType.Team_b: net_team_b = (int)val; break;
            case SyncType.Kill_b: net_kill_b = (int)val; break;
            case SyncType.Killcmax_b: net_killcmax_b = (int)val; break;
            case SyncType.Score_b: net_score_b = (float)val; break;

            case SyncType.AddDamages_b:net_addDamages_b = (float)val;break;
            case SyncType.TakeDamages_b: net_takeDamages_b = (float)val; break;
            case SyncType.AddHeals_b: net_addHeals_b = (float)val; break;

            case SyncType.Cusol:net_cusol = (int)val; break;
            case SyncType.GunID:net_gunID = (int)val; break;
            case SyncType.MelleID:net_meleeID = (int)val; break;

            case SyncType.MoveID: net_moveID = (int)val; break;
            case SyncType.AtkAnimID:net_atkAnimID = (int)val; break;

            case SyncType.NoView: net_noView = (bool)val; break;
            case SyncType.NoDamTime: net_noDamTime = (float)val; break;
        }
    }

    void SyncSet(SyncType type,int cvalue,ref int bvalue)
    {
        if (bvalue == cvalue) return;
        bvalue = cvalue;
        RPC_Sync((int)type, cvalue);
    }
    void SyncSet(SyncType type, float cvalue, ref float bvalue)
    {
        if (bvalue == cvalue) return;
        bvalue = cvalue;
        RPC_Sync((int)type, cvalue);
    }
    void SyncSet(SyncType type, bool cvalue, ref bool bvalue)
    {
        if (bvalue == cvalue) return;
        bvalue = cvalue;
        RPC_Sync((int)type, cvalue);
    }
    void SyncSet(SyncType type, string cvalue, ref string bvalue)
    {
        if (bvalue == cvalue) return;
        bvalue = cvalue;
        RPC_Sync((int)type, cvalue);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Sync(int type, int val)
    {
        SyncSet(type, val);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Sync(int type, bool val)
    {
        SyncSet(type, val);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Sync(int type, float val)
    {
        SyncSet(type, val);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Sync(int type, NetworkString<_64> val)
    {
        SyncSet(type, val.Value);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SyncPassList(Vector2Int[] val)
    {
        net_pass.Clear();
        for (int i = 0; i < val.Length; i++)net_pass.Add(val[i]);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SyncBufList(BufSet[] val)
    {
        net_bufs.Clear();
        for (int i = 0; i < val.Length; i++) net_bufs.Add(val[i]);
    }
    async Task FBSet_Img2D_Save()
    {
        if(pm.states.loadImgID <= 0)
        {
            loadImg_hash = 0;
            back_loadImg_hash = 0;
            return;
        }
        var set = UI_ImageLoader_Base.ImgSets[pm.states.loadImgID - 1];
        var hash = Library_FireBase.HashGet(set);
        if (back_loadImg_hash == hash)
        {
            Debug.Log("2D同データ");
            return;
        }
        back_loadImg_hash = hash;
        if (loadImg_dics.ContainsKey(hash))
        {
            loadImg_hash = hash;
            Debug.Log("2D存在済み");
            return;
        }
        if (loadImg_waits.ContainsKey(hash))
        {
            Debug.Log("2D送信待ち");
            return;
        }
        loadImg_waits.TryAdd(hash, true);
        var loadDataSnap = await Library_FireBase.FireBaseLoadSnap("Img2Ds", hash.ToString());
        if(loadDataSnap != null)
        {
            loadImg_hash = hash;
            var cpj = JsonUtility.ToJson(set);
            var cpd = JsonUtility.FromJson<UI_ImageLoader_Base.ImgBase>(cpj);
            loadImg_dics.TryAdd(hash, cpd);
            loadImg_waits.Remove(hash);
            Debug.Log("2D存在済み");
            return;
        }

        Debug.Log("2D送信開始");
        try
        {
            var DataValues = new Dictionary<string, object>()
                {
                    {"Name", set.name},
                    {"Imgs_Count", set.datas.Count},
                    {"Speeds",set.speeds.ToArray()}
                };
            for (int i = 0; i < set.datas.Count; i++)
            {
                DataValues.Add("Imgs_" + i.ToString("D3"), JsonUtility.ToJson(set.datas[i]));
            }
            await Library_FireBase.FireBaseSave("Img2Ds", hash.ToString(), DataValues);
            loadImg_hash = hash;

            var cpj = JsonUtility.ToJson(set);
            var cpd = JsonUtility.FromJson<UI_ImageLoader_Base.ImgBase>(cpj);
            loadImg_dics.TryAdd(hash, cpd);
            loadImg_waits.Remove(hash);
            Debug.Log("2D送信完了");
        }
        catch
        {
            Debug.Log("2D送信失敗");
        }
    }
    async Task FBSet_Img2D_Load()
    {
        var hash = loadImg_hash;
        if (back_loadImg_hash == hash)
        {
            Debug.Log("2D最新");
            return;
        }
        back_loadImg_hash = hash;
        if (loadImg_dics.ContainsKey(hash))
        {
            Debug.Log("2D存在済み");
            return;
        }
        var loadDataSnap = await Library_FireBase.FireBaseLoadSnap("Img2Ds", hash.ToString());
        if (loadDataSnap == null)
        {
            Debug.Log("2Dデータがありません");
            return;
        }
        Debug.Log("2D受信開始");
        try
        {
            
            var name = Library_FireBase.FireBaseFiledGet(loadDataSnap, "Name", "");
            var imgCount = Library_FireBase.FireBaseFiledGet(loadDataSnap, "Imgs_Count", 0);
            var datas = new List<UI_ImageLoader_Base.ImgData>();
            for (int i = 0; i < imgCount; i++)
            {
                var img = Library_FireBase.FireBaseFiledGet(loadDataSnap, "Imgs_" + i.ToString("D3"), "");
                var modelImg = JsonUtility.FromJson<UI_ImageLoader_Base.ImgData>(img);
                datas.Add(modelImg);
            }
            var loadImg = new UI_ImageLoader_Base.ImgBase();
            loadImg.datas = datas;
            var spd = Library_FireBase.FireBaseFiledGet(loadDataSnap, "Speeds", new float[0]);
            loadImg.speeds.Clear();
            for (int i = 0; i <= (int)Player_ImgAnims.AnimType.Death; i++)
            {
                loadImg.speeds.Add(spd.Length > i ? spd[i] : 1f);
            }
            loadImg_dics.TryAdd(hash, loadImg);
            Debug.Log("2D受信完了");
        }
        catch
        {
            Debug.Log("2D受信失敗");
        }
    }
}
