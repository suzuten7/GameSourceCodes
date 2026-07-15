using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SinMove : MonoBehaviour
{
    [SerializeField] RectTransform Rect;
    [SerializeField] Canvas Canva;
    [SerializeField] float Speed;
    [SerializeField] float Times;
    [SerializeField] float TimeEnd;
    [SerializeField] float Ml;
    void Update()
    {
        Times+= Speed;
        if (Times > TimeEnd) Times = 0;
        if (Times < 0) Times = TimeEnd;
        Rect.localPosition = new Vector2((Times-(TimeEnd/2f))/(float)TimeEnd* Canva.pixelRect.width*2* Ml, 0);

    }
}
