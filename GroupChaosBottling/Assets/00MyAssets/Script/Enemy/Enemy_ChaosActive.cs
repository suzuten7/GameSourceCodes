using UnityEngine;
using static BattleManager;
using static Manifesto;
public class Enemy_ChaosActive : MonoBehaviour
{
    [SerializeField] GameObject[] ActiveObjs;
    void Awake()
    {
        for (int i = 0; i < ActiveObjs.Length; i++)
        {
            if (ActiveObjs[i] != null) ActiveObjs[i].SetActive(false);
        }
    }
    void FixedUpdate()
    {
        for (int i = 0; i < ActiveObjs.Length; i++)
        {
            if (ActiveObjs[i] != null) ActiveObjs[i].SetActive(ChaosCheck((Enum_Stage)BTManager.Stage));
        }
    }
}
