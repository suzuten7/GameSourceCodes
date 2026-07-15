using Fusion;
using UnityEngine;

public class Player_Other : NetworkBehaviour
{
    [SerializeField] Player_Manager pm;
    [Networked] Player_Marker marker { get; set;}
    void Update()
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (pm.controlle.marker.trigger)
        {
            RPC_MarkerSet(pm.objects.targetPoint.position);
        }
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    void RPC_MarkerSet(Vector3 pos)
    {
        if(marker == null)
        {
            Runner.Spawn
                (
                Data_Base.DB.marker,
                pos,
                Quaternion.identity,
                PlayerRef.None,
                onBeforeSpawned: (runner, obj) =>
                {
                    var mark = obj.GetComponent<Player_Marker>();
                    mark.pm = pm;
                    marker = mark;
                }
                );
        }
        else
        {
            Destroy(marker.gameObject);
        }
    }
}
