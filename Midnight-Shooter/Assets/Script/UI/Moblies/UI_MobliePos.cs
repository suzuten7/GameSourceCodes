using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_MobliePos : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField] UI_MoblieSets sets;
    public RectTransform rect;
    public string name;
    public float scale = 100;
    bool drag;
    Vector2 startPos;
    void Start()
    {
        startPos = rect.anchoredPosition;
        Load();
    }
    public void Load()
    {
        var posx = Library_SaveFiles.LoadFileFloat("Option","Moblie_" + gameObject.name + "_x", startPos.x);
        var posy = Library_SaveFiles.LoadFileFloat("Option", "Moblie_" + gameObject.name + "_y", startPos.y);
        rect.anchoredPosition = new Vector2(posx, posy);
        ScaleSet(Library_SaveFiles.LoadFileFloat("Option", "Moblie_" + gameObject.name + "_s", 100));
    }
    public void ScaleSet(float s)
    {
        scale = s;
        rect.localScale = Vector3.one *s * 0.01f;
    }
    public void Resets()
    {
        rect.anchoredPosition = startPos;
        ScaleSet(100);
    }
    public void Save()
    {
        Library_SaveFiles.SaveFile("Option", "Moblie_" + gameObject.name + "_x", rect.anchoredPosition.x.ToString());
        Library_SaveFiles.SaveFile("Option", "Moblie_" + gameObject.name + "_y", rect.anchoredPosition.y.ToString());
        Library_SaveFiles.SaveFile("Option", "Moblie_" + gameObject.name + "_s", scale.ToString());
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (sets != null && drag)
        {
            sets.selectPosUI = this;
            rect.position = eventData.position;
        }
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        drag = true;
    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        drag = false;
    }
}
