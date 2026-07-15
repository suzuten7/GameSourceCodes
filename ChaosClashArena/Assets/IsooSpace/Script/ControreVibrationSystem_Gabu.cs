using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControreVibrationSystem_Gabu : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        foreach (Gamepad gamepad in Gamepad.all)
        {
            StartCoroutine(ResetControllerVibration(gamepad));
        }
    }

    private IEnumerator ResetControllerVibration(Gamepad gamepad)
    {
        gamepad.SetMotorSpeeds(0f, 0f);
        yield break;
    }

    //private IEnumerator Loop()
    //{
    //    while (true)
    //    {
    //        var gamepad = Gamepad.current;
    //        if (gamepad == null)
    //        {
    //            Debug.Log("ゲームパッド未接続");
    //            yield break;
    //        }

    //        Debug.Log("左モーター振動");
    //        gamepad.SetMotorSpeeds(10.0f, 0.0f);
    //        yield return new WaitForSeconds(1.0f);
    //        Debug.Log("右モーター振動");
    //        gamepad.SetMotorSpeeds(0.0f, 1.0f);
    //        yield return new WaitForSeconds(1.0f);
    //        gamepad.SetMotorSpeeds(2.0f, 2.0f);
    //        yield return new WaitForSeconds(1.0f);
    //        Debug.Log("モーター停止");
    //        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    //    }
    //}
}
