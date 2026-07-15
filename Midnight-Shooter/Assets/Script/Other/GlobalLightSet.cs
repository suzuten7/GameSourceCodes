using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightSet : MonoBehaviour
{
    [SerializeField] Light2D light;
    float baseIntensity;
    private void Start()
    {
        baseIntensity = light.intensity;
    }
    private void Update()
    {
        var set = UI_OptionManager.OptionGetFloat("GP_Option 04",50f) * 0.01f * 2;
        light.intensity = baseIntensity * set;
    }
}
