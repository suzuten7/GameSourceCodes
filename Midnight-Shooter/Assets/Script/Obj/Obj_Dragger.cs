using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obj_Dragger : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rig;
    [SerializeField] float dragPow;
    Vector2 startPos;
    Dictionary<GameObject, float> dragCTs = new();
    public void Start()
    {
        startPos = transform.position;
        Obj_LocalObjects.Draggers.Add(this);
    }
    private void Update()
    {
        var drags = dragCTs.Keys.ToArray();
        for(int i = drags.Length - 1; i >= 0; i--)
        {
            dragCTs[drags[i]] -= Time.deltaTime;
            if (dragCTs[drags[i]] <= 0)dragCTs.Remove(drags[i]);
        }
    }
    public void Resets()
    {
        transform.position = startPos;
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        if (!col.collider.gameObject.TryGetComponent<Player_Hit>(out var phit)) return;
        if (!Net_Connect.CanControl(phit.pm.Object)) return;
        if (dragCTs.ContainsKey(phit.pm.gameObject)) return;
        dragCTs.Add(phit.pm.gameObject,1f);
        RPC_Drag(phit.pm.PosGet);
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    void RPC_Drag(Vector3 pos)
    {
        var vect = transform.position - pos;
        rig.AddForce(vect.normalized * dragPow * rig.mass);
    }
}
