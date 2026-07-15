using UnityEngine;
using UnityEngine.Events;
public class AllNonEvent : MonoBehaviour
{
    [SerializeField] GameObject[] Objs;
    [SerializeField] UnityEvent IfEvents;
    [SerializeField] UnityEvent ElseEvents;
    bool Ifd = false;
    void Update()
    {
        bool Check = true;
        for(int i = 0; i < Objs.Length; i++)
        {
            if (Objs[i] == null) continue;
            if (!Objs[i].activeInHierarchy) continue;
            var Sta = Objs[i].GetComponent<State_Base>();
            if (Sta != null && Sta.HP <= 0) continue;
            Check = false;
        }
        if (Check)
        {
            if(!Ifd) IfEvents.Invoke();
            Ifd = true;
        }
        else
        {
            if (Ifd) ElseEvents.Invoke();
            Ifd = false;
        }

    }
}
