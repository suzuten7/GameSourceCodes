namespace UIs
{

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class UI_PressButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] UnityEvent clickEvnet;
        [SerializeField] UnityEvent pressdEvnet;
        [SerializeField] float pressTime;
        bool press = false;
        float ptime = 0;

        void Update()
        {
            if (press)
            {
                ptime += Time.deltaTime;
                if (ptime >= pressTime) pressdEvnet.Invoke();
            }
            else
            {
                if (ptime > 0 && ptime < pressTime) clickEvnet.Invoke();
                ptime = 0;
            }

            if (Input.GetMouseButtonUp(0)) press = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            press = true;
        }
    }
}
