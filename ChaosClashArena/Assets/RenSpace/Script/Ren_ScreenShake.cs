using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Ren_ScreenShake : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Vector3[] rotationStrength = new Vector3[3];

    private float shakeDuration = 0.3f;

    private void Update()
    {
        // テスト用
        if (Input.GetMouseButtonDown(0))
        {
            CameraShaker(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CameraShaker(1);
        }
        if (Input.GetMouseButtonDown(2))
        {
            CameraShaker(2);
        }
    }

    private void CameraShaker(int damage)
    {
        cam.DOComplete();
        if(damage <= 2 && damage >= 0)
        cam.DOShakeRotation(shakeDuration, rotationStrength[damage]);
    }
}
