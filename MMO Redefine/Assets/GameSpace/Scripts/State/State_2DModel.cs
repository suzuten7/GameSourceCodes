
namespace State
{
    using System.Collections.Generic;
    using UnityEngine;

    public class State_2DModel : MonoBehaviour
    {
        public State_StateBase Sta;
        public List<Class_Img> Imgs;
        [SerializeField] Transform RotTrans;
        [SerializeField] MeshRenderer[] Rends;
        [SerializeField] int AID;

        [SerializeField] int pi;
        [SerializeField] Texture tx;

        [System.Serializable]
        public class Class_Img
        {
            public int type;
            public bool back;
            public float paturnSpeed;
            public List<Texture> imgs;
        }
        float t = 0;
        bool back= false;
        bool right= false;
        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                Vector3 forward = transform.forward;
                forward.y = 0;
                forward.Normalize();
                Vector3 camDir = Camera.main.transform.forward;
                camDir.y = 0;
                camDir.Normalize();
                float angleC = Vector3.SignedAngle(forward, camDir, Vector3.up);
                float absAngleC = Mathf.Abs(angleC);
                float offset = Mathf.Sin(absAngleC * Mathf.Deg2Rad) * 45f* Mathf.Sign(angleC);
                RotTrans.rotation = Quaternion.LookRotation(-camDir) * Quaternion.Euler(0, offset * (back ? -1 : 1), 0);

                var nrot = 10f;
                if (angleC >= -90 + nrot && angleC <= 90 - nrot) back = true;
                if (angleC <= -90 - nrot || angleC >= 90 + nrot) back = false;
                if (Sta != null)
                {
                    var rvect = Sta.SettingValues.Rig.linearVelocity;
                    rvect.y = 0;
                    if (rvect.magnitude > 0.05f)
                    {
                        rvect.Normalize();
                        float angleR = Vector3.SignedAngle(rvect, forward, Vector3.up);
                        var nvect = 10f;
                        if (angleR > nvect) right = true;
                        if (angleR < -nvect) right = false;
                    }
                }
                var eri = right;
                if (back) eri = !eri;
                RotTrans.localScale = new Vector3(eri ? 1 : -1, 1, !back ? 1 : -1);
            }


            if (Sta != null)
            {
                AID = 0;
                if (Sta.AnimValues.MoveID > 0) AID = 1;
                if (Sta.AnimValues.LAtkID > 0 || Sta.AnimValues.RAtkID > 0 || Sta.AnimValues.SAtkID > 0) AID = 2;
                if (Sta.HP <= 0 || Sta.AnimValues.OtherID == -1) AID = 3;
            }
            t += Time.deltaTime;
            Class_Img cAnim_F = null;
            Class_Img cAnim_B = null;
            for (int i = 0; i < Imgs.Count; i++)
            {
                var anim = Imgs[i];
                var actives = false;
                if (anim.type == 0 || (anim.type - 1) == AID) actives = true;
                if (!actives) continue;
                if(cAnim_F == null || !anim.back)cAnim_F = anim;
                if (anim.back) cAnim_B = anim;

            }
            if (cAnim_F == null && Imgs.Count > 0) cAnim_F = Imgs[0];
            if(cAnim_B == null)cAnim_B = cAnim_F;
            if (cAnim_F != null)
            {
                var pid = (int)Mathf.Repeat(Time.time * cAnim_F.paturnSpeed, cAnim_F.imgs.Count);
                var img = cAnim_F.imgs[pid];
                pi = pid;
                tx = img;
                for (int k = 0; k < 2; k++) Rends[k].material.SetTexture("_BaseMap", img);
            }
            if (cAnim_B != null)
            {
                var pid = (int)Mathf.Repeat(Time.time * cAnim_B.paturnSpeed, cAnim_B.imgs.Count);
                var img = cAnim_B.imgs[pid];
                pi = pid;
                tx = img;
                for (int k = 0; k < 2; k++) Rends[k + 2].material.SetTexture("_BaseMap", img);
            }

        }
    }
}

