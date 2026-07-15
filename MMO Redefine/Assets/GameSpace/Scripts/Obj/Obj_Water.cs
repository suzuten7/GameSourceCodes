namespace Obj
{
    using System.Collections.Generic;
    using UnityEngine;
    using State;
    public class Obj_Water : MonoBehaviour
    {
        public Vector3 Pow;
        public Vector2 VectRemPer;
        public List<Rigidbody> Hits;
        private void OnTriggerEnter(Collider other)
        {
            var oRig = other.attachedRigidbody;
            if (oRig != null) Hits.Add(oRig);
            var oSHit = other.GetComponent<State_StateHit>();
            if (oSHit != null && !oSHit.State.ChangeValues.Waters.Contains(this)) oSHit.State.ChangeValues.Waters.Add(this);
        }
        private void OnTriggerExit(Collider other)
        {
            var oRig = other.attachedRigidbody;
            if (oRig != null) Hits.Remove(oRig);
            var oSHit = other.GetComponent<State_StateHit>();
            if (oSHit != null && oSHit.State.ChangeValues.Waters.Contains(this)) oSHit.State.ChangeValues.Waters.Remove(this);
        }
        private void FixedUpdate()
        {
            for (int i = Hits.Count - 1; i >= 0; i--)
            {
                if (Hits[i] == null)
                {
                    Hits.RemoveAt(i);
                    continue;
                }
                var RVect = Hits[i].linearVelocity;
                RVect.x *= 1f - VectRemPer.x * 0.01f;
                RVect.z *= 1f - VectRemPer.x * 0.01f;
                RVect.y *= 1f - VectRemPer.y * 0.01f;
                RVect += Pow * 0.01f;
                Hits[i].linearVelocity = RVect;
            }
        }
    }
}
