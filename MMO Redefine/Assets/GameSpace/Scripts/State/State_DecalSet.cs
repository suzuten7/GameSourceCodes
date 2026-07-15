using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace State
{
    public class State_DecalSet : MonoBehaviour
    {
        [SerializeField] DecalProjector Decal;
        [SerializeField] float TimeFadeIn;
        [SerializeField] float TimeEnd;
        [SerializeField] float TimeFadeOut;
        int time = 0;
        private void Start()
        {
            Decal.fadeFactor = 0f;
            Decal.material = new(Decal.material);
        }
        void FixedUpdate()
        {
            time++;
        }
        private void LateUpdate()
        {
            var timef = time / 60f;

            var alpha = 1f;
            if (timef <= TimeFadeIn) alpha = Mathf.Lerp(Decal.fadeFactor, 1f, Mathf.Clamp01(timef / TimeFadeIn));
            else if(timef - TimeEnd >= 0)alpha = Mathf.Lerp(Decal.fadeFactor, 0f, Mathf.Clamp01((timef - TimeEnd) / TimeFadeIn));
            Decal.fadeFactor = alpha;
            Decal.material.SetFloat("_Times", Mathf.Clamp01(timef / TimeEnd));
        }
    }
}

