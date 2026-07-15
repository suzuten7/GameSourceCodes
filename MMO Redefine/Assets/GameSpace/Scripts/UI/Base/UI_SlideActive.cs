namespace UIs
{
    using LitMotion;
    using UnityEngine;

    public class UI_SlideActive : MonoBehaviour
    {
        enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
        [Header("◆メインオプション")]
        [SerializeField, Tooltip("開く向き")]
        Direction direction;

        [SerializeField, Tooltip("展開する距離[ピクセル値]")]
        float moveVol = 100f;
        [SerializeField] float moveTime = 0.1f;

        [Header("◆その他")]
        [SerializeField] RectTransform targetRectPos;
        Vector2 basePosition;

        public bool isMoving = false;
        public bool isOpen = false;

        bool opend = false;
        void Start()
        {
            basePosition = targetRectPos.anchoredPosition;
        }
        public void Opens()
        {
            if (!isOpen) OpenClose();
        }
        public void Close()
        {
            if (isOpen) OpenClose();
        }
        public void OpenClose()
        {
            if (isMoving) return;

            Vector2 stPos = targetRectPos.anchoredPosition;
            Vector2 goPos = stPos;
            //移動方向の取得
            switch (direction)
            {
                case Direction.Left:
                    goPos.x = !isOpen ? basePosition.x - moveVol : basePosition.x;
                    break;

                case Direction.Right:
                    goPos.x = !isOpen ? basePosition.x + moveVol : basePosition.x;
                    break;

                case Direction.Up:
                    goPos.y = !isOpen ? basePosition.y + moveVol : basePosition.y;
                    break;

                case Direction.Down:
                    goPos.y = !isOpen ? basePosition.y - moveVol : basePosition.y;
                    break;

                default:
                    Debug.LogWarning("方向が指定されていません！");
                    return;
            }

            isMoving = true;
            isOpen = !isOpen;

            // アニメーション開始
            if (direction == Direction.Left || direction == Direction.Right)
            {
                LMotion.Create(stPos.x, goPos.x, moveTime)
                    .WithOnComplete(() =>
                    {
                        opend = !opend;
                        targetRectPos.anchoredPosition = new Vector2(goPos.x, targetRectPos.anchoredPosition.y);
                        isMoving = false;
                    })
                    .Bind(x => targetRectPos.anchoredPosition = new Vector2(x, targetRectPos.anchoredPosition.y));
            }
            else if (direction == Direction.Up || direction == Direction.Down)
            {
                LMotion.Create(stPos.y, goPos.y, moveTime)
                    .WithOnComplete(() =>
                    {
                        opend = !opend;
                        targetRectPos.anchoredPosition = new Vector2(targetRectPos.anchoredPosition.x, goPos.y);
                        isMoving = false;
                    })
                    .Bind(y => targetRectPos.anchoredPosition = new Vector2(targetRectPos.anchoredPosition.x, y));
            }
        }
    }
}
