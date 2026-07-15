namespace UIs
{
    using LitMotion;
    using UnityEngine;

    public class UI_FadeActive : MonoBehaviour
    {
        [Header("◆メインオプション")]
        [Tooltip("タブの開閉時間"), SerializeField]
        float changeTime = 0.1f;
        [SerializeField] CanvasGroup canvasGroup;
        public bool isActive = false;
        public bool isMoving = false;

        bool actived = false;

        bool play = false;
        void Start()
        {
            if (!play)
            {
                actived = isActive;
                if (!isActive) gameObject.SetActive(false);
            }
        }
        public void Opens()
        {
            if (!isActive) OpenClose();
        }
        public void Close()
        {
            if (isActive) OpenClose();
        }
        public void OpenClose()
        {
            play = true;
            if (isMoving) return;
            isMoving = true;
            isActive = !isActive;
            float start, goal;
            // Close → Open
            if (!actived)
            {
                gameObject.SetActive(true);
                start = 0; goal = 1;
            }
            // Open → Close
            else { start = 1; goal = 0; }

            // アニメーション
            LMotion.Create(start, goal, changeTime)
                .WithOnComplete(() =>
                {
                    if (actived) gameObject.SetActive(false);   // Open → Close時のみ
                    isMoving = false;
                    actived = !actived;
                })
                .WithEase(Ease.OutCirc)
                .Bind(a => canvasGroup.alpha = a);
        }
    }
}
