using UnityEngine;
using UnityEngine.UI;
public class UI_DebugDispSet : MonoBehaviour
{
    static public bool[] DebugDisps = new bool[5];
    [SerializeField] public Toggle[] DebugToggles;

    void Update()
    {
        for(int i = 0; i < DebugToggles.Length; i++)
        {
            DebugToggles[i].isOn = DebugDisps[i];
        }
    }
    public void ToggleChange(int ID)
    {
        DebugDisps[ID] = DebugToggles[ID].isOn;
    }
}
