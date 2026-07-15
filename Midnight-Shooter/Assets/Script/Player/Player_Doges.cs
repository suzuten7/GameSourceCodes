using Fusion;
using UnityEngine;

public class Player_Doges : NetworkBehaviour
{
    public Player_Manager pm;
    [SerializeField] SpriteRenderer dogeSr;
    [SerializeField] LayerMask checkLayer;
    [SerializeField] float dogeTime;
    [SerializeField] float dogeRange;

    int c = -1;
    float doget = 999;
    float dogeAddCT = 0;
    float dsct = 0;
    void Update()
    {
        dogeAddCT -=Time.deltaTime;
        if (!Net_Connect.CanControl(Object))
        {
            dogeSr.gameObject.SetActive(false);
            return;
        }
        dogeSr.gameObject.SetActive(pm == Obj_LocalObjects.MyPlayer);
        var drange = dogeRange * pm.passc.dogeScale_Multi + pm.passc.charaScale_Multi;
        transform.localScale = Vector3.one * drange * 2f;
        if (pm.hpTotal <= 0 || pm.values.noDamTime > 0 || pm.values.hpRegeneDelay < pm.states.weit_RegeneDelay)
        {
            dogeSr.color = UI_OptionManager.OptionGetColor("UI_Option 37",new Color(1,0,1,0.5f));
            return;
        }
        if (pm.StopMove)
        {
            doget += Time.deltaTime;
            if (doget < dogeTime)
            {
                pm.UltCharge(Data_Base.DB.ultCharge[3] * Time.deltaTime * pm.passc.ultChargeDoge_Multi);
                if (pm.PassiveLvGet(Passive.DogeScore) > 0)
                {
                    dsct -= Time.deltaTime;
                    if (dsct <= 0)
                    {
                        dsct = 0.5f;
                        pm.ScoreChange(pm.PassiveValGet(Passive.DogeScore, 0) * 0.5f * Net_Value.NetValue.scoreMults[0]);
                    }
                }
            }
        }
        Color dcolor;
        if (doget < dogeTime) dcolor = UI_OptionManager.OptionGetColor("UI_Option 36", Color.white);
        else dcolor = UI_OptionManager.OptionGetColor("UI_Option 35", new Color(1, 1, 1, 0.5f));
        dogeSr.color = dcolor;
    }
    public void DogeAdd()
    {
        if (dogeAddCT > 0) return;
        dogeAddCT = 1;
        RPC_DogeAdd();
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    void RPC_DogeAdd()
    {
        doget = 0;
    }
}
