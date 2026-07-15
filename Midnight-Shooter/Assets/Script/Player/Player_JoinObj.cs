using Fusion;
using UnityEngine;
public class Player_JoinObj : NetworkBehaviour
{
    public string name;
    [Networked]NetworkString<_64> net_name { get; set; }
    string bname = "";
    private void Start()
    {
        Obj_LocalObjects.JoinPls.Add(this);
        if (Net_Connect.CanControl(Object))
        {
            Obj_LocalObjects.MyJoin = this;
            name = Net_Connect.PlayerName;
            Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.PlayerIO, "プレイヤー参加", name);
        }
    }
    void Update()
    {
        if (Net_Connect.NoOwnerCheck(Object) && HasStateAuthority)
        {
            Debug.Log(name + "切断");
            Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.PlayerIO, "プレイヤー退室", name);
            Runner.Despawn(Object);
            return;
        }
        if (Net_Connect.CanControl(Object))
        {
            if (bname != name)
            {
                bname = name;
                RPC_NameSet(name);
            }
        }
        else
        {
            name = net_name.Value;
        }
    }
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    void RPC_NameSet(NetworkString<_64> nstr)
    {
        net_name = nstr;
    }
}
