using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BinaryUIAnimationo : MonoBehaviour
{
    public ColorebleUI myAnimation;
    public bool isReverse = true;
    public void UpdateAnimation(bool binary)
    {
        if (binary)
        {
            myAnimation.Play();
        }
        else
        {
            if (isReverse)
            {
                myAnimation.Reverse();
            }
            else
            {
                myAnimation.Play();
            }
        }
    } 
}
