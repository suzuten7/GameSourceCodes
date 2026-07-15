namespace State
{
    using UnityEngine;
    public class State_RigGrounds : MonoBehaviour
    {
        [SerializeField] Rigidbody Rig;
        [SerializeField] float AirYVect;
        [SerializeField] float CheckHight;
        [SerializeField] float CheckSize;
        [SerializeField] float GroundDis;
        [SerializeField] float YSetChange;
        [SerializeField] float YSetPer;
        [SerializeField] float SlideYRemPer;
        [SerializeField] LayerMask CheckLayer;

        RaycastHit[] rayHits = new RaycastHit[50];
        private void FixedUpdate()
        {
            var vect = Rig.linearVelocity;
            var ypos = Rig.position.y;
            var ground = false;
            var dis = GroundDis;
            var rayBase = Rig.position + Vector3.up * CheckHight;
            var normal = Rig.linearVelocity;
            Debug.DrawLine(rayBase, rayBase + Vector3.down * GroundDis, Color.white);
            Debug.DrawLine(rayBase + Rig.transform.forward * CheckSize, rayBase + Rig.transform.forward * CheckSize + Vector3.down * GroundDis, Color.green);

            var hitCount = Physics.SphereCastNonAlloc(rayBase, CheckSize, Vector3.down, rayHits, GroundDis, CheckLayer);
            for(int i=0;i<hitCount;i++)
            {
                var hit= rayHits[i];
                if (hit.collider.isTrigger) continue;
                if (hit.rigidbody != null) continue;
                if (hit.distance <= 0) continue;
                ground = true;
                if(dis > hit.distance)
                {
                    dis = hit.distance;
                    ypos = rayBase.y - hit.distance - YSetChange;
                    normal = hit.normal;
                }

            }
            if (ground)
            {
                Debug.DrawLine(Rig.position, Rig.position + vect, Color.magenta);
                var hvect = vect;
                hvect.y = 0;
                var pvect = Vector3.ProjectOnPlane(hvect, normal);
                Debug.DrawLine(Rig.position, Rig.position + pvect.normalized * 5, Color.blue);
                if (pvect.y < 0)
                {
                    vect.y += pvect.y * SlideYRemPer * 0.01f;
                }
                else
                {
                    var nvect = pvect;
                    nvect.y = 0;
                    var mg = nvect.magnitude;

                    vect.x = pvect.normalized.x * mg;
                    vect.z = pvect.normalized.z * mg;
                }
                Debug.DrawLine(Rig.position,Rig.position + vect, Color.red);
                //BTVc.forward = Rig.linearVelocity;
                //CTVc.forward = vect;
                if (vect.y < AirYVect)
                {
                    var pos = Rig.position;
                    pos.y = Mathf.Lerp(pos.y, ypos, YSetPer * 0.01f);
                    Rig.position = pos;
                    vect.y *= 1f - YSetPer * 0.01f;
                }
                Debug.DrawLine(Rig.position, Rig.position + vect, Color.yellow);
            }
            Rig.linearVelocity = vect;
        }
    }
}

