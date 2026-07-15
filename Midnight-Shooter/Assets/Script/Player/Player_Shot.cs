using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/* 内容
 * ・プレイヤーの射撃
 * ・反動
 * ・リロード処理
 */

public class Player_Shot : NetworkBehaviour
{
    [SerializeField] Player_Manager pm;

    int now_RecoilPatternID, now_FireCount, prevBullet;
    float cursorSpeed = 500f; 

    float spdMulti = 1f;
    float start_ReloadTime = 0f;
    float now_Diffusion, now_CT;
    bool reloading = false;
    Vector2 recoilOffset, targetScreenPos;
    bool reloadMelee = false;
    Data_Gun wepon
    {
        get
        {
            if (pm.values.now_CursorState == CursorState.Melee) return Data_Base.DB.melles[pm.states.melee_IndexNum];
            else return Data_Base.DB.guns[pm.states.gun_IndexNum];
        }
    }
    int mbullet
    {
        get
        {
            switch (pm.values.now_CursorState)
            {
                default: return pm.magazinMax;
                case CursorState.Melee: return pm.meleeMax;
            }
        }
    }
    ref int cbullet
    {
        get
        {
            switch (pm.values.now_CursorState)
            {
                default:return ref pm.values.gun_bullet;
                case CursorState.Melee:return ref pm.values.melee_bullet;
            }
        }
    }
    void Start()
    {
        if (!Net_Connect.CanControl(Object)) return;

        pm.values.gun_bullet = pm.magazinMax;
        pm.values.melee_bullet = pm.meleeMax;
        pm.values.set_ReloadTime = wepon.nReloadTime;
        reloading = false;

        targetScreenPos = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        if (!Net_Connect.CanControl(Object)) return;
        //弾上限+1を超えてる場合減らす
        if (pm.magazinMax > 0) pm.values.gun_bullet = Mathf.Clamp(pm.values.gun_bullet, 0, pm.magazinMax + 1);
        else pm.values.gun_bullet = pm.magazinMax;
        if (pm.meleeMax > 0) pm.values.melee_bullet = Mathf.Clamp(pm.values.melee_bullet, 0, pm.meleeMax + 1);
        else pm.values.melee_bullet = pm.meleeMax;
        MoveTargetPoint();

        #region 反動
        //反動の方向指定
        Vector2 dir = (pm.objects.targetPoint.position - transform.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(Vector2.up, dir);
        Vector2 reco = rot * recoilOffset;

        //反動セット
        pm.objects.recoilPoint.position = (Vector2)pm.objects.assistPoint.position + reco;
        Vector2 vc = pm.objects.recoilPoint.position - pm.PosGet;
        Vector2 finalPos = (Vector2)pm.PosGet + (vc.magnitude * (Vector2)pm.objects.rb.transform.up);
        pm.objects.finalPoint.position = finalPos;
        if (pm.StopMove)
        {
            pm.values.lastAtkTime += Time.deltaTime;

            //反動を戻す
            recoilOffset = Vector2.Lerp(recoilOffset,
                Vector2.zero, Mathf.Clamp01(Time.deltaTime * (1f / wepon.shots.recoil_Time)));
            #endregion
            //銃と近接切り替え
            if (pm.controlle.melee.trigger)
            {
                Data_Gun cwepd;
                if (pm.values.now_CursorState == CursorState.Melee)
                {
                    pm.values.now_CursorState = CursorState.Shot;
                    cwepd = Data_Base.DB.guns[pm.states.gun_IndexNum];
                }
                else
                {
                    pm.values.now_CursorState = CursorState.Melee;
                    cwepd = Data_Base.DB.melles[pm.states.melee_IndexNum];
                }
                var changeWait = cwepd.changeWaitTime * pm.passc.changeWaitTime_Mult;
                now_CT = Mathf.Max(now_CT, changeWait);
                recoilOffset.x += changeWait * 2;
            }
        }
        //リロード中 または カーソルモードがShot以外だったら射撃無効化
        //※ただしリロード中でも弾数が1以上残っていたら射撃可能
        bool canShot =
            (!reloading || cbullet > 0) &&
            (pm.values.now_CursorState == CursorState.Shot || pm.values.now_CursorState == CursorState.Melee);

        //拡散の表示切り替え
        if (pm.objects.dif_View != null)
        {
            now_Diffusion = getDiffusion();

            pm.objects.dif_Mr.enabled = canShot && now_Diffusion > 0;
            pm.objects.dif_View.viewAngle = now_Diffusion;
        }
        //死亡
        if (pm.hpTotal <= 0) return;
        if (pm.BufGet(BufType.Shock)) return;
        if (!pm.StopMove) return;
        //射撃不可なら終了
        if (!canShot) return;

        //リロードチェック
        var reloadCheck = false;
        if (wepon.tacticalUse && mbullet + 1 > cbullet) reloadCheck = true;
        if (!wepon.tacticalUse && mbullet > cbullet) reloadCheck = true;
        //リロード
        if (reloadCheck && pm.controlle.reload.trigger)
        {
            StartCoroutine(Reload());
        }
        if (reloading) return;
        #region 反動パターンセット
        //反動セット
        if (pm.controlle.shot.trigger)
        { now_RecoilPatternID = Random.Range(0, wepon.shots.recoil_Pattern.Count); }
        //リセット
        if (!pm.controlle.shot.press)
        { now_FireCount = 0; }
        #endregion

        if (now_CT > 0) now_CT -= Time.deltaTime;
        else
        {
            //フルオート射撃(長押し)
            if (!wepon.shots.semiAuto && pm.controlle.shot.press) Shot();
            //セミオート射撃(単押し)
            else if (wepon.shots.semiAuto && pm.controlle.shot.trigger) Shot();

            if (!pm.controlle.shot.press) now_FireCount = 0;
        }
    }

    /// <summary>
    /// リコイルポイントの移動
    /// </summary>
    void MoveTargetPoint()
    {
        //座標操作時
        if (!pm.controlle.targetMoveMode)
        {
            targetScreenPos = pm.controlle.target_pos;
        }
        //移動操作時
        else
        {
            targetScreenPos += pm.controlle.target_move * cursorSpeed * Time.deltaTime;
            //画面外防止
            targetScreenPos.x = Mathf.Clamp(targetScreenPos.x, 0f, Screen.width);
            targetScreenPos.y = Mathf.Clamp(targetScreenPos.y, 0f, Screen.height);
        }

        //変換
        Vector3 targetWorldPos = pm.objects.camera.ScreenToWorldPoint(new Vector3
            (targetScreenPos.x, targetScreenPos.y, Mathf.Abs(pm.objects.camera.transform.position.z)));
        targetWorldPos.z = 0f;

        pm.objects.targetPoint.position = targetWorldPos;


        var ndis = float.MaxValue;
        var npos = Vector2.zero;
        for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
        {
            var pl = Obj_LocalObjects.Players[i];
            if (pl == null) continue;
            if (pl.states.teamID == pm.states.teamID) continue;
            if (pl.Visible) continue;
            if (pl.hpTotal <= 0) continue;
            if (pl.values.noDamTime > 0) continue;

            if (!Obj_LocalObjects.ViewCheck(pl.PosGet, pm)) continue;
            if (Obj_LocalObjects.HideCheck(pl.PosGet, pl.passc.charaScale_Multi)) continue;
            var dis = Vector2.Distance(pl.PosGet, pm.objects.targetPoint.position);
            if (ndis <= dis) continue;
            ndis = dis;
            npos = pl.PosGet;
        }
        var assistRange = pm == Obj_LocalObjects.MyPlayer ? UI_OptionManager.OptionGetFloat("GP_Option 17", 3f) : 3f;
        var assistSpeed = pm == Obj_LocalObjects.MyPlayer ? UI_OptionManager.OptionGetFloat("GP_Option 19", 10f) : 20f;
        var tpos = (Vector2)pm.objects.targetPoint.position;
        if (ndis <= assistRange) tpos = npos;
        var vect = tpos - (Vector2)pm.objects.assistPoint.position;
        var pos = (Vector2)pm.objects.assistPoint.position;
        pos += vect.normalized * Mathf.Min(vect.magnitude, assistSpeed * 0.1f * Time.deltaTime);
        pm.objects.assistPoint.position = pos;
    }

    /// <summary>
    /// 射撃処理
    /// </summary>
    void Shot()
    {
        pm.values.lastAtkTime = 0;
        //弾数が0の時は強制リロード
        if (cbullet == 0) { StartCoroutine(Reload()); return; }
        int gid;
        switch (pm.values.now_CursorState)
        {
            default: gid = pm.states.gun_IndexNum; break;
            case CursorState.Melee:gid = pm.states.melee_IndexNum + (int)AtkID.Melee;break;
        }
        RPC_Shot
        (
            pm.PosGet,
            transform.eulerAngles,
            gid,
            now_Diffusion,
            pm.objects.finalPoint.position
        );
        Net_Value.SoundSet(pm.PosGet,pm,wepon.shots.soundRange*pm.passc.atkSound_Multi,wepon.shots.soundTime,wepon.shots.seAudio,wepon.shots.seVolue,wepon.shots.sePitch);
        //反動
        Vector2 recoil = GetRecoil();
        recoil *= pm.controlle.ads.press ? wepon.shots.adsRecoil_Multi : 1f;
        recoil *= pm.passc.recoil_Multi;
        //遠くになるほど反動減少
        float distance = Vector2.Distance(pm.objects.targetPoint.position, pm.transform.position);
        float t = Mathf.Clamp01(distance / 10f);
        float dis_Multi = Mathf.Lerp(1.0f, 0.7f, t);

        recoilOffset += recoil * dis_Multi * (spdMulti + 1);

        now_CT = 1 / (wepon.shots.RPS * pm.passc.bulletRPS_Multi);
        cbullet--;
    }

    /// <summary>
    /// 拡散の取得
    /// </summary>
    /// <returns> 拡散の値 </returns>
    float getDiffusion()
    {
        //現在の移動速度取得
        foreach (var data in pm.states.set_MoveState)
        {
            if (data.moveState == pm.values.now_MoveState)
            { spdMulti = data.spd_Multi; break; }
        }

        //腰撃ち状態か、ADS状態かの確認
        float stop = pm.controlle.ads.press ?
            wepon.shots.stopDiffusion.ads_Diffusion : wepon.shots.stopDiffusion.hip_Diffusion;
        float move = pm.controlle.ads.press ?
            wepon.shots.moveDiffusion.ads_Diffusion : wepon.shots.moveDiffusion.hip_Diffusion;
        var dif = stop + (move - stop) * spdMulti;
        return dif * pm.passc.diffusion_Multi;
    }

    /// <summary>
    /// リロード処理
    /// </summary>
    IEnumerator Reload()
    {
        if (reloading) yield break;
        reloading = true;
        reloadMelee = pm.values.now_CursorState == CursorState.Melee;
        Net_Value.SoundSet(pm.PosGet, pm, wepon.reloadSoundRange * pm.passc.atkSound_Multi, wepon.reloadSoundTime, wepon.reloadSeAudio, wepon.reloadSeVolume, wepon.reloadSePitch);
        bool tacticalReload = false;
        if (wepon.tacticalUse)
        {
            prevBullet = cbullet;
            tacticalReload = prevBullet > 0;
            pm.values.set_ReloadTime = tacticalReload
                ? wepon.tReloadTime  //タクティカルリロード
                : wepon.nReloadTime; //通常リロード

            //タクティカルリロードの場合は1発分残す
            //通常リロードの場合は0発分に
            //cbullet = tacticalReload ? 1 : 0;
        }
        else
        {
            prevBullet = 0;
            pm.values.set_ReloadTime = wepon.nReloadTime;

        }
        pm.values.now_ReloadTime = pm.values.set_ReloadTime * (1f - start_ReloadTime);

        while (pm.values.now_ReloadTime > 0)
        {
            //射撃などによるリロード中断
            //※次回はある程度進行した状態からリロードスタート(詳細はメソッド内コメント)
            var chancel = false;
            if (mbullet < 0) chancel = true;
            switch (pm.values.now_CursorState)
            {
                default:
                    chancel = true;
                    break;
                case CursorState.Shot:
                    if (reloadMelee) chancel = true;
                    break;
                case CursorState.Melee:
                    if (!reloadMelee) chancel = true;
                    break;
            }
            if (pm.controlle.shot.trigger && tacticalReload) chancel = true;
            if (chancel)
            {
                ReloadProgress(tacticalReload);

                //1発残っている状態で射撃した場合
                //if (pm.controlle.shot.trigger && tacticalReload)
                //{
                //    cbullet--;
                //}

                pm.values.now_ReloadTime = 0;
                reloading = false;
                yield break;
            }
            pm.values.now_ReloadTime -= Time.deltaTime / pm.passc.reload_Multi;
            yield return null;
        }
        cbullet = prevBullet > 0
                    ? mbullet + 1 //タクティカルリロード
                    : mbullet;    //通常リロード
        reloading = false;
        start_ReloadTime = 0f;
    }

    /// <summary>
    /// リロード中断処理
    /// </summary>
    /// <remarks> ※詳細はコメントを見て </remarks>
    void ReloadProgress(bool tacticalReload)
    {
        /* メモ
         * 20%未満                ：元に戻す
         * 20%～                  ：次回30%スタート([タクティカル]弾数1 / [通常]弾数0)
         * 90%以上 & 通常リロード ：次回90%スタート(弾数0)
         */

        float progress = 1f - (pm.values.now_ReloadTime / pm.values.set_ReloadTime);

        //20%未満：元に戻す
        if (progress < 0.2f)
        {
            start_ReloadTime = 0f;
            pm.values.now_ReloadTime = 0;
            //cbullet = prevBullet;
        }
        //90%以上 & 通常リロード：次回90%スタート
        else if (progress >= 0.9f && !tacticalReload)
        { start_ReloadTime = 0.9f; }
        //20%～：次回30%スタート
        else
        {
            start_ReloadTime = 0.3f;
            //cbullet = tacticalReload ? 1 : 0;
        }
    }

    /// <summary>
    /// 反動パターンの取得
    /// </summary>
    Vector2 GetRecoil()
    {
        //現在の発射数に応じたパターンを探す
        RecoilPattern now_Pattern = null;

        for (int i = 0; i < wepon.shots.recoil_Pattern.Count; i++)
        {
            if (now_FireCount >= wepon.shots.recoil_Pattern[i].activeNum)
            { now_Pattern = wepon.shots.recoil_Pattern[i];}
            else break;
        }

        now_FireCount++;
        float x = Random.Range(-wepon.shots.recoil_Random.x / 2, wepon.shots.recoil_Random.x / 2);
        float y = Random.Range(-wepon.shots.recoil_Random.y / 2, wepon.shots.recoil_Random.y / 2);
        return (now_Pattern != null ? now_Pattern.pattern : Vector2.zero) + new Vector2(x, y);
    }

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    void RPC_Shot(Vector3 bpos,Vector3 rot,int gid, float dif,Vector3 rpos)
    {
        Data_Gun guns;
        if(gid < (int)AtkID.Melee)guns = Data_Base.DB.guns[gid];
        else guns = Data_Base.DB.melles[gid - (int)AtkID.Melee];
        var up = Quaternion.Euler(rot) * Vector2.up;
        var right = Quaternion.Euler(rot) * Vector2.right;
        Vector2 pos = bpos + up * pm.passc.charaScale_Multi * 0.6f;
        var bcount = Mathf.Max(1, guns.shots.pallet);
        //同時弾射撃
        for (int i = 0; i < bcount; i++)
        {
            var offCountMult = i - (bcount-1) / 2f;
            var offPos = pos;
            offPos += (Vector2)up * guns.shots.offSetFront_All;
            offPos += (Vector2)up * guns.shots.offSetFront_Bullet * i;
            offPos += (Vector2)right * guns.shots.offSetSide_Bullet * offCountMult;
            Vector3 rotd = Vector3.zero;
            if (guns.shots.bulletRot_Reticle)
            {
                var vc = (Vector2)rpos - offPos;
                rotd.z = Mathf.Atan2(vc.y, vc.x) * Mathf.Rad2Deg - 90;
            }
            else rotd = rot;
            rotd.z += Random.Range(-dif / 2, dif / 2);
            rotd.z += guns.shots.offSetRot_Bullet * offCountMult;
            var qrot = Quaternion.Euler(rotd);
            //弾速
            float speed = guns.shots.speed * pm.passc.bulletSpeed_Multi * Random.Range(guns.shots.speed_Min, 1);
            var vect = qrot * Vector2.up * speed;
            //弾の生成
            GameObject bullet_Obj = Runner.Spawn
            (
                guns.shots.bulletObj,
                offPos,
                qrot,
                pm.Object.StateAuthority,
                onBeforeSpawned: (runner, obj) =>
                {
                   Net_RigSync.NStartSet(obj,vect);
                }
            ).gameObject;

            Bullet_Hit b = bullet_Obj.GetComponent<Bullet_Hit>();
            b.SetUp(pm,gid,guns,rpos);
        }
    }
}
