using Fusion;
using UnityEngine;

public class Net_PlayerSet : NetworkBehaviour
{
    static Net_PlayerSet playerSet;
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject viewObj;
    [SerializeField] GameObject joinPlObj;
    static public GameObject view;
    float cct = 0.5f;
    private void Start()
    {
        playerSet = this;
    }
    private void Update()
    {
        if (view != null)
        {
            if (Obj_LocalObjects.MyPlayer != null) Destroy(view);
        }
        else
        {
            if (Obj_LocalObjects.MyPlayer == null) view = Instantiate(viewObj);
        }
        cct -=Time.deltaTime;
        if (Net_Connect.InsRunner != null && cct <= 0)
        {
            cct = 1f;
            if (JoinSetCheck(Net_Connect.InsRunner.LocalPlayer)) return;
            RPC_JoinSet(Net_Connect.InsRunner.LocalPlayer);
        }
    }
    static public void PlayerSet()
    {
        playerSet.RPC_PlayerSet(Net_Connect.InsRunner.LocalPlayer);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_PlayerSet(PlayerRef player)
    {
        Debug.Log("プレイヤーキャラ配置" + player);
        var check = true;
        for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
        {
            var pl = Obj_LocalObjects.Players[i];
            if (pl == null) continue;
            if (pl.Object == null) continue;
            if (pl.Object.InputAuthority == player)
            {
                check = false;
                break;
            }
        }
        if (!check)
        {
            Debug.Log("プレイヤーキャラ配置済み" + player);
            return;
        }
        if (!Net_Connect.InsRunner.IsClient)
        {
            Debug.Log($"Spawn player for {player}");
            var pos = Vector3.zero;
            // プレイヤーを生成
            var rot = Quaternion.identity;
            var pobj = Net_Connect.InsRunner.Spawn(
                playerObj,
                pos,
                rot,
                player, // 所有者
                onBeforeSpawned: (runner, obj) =>
                {

                });
            Net_Connect.InsRunner.SetPlayerObject(player, pobj);
        }
        Debug.Log("プレイヤーキャラ配置完了" + player);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_JoinSet(PlayerRef player)
    {
        Debug.Log("プレイヤー接続用配置" + player);
        if (JoinSetCheck(player))
        {
            Debug.Log("プレイヤー接続用配置済み" + player);
            return;
        }
        var pobj = Net_Connect.InsRunner.Spawn(
        joinPlObj,
        Vector2.zero,
        Quaternion.identity,
        player, // 所有者
        onBeforeSpawned: (runner, obj) => { });
        Net_Connect.InsRunner.SetPlayerObject(player, pobj);
        Debug.Log("プレイヤー接続用配置完了" + player);
    }
    bool JoinSetCheck(PlayerRef player)
    {
        for (int i = 0; i < Obj_LocalObjects.JoinPls.Count; i++)
        {
            var pl = Obj_LocalObjects.JoinPls[i];
            if (pl == null) continue;
            if (pl.Object == null) continue;
            if (pl.Object.InputAuthority == player)
            {
                return true;
            }
        }
        return false;
    }
}
