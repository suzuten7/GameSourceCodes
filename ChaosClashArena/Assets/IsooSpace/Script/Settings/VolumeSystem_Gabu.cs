using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSystem_Gabu : MonoBehaviour
{
    [SerializeField]
    private static AudioSource se;

    [SerializeField]
    private AudioMixer audioMixer;


    public Slider seS;
    public Slider bgmS;


    private void Start()
    {
        float BGM_Vol = PlayerPrefs.GetFloat("BGM_Volume", 1);
        bgmS.value = BGM_Vol;
        ChangeBGMVolume(BGM_Vol);

        float SE_Vol = PlayerPrefs.GetFloat("SE_Volume", 1);
        seS.value = SE_Vol;
        ChangeSEVolume(SE_Vol);

    }

    private float LnMethod(float x)
    {
        return Mathf.Pow(Mathf.Exp(1), (x * Mathf.Log(Mathf.Exp(1), 10)) / 10);
    }

    public void ChangeBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM_Volume", Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f));
    }
    public void ChangeSEVolume(float volume)
    {
        audioMixer.SetFloat("SE_Volume", Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f));
    }
    public void ChangeBGMVolume(Slider slider)
    {
        audioMixer.SetFloat("BGM_Volume", Mathf.Clamp(Mathf.Log(slider.value, 10) * 60f, -80, 20));
        PlayerPrefs.SetFloat("BGM_Volume", slider.value);
    }
    //Mathf.Clamp(Mathf.Log10(slider.value) * 20f, -80f, 0f)
    public void ChangeSEVolume(Slider slider)
    {
        audioMixer.SetFloat("SE_Volume", Mathf.Clamp(Mathf.Log(slider.value, 10) * 60f, -80, 20));
        PlayerPrefs.SetFloat("SE_Volume", slider.value);
    }

    static public void RingSound(AudioClip clip)
    {
        if (se != null)
        {
            se.PlayOneShot(clip);
        }
    }
    static public void RingSound(AudioClip clip, AudioSource auso)
    {
        if (auso != null)
        {
            auso.PlayOneShot(clip);
        }
    }
}
