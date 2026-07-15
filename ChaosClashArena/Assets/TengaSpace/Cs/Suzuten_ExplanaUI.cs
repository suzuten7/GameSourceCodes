using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_ExplanaUI : MonoBehaviour
{
    [SerializeField] GameObject[] UIs;
    void Start()
    {
        UIChange(0);
    }
    public void UIChange(int ID)
    {
        for (int i = 0; i < UIs.Length; i++) UIs[i].SetActive(i == ID);
    }
}
