
namespace State
{
    using UnityEngine;
    using static GmSystem.GS_GlobalValues;
    public class State_StateHit : MonoBehaviour
    {
        public State_StateBase State;
        public float YOffSet;
        [SerializeField] Transform WeakTrans;
        [Header("部位補正,x=防御補正,y=軽減率")]
        public Vector2 AllRegists;
        public Vector2 ShortRegists;
        public Vector2 MidleRegists;
        public Vector2 LongRegists;
        public Vector2 OtherRegists;
        private void Start()
        {
            StHitList.Add(this);
            if (WeakTrans != null)
            {
                transform.parent = WeakTrans;
            }
        }
        public Vector3 PosGet
        {
            get
            {
                return transform.position + Vector3.up * YOffSet;
            }
        }
    }
}
