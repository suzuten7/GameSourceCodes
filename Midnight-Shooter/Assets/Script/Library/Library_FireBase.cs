using Firebase;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
public class Library_FireBase : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI testOut;
    static FirebaseFirestore _db;

    static bool SetUpWait = false;
    async static public Task<(string,FirebaseFirestore)> FireBaseSystemGet()
    {
        var log = "FBLogs";
        while (SetUpWait)
        {
            await Task.Delay(100); // 100msごとに待機
        }

        if (_db != null)
        {
            log += "\nFB_接続済み";
            Debug.Log("FB_接続済み");
        }
        else
        {
            SetUpWait = true;
            try
            {
                // Firebase 初期化
                var status = await FirebaseApp.CheckAndFixDependenciesAsync();
                log += "\nFB_初期化状態:" + status;
                Debug.Log("FB_初期化状態:" + status);


#if UNITY_EDITOR || UNITY_STANDALONE
                //  PCは名前付きで作る（複数起動対策）
                string appName = Application.productName + "_" + System.Diagnostics.Process.GetCurrentProcess().Id;
                var options = FirebaseApp.DefaultInstance.Options;
                var app = FirebaseApp.Create(options, appName);
                log += "\nFB_App: " + appName;
                Debug.Log("FB_App: " + appName);
#else
        // AndroidはDefaultInstance使う
        var app = FirebaseApp.DefaultInstance;
#endif

                // 名前付きアプリから Auth / Firestore を取得
                _db = FirebaseFirestore.GetInstance(app);
                log += "\nFB_接続完了";
                Debug.Log("FB_接続完了");
            }
            catch (System.Exception e)
            {
                log += "\nFB_接続エラー: " + e;
                Debug.LogError("FB_接続エラー: " + e);
            }
            SetUpWait = false;
        }

        return (log,_db);
    }

    //フィールド単位でセーブ
    async static public Task<bool> FireBaseSave(string collection, string document, string field, object value)
    {
        var FBs = await FireBaseSystemGet();
        var FB = FBs.Item2;
        try
        {
            DocumentReference docRef = FB.Collection(collection).Document(document);
            Dictionary<string, object> update = new()
                {
                    { field, value }
                };
            await docRef.SetAsync(update, SetOptions.MergeFields(field));
            //Debug.Log($"{collection}/{document}/{field} セーブ完了-{value}");
            return true;
        }
        catch
        {
            //Debug.Log($"{collection}/{document}/{field} セーブ失敗-{value}");
            return false;
        }
    }

    async static public Task<bool> FireBaseSave(string ollection, string document, Dictionary<string, object> values)
    {
        var FBs = await FireBaseSystemGet();
        var FB = FBs.Item2;
        try
        {
            DocumentReference docRef = FB.Collection(ollection).Document(document);

            await docRef.SetAsync(values, SetOptions.MergeAll);
            //Debug.Log($"{collection}/{document}/{field} セーブ完了-{values}");
            return true;
        }
        catch
        {
            //Debug.Log($"{collection}/{document}/{field} セーブ失敗-{values}");
            return false;
        }
    }
    async static public Task<DocumentSnapshot> FireBaseLoadSnap(string coll, string documet)
    {
        var FBs = await FireBaseSystemGet();
        var FB = FBs.Item2;
        try
        {
            DocumentReference docRef = FB.Collection(coll).Document(documet);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            //Debug.Log($"{coll}/{documet} ロード完了");
            return snapshot.Exists ? snapshot : null;
        }
        catch
        {
            //Debug.Log($"{coll}/{documet} ロード失敗");
            return null;
        }
    }
    static public T FireBaseFiledGet<T>(DocumentSnapshot snap, string field, T defaultValue)
    {
        if (snap.Exists && snap.ContainsField(field))
        {
            try
            {
                object value = snap.GetValue<object>(field);

                // int
                if (typeof(T) == typeof(int))
                    return (T)(object)Convert.ToInt32(value);

                // float
                if (typeof(T) == typeof(float))
                    return (T)(object)Convert.ToSingle(value);

                // List / Array
                if (value is List<object> list)
                {
                    if (typeof(T) == typeof(List<int>))
                        return (T)(object)list.Select(x => Convert.ToInt32(x)).ToList();

                    if (typeof(T) == typeof(int[]))
                        return (T)(object)list.Select(x => Convert.ToInt32(x)).ToArray();

                    if (typeof(T) == typeof(List<float>))
                        return (T)(object)list.Select(x => Convert.ToSingle(x)).ToList();

                    if (typeof(T) == typeof(float[]))
                        return (T)(object)list.Select(x => Convert.ToSingle(x)).ToArray();
                }

                // fallback
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception e)
            {
                Debug.LogError($"型変換エラー: {field} → {typeof(T)}\n{e}");
            }
        }
        return defaultValue;
    }

    public static int HashGet(object obj)
    {
        return HashCompute(obj, 17);
    }

    static int HashCompute(object obj, int hash)
    {
        if (obj == null) return hash * 31;

        Type type = obj.GetType();

        // プリミティブ系
        if (type.IsPrimitive || obj is string || obj is decimal)
        {
            return hash * 31 + obj.GetHashCode();
        }

        // IEnumerable（Listなど）
        if (obj is IEnumerable enumerable && obj is not string)
        {
            foreach (var item in enumerable)
            {
                hash = HashCompute(item, hash);
            }
            return hash;
        }

        // クラス → フィールドを順序固定で取得
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                         .OrderBy(f => f.Name);

        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            hash = HashCompute(value, hash);
        }

        return hash;
    }

    async public void Test(bool load)
    {
        var FBs = await FireBaseSystemGet();
        var log = FBs.Item1;
        var FB = FBs.Item2;
        if (FB != null)
        {
            DocumentReference docRef = FB.Collection("Test").Document("t");
            if (!load)
            {
                var val = UnityEngine.Random.Range(0, 10000).ToString("D4");
                Dictionary<string, object> update = new()
                {
                    { "1", val }
                };
                try
                {
                    await docRef.SetAsync(update, SetOptions.MergeFields("1"));
                    log += "\nセーブ" + val;
                }
                catch (Exception ex)
                {
                    log += "\nエラー" + ex;
                }

            }
            else
            {
                try
                {
                    DocumentSnapshot snap = await docRef.GetSnapshotAsync();
                    var val = snap.GetValue<object>("1");
                    log += "\nロード" + (string)val;
                }
                catch (Exception ex)
                {
                    log += "\nエラー" + ex;
                }

            }
                
        }
        testOut.text = log;
    }
}
