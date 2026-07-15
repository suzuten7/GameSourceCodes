using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAndDisplaySystem_Gabu : MonoBehaviour
{
    [SerializeField] private Camera _pl1Camera;
    [SerializeField] private Camera _pl2Camera;

    #region get set
    public Camera Pl1Camera { get => _pl1Camera; set => _pl1Camera = value; }
    public Camera Pl2Camera { get => _pl2Camera; set => _pl2Camera = value; }
    #endregion

    private void FixedUpdate()
    {
        //ディスプレイ数が２異常だったらtrueを与える
        DualDisplayMode(Display.displays.Length > 0 ? false : true);
    }

    public void DualDisplayMode(bool on)
    {
        if (on)
        {
            Pl1Camera.targetDisplay = 1;
            Pl2Camera.targetDisplay = 2;
        }
        else
        {
            Pl1Camera.targetDisplay = 1;
        }
    }
}
