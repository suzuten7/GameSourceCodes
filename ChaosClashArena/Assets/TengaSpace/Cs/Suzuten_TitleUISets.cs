using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Suzuten_TitleUISets : MonoBehaviour
{
    [SerializeField] GameObject BaseUIs;
    [SerializeField] GameObject StartsUIs;

    private void Start()
    {
        BaseUIs.SetActive(true);
        StartsUIs.SetActive(false);
    }
    public void StartsOC(bool B)
    {
        BaseUIs.SetActive(!B);
        StartsUIs.SetActive(B);
    }

}
