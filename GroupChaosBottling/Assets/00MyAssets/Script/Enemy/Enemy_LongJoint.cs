using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Enemy_LongJoint : MonoBehaviour
{
    [SerializeField] State_Base ESta;
    [SerializeField] float StartYUnder;
    [SerializeField] Transform[] JointTrans;
    [SerializeField] LineRenderer LRend;
    [SerializeField] float LeepPer;
    private void Start()
    {
        for (int i = 1; i < JointTrans.Length; i++)
        {
            var JPos = JointTrans[i].position;
            JPos.y -= i * StartYUnder;
            JointTrans[i].position = JPos;
        }
    }
    void FixedUpdate()
    {
        if (ESta.BreakT > 0) ESta.Rig.linearVelocity = Vector3.down * 100;
        for (int i = 0; i < JointTrans.Length -1; i++)
        {
            Vector3 JPos;
            if (ESta.BreakT <= 0)
            {
                JPos = Vector3.Slerp(JointTrans[i + 1].position, JointTrans[i].position, LeepPer * 0.01f);
            }
            else
            {

                var FPos = JointTrans[i + 1].position;
                FPos.y = JointTrans[0].position.y;
                JPos = Vector3.Slerp(JointTrans[i + 1].position, FPos, LeepPer * 0.01f);
            }
            JointTrans[i + 1].position = JPos;
        }

        if (LRend != null)
        {
            LRend.useWorldSpace = true;
            var Poss = new List<Vector3>();
            for (int i = 0; i < JointTrans.Length; i++) Poss.Add(JointTrans[i].position);
            LRend.positionCount = Poss.Count;
            LRend.SetPositions(Poss.ToArray());
        }

    }
}
