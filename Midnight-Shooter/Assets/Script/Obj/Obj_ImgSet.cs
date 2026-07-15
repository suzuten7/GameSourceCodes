using UnityEngine;

public class Obj_ImgSet : MonoBehaviour
{
    float time;
    [SerializeField] SpriteRenderer srImg;
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] bool alphaOnly;
    [SerializeField] Gradient gradTime;
    [SerializeField] float timeStart;
    [SerializeField] float timeEnd;
    [SerializeField] bool noStop;
    Vector3 baseSize;
    private void Start()
    {
        baseSize = srImg.transform.localScale;
        ImgSet();
    }
    void Update()
    {
        if (!noStop && Obj_LocalObjects.TimeStopd) return;
        time += Time.deltaTime;
        ImgSet();
    }
    void ImgSet()
    {
        var t = Mathf.Clamp01(Mathf.InverseLerp(timeStart, timeEnd, time));
        srImg.transform.localScale = baseSize * scaleCurve.Evaluate(t);
        var col = gradTime.Evaluate(t);
        if (alphaOnly)
        {
            col.r = srImg.color.r;
            col.g = srImg.color.g;
            col.b = srImg.color.b;
        }
        srImg.color = col;
    }
}
