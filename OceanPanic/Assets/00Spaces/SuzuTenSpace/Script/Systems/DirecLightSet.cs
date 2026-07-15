using Photon.Pun;
using UnityEngine;

public class DirecLightSet : MonoBehaviour
{
    [SerializeField] Light DLight;
    [SerializeField,Header("朝")]TimeLights Morning_Lights;
    [SerializeField, Header("昼")] TimeLights Afternoon_Lights;
    [SerializeField, Header("夕")] TimeLights Evening_Lights;
    [SerializeField, Header("夜")] TimeLights Night_Lights;
    [System.Serializable]
    class TimeLights
    {
        public Color Color;
        public float Power;
    }

    void Start()
    {
        if (!PhotonNetwork.InRoom) return;
        var RoomProp = PhotonNetwork.CurrentRoom.CustomProperties;
        TimeLights SetLights;
        switch (RoomProp["StageTime"])
        {
            default: SetLights = Morning_Lights; break;
            case 1: SetLights = Afternoon_Lights; break;
            case 2: SetLights = Evening_Lights; break;
            case 3: SetLights = Night_Lights; break;
        }
        DLight.color = SetLights.Color;
        DLight.intensity = SetLights.Power;

    }
}
