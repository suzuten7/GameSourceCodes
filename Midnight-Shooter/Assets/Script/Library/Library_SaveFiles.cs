using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Library_SaveFiles : MonoBehaviour
{
    static public string PathBase
    {
        get
        {
            var name = Application.isEditor ? "Save_Edit" : "Save_Bulid";
#if UNITY_EDITOR
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string projectName = Path.GetFileName(projectPath);
            name += "_" + projectName;
#endif
            return Application.persistentDataPath + "/" + name;
        }
    }
    static public void SaveFile(string folder, string fileName, string dataStr)
    {
        var fpath = PathBase;
        if (folder != "") fpath += "/" + folder;
        try
        {
            if (!Directory.Exists(fpath))
            {
                Directory.CreateDirectory(fpath);
            }
            var writer = new StreamWriter(fpath + "/" + fileName);
            writer.Write(dataStr);
            writer.Flush();
            writer.Close();
        }
        catch
        {
            Debug.Log("保存失敗" + fpath + "/" + fileName);
        }
    }
    static public string LoadFileStr(string folder, string fileName, string defalutStr = "")
    {
        try
        {
            var fpath = PathBase;
            if (folder != "") fpath += "/" + folder;
            var reader = new StreamReader(fpath + "/" + fileName);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }
        catch
        {
            return defalutStr;
        }
    }
    static public int LoadFileInt(string folder, string fileName,int defalutVal = 0)
    {
        var str = LoadFileStr(folder, fileName, "");
        return int.TryParse(str,out var val) ? val: defalutVal;
    }
    static public float LoadFileFloat(string folder, string fileName, float defalutVal = 0)
    {
        var str = LoadFileStr(folder, fileName, "");
        return float.TryParse(str, out var val) ? val : defalutVal;
    }

    static public void SaveDelete(string folder)
    {
        var fpath = PathBase + "/" + folder;
        if (Directory.Exists(fpath))
        {
            Directory.Delete(fpath,true);
        }
    }
}
