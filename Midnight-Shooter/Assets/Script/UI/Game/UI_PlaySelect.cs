using UnityEngine;

public class UI_PlaySelect : MonoBehaviour
{
    [SerializeField] GameObject[] playOnlys;
    [SerializeField] GameObject[] viewOnlys;
    void Update()
    {
        var view = false;
        if (Obj_LocalObjects.MyPlayer != null) view = true;
        for (int i = 0; i < playOnlys.Length; i++)
        {
            if (playOnlys[i] != null)
            {
                playOnlys[i].gameObject.SetActive(view);
            }
        }
        for (int i = 0; i < viewOnlys.Length; i++)
        {
            if (viewOnlys[i] != null)
            {
                viewOnlys[i].gameObject.SetActive(!view);
            }
        }
    }
}
