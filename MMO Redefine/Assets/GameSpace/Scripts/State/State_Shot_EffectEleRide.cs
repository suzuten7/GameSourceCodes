
namespace State
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Datas.Data_Get;
    public class State_Shot_EffectEleRide : MonoBehaviour
    {
        [SerializeField] State_Shot_Base Shot;
        [SerializeField] GameObject[] Effects;
        private void Start()
        {
            ColSet();
        }
        void ColSet()
        {

            var ColBase = DB.ElementColors[Shot.LEleRideID];
            Color Colc;
            for (int i = 0; i < Effects.Length; i++)
            {
                var EfPart = Effects[i].GetComponent<ParticleSystem>();
                if (EfPart != null)
                {
                    var EfMain = EfPart.main;
                    var EfColors = EfMain.startColor;
                    switch (EfColors.mode)
                    {
                        case ParticleSystemGradientMode.Color:
                            Colc = ColBase;
                            Colc.a = EfColors.color.a;
                            EfColors.color = Colc;
                            break;
                        case ParticleSystemGradientMode.Gradient:
                            EfColors.gradient = GradColKeyChange(ColBase, EfColors.gradient);
                            break;
                        case ParticleSystemGradientMode.TwoColors:
                            Colc = ColBase;
                            Colc.a = EfColors.colorMin.a;
                            EfColors.colorMin = Colc;
                            Colc = ColBase;
                            Colc.a = EfColors.colorMax.a;
                            EfColors.colorMax = Colc;
                            break;
                        case ParticleSystemGradientMode.TwoGradients:
                            EfColors.gradientMin = GradColKeyChange(ColBase, EfColors.gradientMin);
                            EfColors.gradientMax = GradColKeyChange(ColBase, EfColors.gradientMax);
                            break;
                    }
                    EfMain.startColor = EfColors;
                    EfPart.Stop();
                    EfPart.Play();
                }
                var EfTrail = Effects[i].GetComponent<TrailRenderer>();
                if (EfTrail != null)
                {
                    EfTrail.colorGradient = GradColKeyChange(ColBase,EfTrail.colorGradient);
                }
                var EfLine = Effects[i].GetComponent<LineRenderer>();
                if (EfLine != null)
                {
                    EfLine.colorGradient = GradColKeyChange(ColBase, EfLine.colorGradient);
                }
                var EfMeshRend = Effects[i].GetComponent<MeshRenderer>();
                if (EfMeshRend != null)
                {
                    Colc = ColBase;
                    Colc.a = EfMeshRend.material.color.a;
                    EfMeshRend.material.color = Colc;
                }
            }
        }
        Gradient GradColKeyChange(Color ColBase,Gradient Grad)
        {
            var ColKeys = new List<GradientColorKey>();
            for(int i = 0; i < Grad.colorKeys.Length; i++)
            {
                ColKeys.Add(new GradientColorKey(ColBase, Grad.colorKeys[i].time));
            }
            Grad.SetColorKeys(ColKeys.ToArray());
            return Grad;
        }
    }
}
