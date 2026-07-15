namespace UIs
{
    using UnityEngine;
    using UnityEngine.UI;
    using Firebase;
    using Firebase.Auth;
    using Firebase.Firestore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TMPro;
    using System;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_FireBaseSet;
    using static UIs.UI_System;
    public class UI_CloudSave : MonoBehaviour
    {
        public Image[] ModeButtonBacks;
        int Mode = 0;
        public TMP_InputField curAccountInput;   // 現アカウント名（メール）
        public TMP_InputField passwordInput; //パスワード
        public TMP_InputField newAccountInput;   // 新アカウント名（メール）

        public Toggle localSaveTo;

        public TMP_InputField nameInput;      // 保存する文字列

        public Button saveButton;
        public Button loadButton;
        public Button changeButton;

        public TextMeshProUGUI lastTimeTx;
        public TextMeshProUGUI outLogTx;

        bool lSave = false;
        long lastTime = 0;
        bool saveWaringFlag = false;
        bool loadWaringFlag = false;

        void Start()
        {
            outLogTx.text = "";
            ModeChange(0);
            //保持設定
            lSave = PlayerPrefs.GetInt("LCS_If_" + SaveNameGet, 0) == 1;
            localSaveTo.isOn = lSave;
            if (lSave)
            {
                curAccountInput.text = PlayerPrefs.GetString("LCS_Account_" + SaveNameGet, "");
                passwordInput.text = PlayerPrefs.GetString("LCS_Password_" + SaveNameGet, "");
            }
            lastTime = long.TryParse(PlayerPrefs.GetString("LCS_LTime_" + SaveNameGet, ""),out var oLT) ? oLT : 0;
            LTimeTxSet();
        }
        private void Update()
        {
            if (lSave != localSaveTo.isOn)
            {
                lSave = localSaveTo.isOn;
                PlayerPrefs.SetInt("LCS_If_" + SaveNameGet, lSave ? 1 : 0);
            }
            else LSaved();
        }
        void LTimeTxSet()
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(lastTime).LocalDateTime;
            lastTimeTx.text = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        }
        void LSaved()
        {
            if (lSave)
            {
                PlayerPrefs.SetString("LCS_Account_" + SaveNameGet, curAccountInput.text);
                PlayerPrefs.SetString("LCS_Password_" + SaveNameGet, passwordInput.text);
            }
            else
            {
                PlayerPrefs.SetString("LCS_Account_" + SaveNameGet, "");
                PlayerPrefs.SetString("LCS_Password_" + SaveNameGet, "");
            }
        }
        public void ModeChange(int m)
        {
            Mode = m;
            for (int i = 0; i < ModeButtonBacks.Length; i++)
            {
                ModeButtonBacks[i].color = OnColors(i == Mode);
            }
            bool[] actives;
            switch (Mode)
            {
                default:
                    actives = new bool[] { true, false, true, true, true, false };
                    break;
                case 1:
                    actives = new bool[] { false,false,false,false,false,true };
                    break;
                case 2:
                    actives = new bool[] { true,true,false,false,false,true };
                    break;
            }
            passwordInput.gameObject.SetActive(actives[0]);
            newAccountInput.gameObject.SetActive(actives[1]);
            localSaveTo.gameObject.SetActive(actives[2]);
            saveButton.gameObject.SetActive(actives[3]);
            loadButton.gameObject.SetActive(actives[4]);
            changeButton.gameObject.SetActive(actives[5]);
        }

        public async void OnSaveClicked()
        {
            loadWaringFlag = false;
            // メールアカウントでログイン or 作成
            var uid = await GetUid();
            if (uid == "")
            {
                Debug.Log("アカウントエラー");
                outLogTx.text = "<color=#FF0000>アカウントエラー</color>";
                return;
            }

            // 入力文字列を保存
            outLogTx.text = "セーブ中";
            try
            {
                var loadCheckSnap = await FireBaseLoadSnap("CSaves", uid + "_Check");
                if (loadCheckSnap != null)
                {
                    var loadLastTime = (long)FireBaseFiledGet(loadCheckSnap, "Time", 0);
                    if (!saveWaringFlag && lastTime < loadLastTime)
                    {
                        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(loadLastTime).LocalDateTime;
                        outLogTx.text = "※新しいデータ\n" + dateTime.ToString("yyyy/MM/dd HH:mm:ss"); ;
                        saveWaringFlag = true;
                        return;
                    }
                }
                saveWaringFlag = false;
                var CheckValues = new Dictionary<string, object>
                {
                    {"End",false },
                    { "Time", DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
                    {"Name", nameInput.text}
                };
                await FireBaseSave("CSaves", uid + "_Check", CheckValues);

                await FireBaseSave("CSaves", uid + "_State","State", JsonUtility.ToJson(GetSave_State));

                var CharaValues = new Dictionary<string, object>
                {
                    { "Chara_Count", GetSave_Charas.Count},
                };
                for (int i = 0; i < GetSave_Charas.Count; i++)
                {
                    CharaValues.Add("Chara_" + i.ToString("D3"), JsonUtility.ToJson(GetSave_Charas[i]));
                }
                await FireBaseSave("CSaves", uid + "_Chara", CharaValues);

                var Img2DValues = new Dictionary<string, object>
                {
                    { "Img2D_Count", GetSave_2DImages.Count},
                };
                for (int i = 0; i < GetSave_2DImages.Count; i++)
                {
                    Img2DValues.Add("Img2D_" + i.ToString("D3"), JsonUtility.ToJson(GetSave_2DImages[i]));
                }
                await FireBaseSave("CSaves", uid + "_Img2D", Img2DValues);

                await FireBaseSave("CSaves", uid + "_Check", "End", true);
                outLogTx.text = "<color=#FFFF00>セーブ完了</color>";
            }
            catch (Exception ex)
            {
                Debug.Log("セーブエラー発生:" + ex.Message);
                outLogTx.text = "<color=#FFFF00>セーブエラー発生</color>";
            }
            LSaved();

            PlayerPrefs.SetString("LCS_LTime_" + SaveNameGet, lastTime.ToString());
            LTimeTxSet();
        }
        public async void OnLoadClicked()
        {
            saveWaringFlag = false;
            // メールアカウントでログイン or 作成
            var uid = await GetUid();
            if (uid == "")
            {
                Debug.Log("アカウントエラー");
                outLogTx.text = "<color=#FF0000>アカウントエラー</color>";
                return;
            }
            // 読み込み
            outLogTx.text = "ロード中";
            var loadCheckSnap = await FireBaseLoadSnap("CSaves", uid + "_Check");
            if (loadCheckSnap != null)
            {
                var loadLastTime = FireBaseFiledGet(loadCheckSnap, "Time", (long)0);
                var loadNameStr = FireBaseFiledGet(loadCheckSnap, "Name", "");
                if (!loadWaringFlag)
                {
                    var loadEnd = FireBaseFiledGet(loadCheckSnap, "End", false);
                    if (!loadEnd)
                    {
                        outLogTx.text += "\n※セーブエラー";
                        loadWaringFlag = true;
                    }
                    if (lastTime >= loadLastTime)
                    {
                        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(loadLastTime).LocalDateTime;
                        outLogTx.text += "\n※過去データ\n" + dateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        loadWaringFlag = true;
                    }
                    if (nameInput.text != loadNameStr)
                    {
                        outLogTx.text += "\n※名前が異なります\n「" + loadNameStr + "」";
                        loadWaringFlag = true;
                    }
                    if (loadWaringFlag) return;
                }
                loadWaringFlag = false;
                var loadStateSnap = await FireBaseLoadSnap("CSaves", uid + "_State");
                if (loadStateSnap != null)
                {
                    var loadState = FireBaseFiledGet(loadStateSnap, "State", "");
                    if (loadState != "") DataSet(JsonUtility.FromJson<Class_Save_State>(loadState));
                }
                var loadCharaSnap = await FireBaseLoadSnap("CSaves", uid + "_Chara");
                if (loadCharaSnap != null)
                {
                    var Charas = new List<Class_Save_Chara>();
                    var loadCharaCount = FireBaseFiledGet(loadCharaSnap, "Chara_Count", 0);
                    for (int i = 0; i < loadCharaCount; i++)
                    {
                        var loadCharas = FireBaseFiledGet(loadCharaSnap, "Chara_" + i.ToString("D3"), "");
                        Charas.Add(JsonUtility.FromJson<Class_Save_Chara>(loadCharas));
                    }
                    DataSet(Charas);
                }
                var loadImg2DSnap = await FireBaseLoadSnap("CSaves", uid + "_Img2D");
                if (loadImg2DSnap != null)
                {
                    var Img2Ds = new List<Class_Save_2DImageBase>();
                    var loadImg2DCount = FireBaseFiledGet(loadImg2DSnap, "Img2D_Count", 0);
                    for (int i = 0; i < loadImg2DCount; i++)
                    {
                        var loadImg2Ds = FireBaseFiledGet(loadImg2DSnap, "Img2D_" + i.ToString("D3"),"");
                        Img2Ds.Add(JsonUtility.FromJson<Class_Save_2DImageBase>(loadImg2Ds));
                    }
                    DataSet(Img2Ds);
                }
                outLogTx.text = "<color=#FFFF00>ロード完了</color>";
                LSaved();
                lastTime = loadLastTime;
                PlayerName = loadNameStr;
                nameInput.text = loadNameStr;
                PlayerPrefs.SetString("Name_" + SaveNameGet, loadNameStr);
                PlayerPrefs.SetString("LCS_LTime_" + SaveNameGet, lastTime.ToString());
                LTimeTxSet();
            }
            else
            {
                outLogTx.text = "<color=#FF0000>データがありません</color>";
            }
        }

        async Task<string> GetUid()
        {
            outLogTx.text = "ログイン中";
            FirebaseUser user = await EmailAuth(curAccountInput.text);
            if (user != null) return user.UserId;
            return "";
        }
        // メールログイン
        async Task<FirebaseUser> EmailAuth(string email)
        {
            var FB = await FireBaseSystemGet();
            FirebaseUser user = null;
            try
            {
                //ログイン
                var result = await FB.Item1.SignInWithEmailAndPasswordAsync(email, passwordInput.text);
                user = result.User;
                Debug.Log("ログイン成功: " + user.UserId);
            }
            catch (FirebaseException elog)
            {
                Debug.Log("アカウントが無いため作成します" + elog.Message);
                try
                {
                    // アカウントがなければ作成
                    var result = await FB.Item1.CreateUserWithEmailAndPasswordAsync(email, passwordInput.text);
                    user = result.User;
                    Debug.Log("アカウント作成: " + user.UserId);
                }
                catch (FirebaseException ecre)
                {
                    Debug.LogError("作成エラー" + ecre.Message);
                }
            }
            return user;
        }
        public async void ChangeSend()
        {
            switch (Mode)
            {
                case 1:
                    await ChangePassEmail();
                    break;
                case 2:
                    await ChangeAdressEmail();
                    break;
            }
        }
        //　メールパスワード変更
        async Task ChangePassEmail()
        {
            var FB = await FireBaseSystemGet();
            outLogTx.text = "変更要求中";
            var email = curAccountInput.text;
            try
            {
                await FB.Item1.SendPasswordResetEmailAsync(email);
                Debug.Log("パスワード再設定メールを送信しました: " + email);
                outLogTx.text = "<color=#FFFF00>要求送信成功</color>";
            }
            catch (FirebaseException e)
            {
                Debug.LogError("メール送信失敗: " + e.Message);
                outLogTx.text = "<color=#FF0000>要求送信失敗</color>";
            }
        }
        // メールアドレス変更
        async Task ChangeAdressEmail()
        {
            var FB = await FireBaseSystemGet();
            outLogTx.text = "変更要求中";
            try
            {
                var result = await FB.Item1.SignInWithEmailAndPasswordAsync(curAccountInput.text, passwordInput.text);
                var user = result.User;
                // 再認証
                var credential = EmailAuthProvider.GetCredential(curAccountInput.text, passwordInput.text);


                await user.ReauthenticateAsync(credential);

                // メールアドレス変更
                await user.SendEmailVerificationBeforeUpdatingEmailAsync(newAccountInput.text);
                Debug.Log("アドレス変更メールを送信しました: " + user.Email);
                outLogTx.text = "<color=#FFFF00>要求送信成功</color>";
            }
            catch (FirebaseException e)
            {
                Debug.LogError("メール送信失敗: " + e.Message);
                outLogTx.text = "<color=#FF0000>要求送信失敗</color>";
            }
        }

        async public void TestConnect()
        {
            outLogTx.text = "テスト接続開始";
            var B = await FireBaseSystemGet();
            outLogTx.text = "テスト接続完了:" + (B.Item1 != null) + "_" + (B.Item2 != null);
            Debug.Log("Connect:" + (B.Item1 != null) + "_" + (B.Item2 != null));

        }
        async public void TestLoad(string s)
        {
            outLogTx.text = "テストロード開始";
            var Snap = await FireBaseLoadSnap("Test", "A");
            if (Snap != null)
            {
                outLogTx.text = "テストロード完了:" + FireBaseFiledGet(Snap, s, "null!!!");
                Debug.Log(FireBaseFiledGet(Snap, s, "null!!!"));
            }
            else
            {
                Debug.Log("テスト無し");
            }
        }
    }
}
