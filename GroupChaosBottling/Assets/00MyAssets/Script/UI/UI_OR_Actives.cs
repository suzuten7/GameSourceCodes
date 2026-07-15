using UnityEngine;

public class UI_OR_Actives : MonoBehaviour
{
    [SerializeField] GameObject[] ORObjs;
    [SerializeField] GameObject[] ActiveObjs;

    void LateUpdate()
    {
        bool OR = false;
        for (int i = 0; i < ORObjs.Length; i++) if (ORObjs[i].activeSelf) OR = true;
        for(int i = 0; i < ActiveObjs.Length; i++) ActiveObjs[i].SetActive(OR);
    }
}
