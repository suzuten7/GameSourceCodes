using UnityEngine;
using DG.Tweening;

public class ObjectAnimation_Gabu : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(-10, 0, 0);
    public Vector3 endPosition = new Vector3(10, 0, 0);
    public float duration = 1f;
    public Ease ease = Ease.Linear;
    private void Start()
    {
        transform.position = startPosition;

        transform.DOMove(endPosition, duration).SetEase(ease).SetLoops(-1,LoopType.Yoyo);
    }
}
