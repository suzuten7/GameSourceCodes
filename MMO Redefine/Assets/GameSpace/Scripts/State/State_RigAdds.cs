using UnityEngine;
namespace State
{
    public class State_RigAdds : MonoBehaviour
    {
        [System.NonSerialized] public Rigidbody Rig;
        public float GravPer;
        private void FixedUpdate()
        {
            if (Rig == null)
            {
                Rig = GetComponent<Rigidbody>();
                if (Rig == null)
                {
                    enabled = false;
                    return;
                }
            }
            Rig.useGravity = false;
            Rig.AddForce(Physics.gravity * Rig.mass * GravPer * 0.01f, ForceMode.Force);
        }
    }
}
