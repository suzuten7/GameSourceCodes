using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Splines;

public class TestMovingUi_Gabu : MonoBehaviour
{
    [Header("四つ")]
    public GameObject[] objs;
    public SplineContainer[] splines;
    public string[] elements;
    public Animation[] anims;
    private int times;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            times++;
            Debug.Log("is OK");
            for (int i = 0; i < objs.Length; i++)//全て再生
            {

                objs[i].GetComponent<SplineAnimate>().Container= splines[(int)Mathf.Repeat(times+i, objs.Length)];
                //    objs[i].GetComponent<SplineAnimate>().Play();
                objs[i].GetComponent<SplineAnimate>().Restart(true);
                
            }

            //for (int i = 0; i < objs.Length; i++)
            //{
            //    //テキストを入れ替え
            //    objs[i].GetComponent<TextMeshPro>().text
            //        = elements[i != (elements.Length - 1) ? i + 1 : 0];

            //    //位置リセット
            //    objs[i].GetComponent<SplineAnimate>().Restart(true);
            //}
        }
    }
}
