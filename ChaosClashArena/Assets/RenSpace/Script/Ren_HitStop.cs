using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ren_HitStop : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            HitStop();
        }
    }

    public void HitStop()
    {
        Time.timeScale = 0;
        StartCoroutine(TimerStart());
    }

    private IEnumerator TimerStart()
    {
        Debug.Log("A");
        yield return new WaitForSecondsRealtime(1);
        Debug.Log("B");
        Time.timeScale = 1;
        Debug.Log("C");
    }
}
