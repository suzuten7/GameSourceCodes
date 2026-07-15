using UnityEngine;
using UnityEngine.Audio;
using static GlovalValues;
public class AudioSets : MonoBehaviour
{
    [SerializeField] AudioMixer AudioMix;
    void Update()
    {
        AudioMix.SetFloat("Master_Volume", Mathf.Clamp(Mathf.Log(SoundOptions[0] / 100f, 10) * 60f, -80, 20));
        AudioMix.SetFloat("BGM_Volume", Mathf.Clamp(Mathf.Log(SoundOptions[1] / 100f, 10) * 60f, -80, 20));
        AudioMix.SetFloat("SE_Volume", Mathf.Clamp(Mathf.Log(SoundOptions[2] / 100f, 10) * 60f, -80, 20));
    }
}
