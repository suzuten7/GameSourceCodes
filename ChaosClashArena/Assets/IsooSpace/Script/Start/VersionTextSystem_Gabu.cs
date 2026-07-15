using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionTextSystem_Gabu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        if (tmp == null)
        {
            return;
        }
        tmp.text = "Version:" + Application.version;
    }
}
