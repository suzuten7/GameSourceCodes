using UnityEngine;
using UnityEngine.InputSystem;

public class UI_KeySets : MonoBehaviour
{
    PlayerInput PI;
    [SerializeField] GameObject[] UIs;
    [SerializeField] string[] Schemes;

    void Update()
    {
        if (PI == null) PI = PlayerInput.GetPlayerByIndex(0);
        if (PI == null) return;
        for(int i = 0; i < Schemes.Length; i++)
        {
            UIs[i].SetActive(PI.currentControlScheme == Schemes[i]);
        }
    }
}
