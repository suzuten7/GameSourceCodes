namespace UIs
{
    using UnityEngine;
    public class UI_StateObject : UI_StatusBase
    {

        float Hight;
        private void Start()
        {
            Hight = transform.localPosition.y;
        }
        protected override void LateUpdate()
        {
            base.LateUpdate();
            transform.position = Sta.PosGet + Vector3.up * (Hight - 1.2f);
            if (Camera.main != null) transform.LookAt(Camera.main.transform);
        }
    }
}
