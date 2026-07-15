using UnityEngine;

public class Obj_Sound : MonoBehaviour
{
    public bool my;
    public int userTeam;
    public float rangeMax;
    public float lastTime;
    [SerializeField]float times;
    [SerializeField] AnimationCurve alphaTimeCurve;
    [SerializeField] SpriteRenderer sr;
    public AudioSource se;
    private void Start()
    {
        Obj_LocalObjects.Sounds.Add(this);
        sr.gameObject.SetActive(false);
        Library_ObjParentSet.ParentSet(gameObject, "SoundSEs");
    }
    private void Update()
    {
        times += Time.deltaTime;
        if (times >= lastTime) Destroy(gameObject);
        sr.gameObject.SetActive(my);
        if (my)
        {
            float tsize = 1f;
            if (lastTime < float.PositiveInfinity) tsize = Mathf.Clamp01(times / lastTime * 1.5f);
            sr.transform.localScale = Vector3.one * rangeMax * tsize * 2f;
            var color = UI_OptionManager.OptionGetColor("UI_Option 25", new Color(1, 1, 1, 0.25f));
            color.a *= AlphaGet;
            sr.color = color;
        }
    }
    public float AlphaGet
    {
        get
        {
            return alphaTimeCurve.Evaluate(times / lastTime);
        }
    }
}
