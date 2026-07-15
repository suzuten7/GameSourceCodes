using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;

public class DisplaySystem_Gabu : MonoBehaviour
{
    public CameraAndDisplaySystem_Gabu cads;

    // Start is called before the first frame update
    void Start()
    {
        if (cads == null)
        {
            return;
        }

        if (Display.displays.Length > 0)
        {
            cads.DualDisplayMode(false);
        }
        if (Display.displays.Length > 1)
        {
            cads.DualDisplayMode(true);
        }
    }
}
