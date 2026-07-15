namespace GmSystem
{
    using Pixiv.VroidSdk;
    using Pixiv.VroidSdk.Api;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;
    using UniVRM10;

    public class GS_VroidDictionary : MonoBehaviour
    {
        [SerializeField]string Pass;
        static GS_VroidDictionary VroidDic;
        Dictionary<string, (string,Texture2D)> Vroids = new ();
        private void Start()
        {
            VroidDic = this;
        }
        static public void VroidAdds(string ID,GameObject VroidObj)
        {
            if (ID == "") return;

            var Vrm = VroidObj != null ? VroidObj.GetComponent<Vrm10Instance>() : null;
            if (!VroidDic.Vroids.ContainsKey(ID))
            {
                VroidDic.Vroids.Add(ID, ("読み込みエラー",null));
                if (VroidObj != null)
                {
                    VroidObj.transform.parent = VroidDic.transform;
                    VroidObj.SetActive(false);
                }
            }
            if(Vrm != null)
            {
                var img = Vrm.Vrm.Meta.Thumbnail;
                RenderTexture rt = RenderTexture.GetTemporary(img.width, img.height, 0);
                Graphics.Blit(img, rt);

                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = rt;

                var nimg = new Texture2D(img.width, img.height);
                nimg.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                nimg.Apply();

                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(rt);
                VroidDic.Vroids[ID] = (Vrm.Vrm.Meta.Name,nimg);
            }
        }
        static public (string, Texture2D)? VroidGet(string ID)
        {
            if (ID == "") return null;
            return VroidDic.Vroids.TryGetValue(ID,out var oObj) ? oObj : null;
        }
        static public void VroidIDLoad(string ID,System.Action<GameObject> LoadObj,System.Action<bool> End)
        {
            // 通信に使うThreadContext
            var context = SynchronizationContext.Current;
            // ダウンロードしたcredential.json.bytesを読み込み
            var credential = Resources.Load<TextAsset>("credential.json");
            // 設定に使うアプリ情報
            var credentialJson = credential.text;
            // 認可に使うConfigの作成
            var config = OauthProvider.CreateSdkConfig(credentialJson);
            // OAuthの認可を扱うClientを作成する
            var oauthClient = OauthProvider.CreateOauthClient(config, context);
            // ログインに使うBrowserを作成
            var browser = BrowserProvider.Create(oauthClient, config);
            // ローカルにアカウントファイル保存済みかつ期限が切れてない
            var isLoggedIn = oauthClient.IsAccountFileExist() && !oauthClient.IsAccessTokenExpired();
            // ログイン
            if (!isLoggedIn)
            {
                // すでに認可済みだが期限切れの場合は再認可。 
                // そうでなければブラウザを開いて認可フローを開始する。
                oauthClient.Login(
                    browser,
                    (account) => { /*ログイン成功時*/ },
                (error) => { /*ログイン失敗時*/ }
                );
            }
            Debug.Log("Logins" + isLoggedIn);
            var defaultApi = new DefaultApi(oauthClient);
            Debug.Log("Vroid" + ID + "ロード開始");
            oauthClient.Login
                (
                    browser,
                    (account) =>
                    {
                        // モデル読み込みに使うModelLoaderを初期化
                        ModelLoader.Initialize(
                            config,                  // Credentialから作成したConfig
                            defaultApi,              // 認可済みのAPI
                            VroidDic.Pass, // モデル暗号化のパスワード
                            10                       // モデルの最大キャッシュ数
                        );
                        defaultApi.GetCharacterModels(ID, (model) =>
                        {
                            
                            // モデル読み込みの開始
                            ModelLoader.LoadVrm(
                                model.character_model,
                                // 読み込むモデル
                                (gameObject) =>
                                {
                                    // 読み込み完了後のコールバック
                                    VroidAdds(ID, gameObject);
                                    Debug.Log("Vroid" + ID + "ロード完了");
                                    LoadObj.Invoke(gameObject);
                                    End.Invoke(true);
                                    return;
                                },
                                (progress) =>
                                {
                                    // 読み込み中の進捗コールバック
                                },
                                (error) =>
                                {
                                    // エラー発生時のコールバック
                                    VroidAdds(ID, null);
                                    Debug.Log("Vroid" + ID + "ロード失敗" + error.Message);
                                    End.Invoke(false);
                                }
                            );
                        },
                        (error) =>
                        {
                            VroidAdds(ID, null);
                            Debug.Log("Vroid" + ID + "ロード失敗" + error.message);
                            End.Invoke(false);
                        });
                    },
                (error) =>
                {
                    /*ログイン失敗時*/
                    VroidAdds(ID, null);
                    Debug.Log("Vroid" + ID + "ログイン失敗" + error.Message);
                    End.Invoke(false);
                }
            );
        }
    }
}

