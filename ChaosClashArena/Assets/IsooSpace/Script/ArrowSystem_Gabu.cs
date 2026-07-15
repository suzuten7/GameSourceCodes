using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowSystem_Gabu : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private Camera camera;
    [SerializeField] private Vector3 _forward = Vector3.forward;
    [SerializeField] private float n = 1f;
    public GameObject target;
    private Vector3 dir;



    private void Update()
    {
        if (CheckTargetPos(target))//ターゲットが画面内にいたら
        {
            arrow.SetActive(false);//矢印をオフ
            return;
        }
        arrow.SetActive(true);
        //arrow.transform.position = ArrowPosition();
        ArrowVector();
    }

    private bool CheckTargetPos(GameObject target)//targetが画面内に居るかを確認
    {
        if (camera.WorldToScreenPoint(target.transform.position).y > Screen.height)
        {
            return false;
        }
        if (camera.WorldToScreenPoint(target.transform.position).y < 0)
        {
            return false;
        }
        if (camera.WorldToScreenPoint(target.transform.position).x > Screen.width)
        {
            return false;
        }
        if (camera.WorldToScreenPoint(target.transform.position).x < 0)
        {
            return false;
        }
        return true;
    }

    private void ArrowVector()//矢印をターゲットに向かせる
    {
        dir = target.transform.position - arrow.transform.position;
        // ターゲットの方向への回転
        var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
        // 回転補正
        var offsetRotation = Quaternion.FromToRotation(_forward, Vector3.forward);

        // 回転補正→ターゲット方向への回転の順に、自身の向きを操作する
        arrow.transform.rotation = lookAtRotation * offsetRotation;
    }

    //private Vector3 ArrowPosition()
    //{
    //    Vector3 screenDir = target.transform.position - camera.transform.position;
    //    //矢印の方法の先にある画面の端の座標を求める
    //    Vector2 vecotor = camera.ScreenToWorldPoint(new Vector2(screenDir.normalized.x * Screen.width
    //                        , screenDir.normalized.y * Screen.height));

    //    return new Vector3(vecotor.x * n, vecotor.y * n, arrow.transform.position.z);
    //}
}
