//------------------------------------------------------------------------------
//GENERAL UTIL FUNCTION for UNITY PROFJECT
//MAKE BY SUGA STUDIO
//@2016
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class SGUtils
{
    public delegate bool BubbleSortCompare<T>(T a, T b);
    public static void BubbleSort<T>(List<T> list, BubbleSortCompare<T> isHigher)
    {
        if (list == null)
            return;
        for (int i = list.Count - 1; i > 0; i--)
        {
            for (int j = 0; j <= i - 1; j++)
            {
                if (isHigher(list[i], list[j]))
                {
                    var highValue = list[i];

                    list[i] = list[j];
                    list[j] = highValue;
                }
            }
        }
    }

    public static void BubbleSort<T>(T[] list, BubbleSortCompare<T> compare)
    {
        if (list == null)
            return;
        var length = list.Length;
        for (int i = length - 1; i > 0; i--)
        {
            for (int j = 0; j <= i - 1; j++)
            {
                if (compare(list[i], list[j]))
                {
                    var highValue = list[i];

                    list[i] = list[j];
                    list[j] = highValue;
                }
            }
        }
    }


    /// <summary>
    /// Clears all child of parent tranform
    /// </summary>
    /// <param name="parent">Parent.</param>
    public static void ClearAllChild(Transform parent)
    {
        var children = new List<GameObject>();

        foreach (Transform child in parent)
            children.Add(child.gameObject);

        parent.DetachChildren();

        if (Application.isEditor)
        {
            children.ForEach(child => GameObject.DestroyImmediate(child));
        }
        else
        {
            children.ForEach(child => GameObject.Destroy(child));
        }

        Resources.UnloadUnusedAssets();//giam mem sau khi destroy list
    }

    /// <summary>
    /// Sets the active all child.
    /// </summary>
    /// <param name="parent">Parent.</param>
    /// <param name="active">If set to <c>true</c> active.</param>
    public static void SetActiveAllChild(GameObject parent, bool active)
    {
        //      Debug.LogWarning("SetActiveAllChild " + parent.name + ":" + active);
        foreach (Transform child in parent.transform)
        {
            //  Debug.LogWarning("child " + child.name + ":" + active);
            child.gameObject.SetActive(active);
        }
    }

    public static GameObject AddChild(GameObject parent, string name = "NewGO")
    {
        GameObject c = new GameObject();
        c.name = name;
        c.transform.SetParent(parent.transform);
        c.transform.localPosition = Vector3.zero;
        c.transform.localScale = Vector3.one;
        c.transform.localEulerAngles = Vector3.zero;

        return c;
    }

    public static T InstantiateObject<T>(T prefab, Transform parent) where T : Component
    {
        if (prefab != null)
        {
            T go = GameObject.Instantiate<T>(prefab) as T;
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
            return go;
        }
        return null;
    }

    public static List<T> SetupUIPool<T>(T prefab, ref List<T> pool, Transform container, int count) where T : MonoBehaviour
    {
        List<T> result = new List<T>();

        for (int index = 0; index < count; index++)
        {
            T item = null;
            if (index < pool.Count)
            {
                item = pool[index];
            }
            else
            {
                item = Object.Instantiate(prefab, container);
                pool.Add(item);
            }
            item.gameObject.SetActive(true);
            result.Add(item);
        }

        for (int index = count; index < pool.Count; index++)
        {
            pool[index].gameObject.SetActive(false);
        }

        return result;
    }

    public static Component FindComponentInParent(Transform child, Type type)
    {
        Transform p = child.parent;
        if (p == null)
        {
            return null;
        }

        if (p.GetComponent(type) != null)
            return p.GetComponent(type);

        return FindComponentInParent(p, type);
    }

    public static Texture LoadTexture(string path)
    {
        //Debug.Log("LoadTexture from resources" + path);
        try
        {
            return Resources.Load(path, typeof(Texture)) as Texture;
        }
        catch (Exception e)
        {
            Debug.LogError("Cannot load texture " + path + "with error " + e.Message);
        }
        return null;
    }

    public static TextAsset LoadTextAsset(string path)
    {
        //Debug.Log("LoadTextAsset " + path);
        try
        {
            return Resources.Load(path, typeof(TextAsset)) as TextAsset;
        }
        catch (Exception e)
        {
            Debug.LogError("Cannot load text asset " + path + "with error " + e.Message);
        }
        return null;
    }

    public static IEnumerator LoadPrefabAsync<T>(string path, System.Action<T> cb) where T : Component
    {
        var request = Resources.LoadAsync<T>(path);
        yield return request;
        if (request.asset != null)
        {
            T prefab = request.asset as T;
            cb(prefab);
        }
        else
            cb(null);
    }

    public static GameObject LoadPrefab(string path)
    {
        try
        {
            GameObject ret = Resources.Load(path, typeof(GameObject)) as GameObject;
            if (ret == null)
                Debug.LogError("Cannot load prefab " + path);
            return ret;
        }
        catch (Exception e)
        {
            Debug.LogError("Cannot load prefab " + path + "with error " + e.Message);
        }
        return null;
    }

    public static GameObject LoadGameObject(string path)
    {
        return LoadGameObject(null, path);
    }

    public static GameObject LoadGameObject(Transform parent, string path)
    {
        GameObject prefab = LoadPrefab(path);
        if (prefab == null) return null;

        GameObject go = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        return go;

    }

    public static GameObject InstantiateGameObject(GameObject prefab, Transform parent)
    {
        if (prefab == null) return null;

        GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go;

    }

    //    public static void SetLayerRecursively(GameObject go, int layer)
    //    {
    //        go.layer = layer;
    //        int count = go.transform.childCount;
    //        for(int i = 0; i < count; i++)
    //        {
    //            SetLayerRecursively(go.transform.GetChild(i).gameObject, layer);
    //        }        
    //    }



#if UNITY_EDITOR
    public static GameObject LoadPrefabInAsset(Transform parent, string name)
    {
        GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Cannot load asset at path " + name);
#endif
            return null;
        }
        GameObject go = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go;
    }

    public static Sprite LoadSpriteInAsset(string path)
    {
        Debug.Log(path);
        Sprite tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
#if UNITY_EDITOR
        if (tex == null)
            Debug.LogError("Khong tim thay " + path);
#endif
        return tex;
    }

    public static TextAsset LoadTextInAsset(string path)
    {
        Debug.Log(path);
        TextAsset ret = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
#if UNITY_EDITOR
        if (ret == null)
            Debug.LogError("Khong tim thay " + path);
#endif
        return ret;
    }
#endif

    // Takes a screenshot and puts it in the Application.persistentDataPath directory (which is Documents on iOS)
    public static void SaveToFile(Texture2D tex, string filename)
    {
        var bytes = tex.EncodeToPNG();
        string path = Application.dataPath + "/" + filename + ".png";
        File.WriteAllBytes(path, bytes);
        Debug.Log("Finish save file to " + path);
    }

    public static IEnumerator UploadToServer(Texture2D tex, string name, string serverUrl, Action<bool> callback)
    {
        var bytes = tex.EncodeToPNG();
        WWWForm form = new WWWForm();
        Debug.Log("Start upload to " + serverUrl);

        form.AddField("frameCount", Time.frameCount.ToString());
        form.AddBinaryData("fileUpload", bytes, name + ".png", "image/png");
        using (var www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error upload Screenshot:" + www.error);
                if (callback != null)
                    callback(false);
            }
            else
            {
                if (callback != null)
                    callback(true);
                Debug.Log("Upload image result");
            }
        }
    }

    // Takes a screenshot and puts it in the Application.persistentDataPath directory (which is Documents on iOS)
    public static IEnumerator TakeScreenShotToTexture(Rect rect, Action<Texture2D> callback)
    {
        yield return new WaitForSeconds(1);
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(1);
        try
        {
            Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            Debug.Log("Capture rec " + rect);
            // Read screen contents into the texture
            tex.ReadPixels(rect, 0, 0);
            tex.Apply();
            callback(tex);
        }
        catch (Exception ex)
        {
            Debug.LogError("TakeScreenShotToTexture ERROR " + ex.StackTrace);
            callback(null);
        }
    }

    public static IEnumerator TakeScreenShotAndUpload(MonoBehaviour mono, Rect rect, string name, string serverUrl, Action<bool> callback)
    {
        Debug.Log("TakeScreenShotAndUpload");
        yield return mono.StartCoroutine(TakeScreenShotToTexture(rect, tex => {
            if (tex)
            {
                //TextureScale.Bilinear(tex,100,100);
                Debug.Log("Finished TakeScreenShotAndUpload");
                callback(true);
                mono.StartCoroutine(UploadToServer(tex, name, serverUrl, null));
            }
            else
            {
                callback(false);
            }
        }));
    }


    static System.Random rand = new System.Random();

    /// <summary>
    /// Randoms the int. Not include max
    /// </summary>
    /// <returns>The int.</returns>
    /// <param name="max">Max.</param>
    public static int RandomInt(int max)
    {
        return rand.Next(max);
    }

    /// <summary>
    /// Randoms the int. Not include max
    /// </summary>
    /// <returns>The int.</returns>
    /// <param name="max">Max.</param>
    public static int RandomInt(int min, int max)
    {
        return rand.Next(min, max);
    }

    public static int GetRandomFormDistance(string str, string[] split)
    {
        string[] _data = str.Split(split, StringSplitOptions.RemoveEmptyEntries);
        try
        {
            int _min = int.Parse(_data[0]);
            int _max = int.Parse(_data[1]);
            return GetRandomFormDistance(_min, _max);
        }
        catch (Exception ex)
        {
            Debug.LogError("GetRandomFormDistance Error:" + ex.Message);
            return 0;
        }
    }
    public static int GetRandomFormDistance(string str)
    {
        string[] _split = new string[] { "-" };
        return GetRandomFormDistance(str, _split);
    }

    public static int GetRandomFormDistance(int min, int max)
    {
        if (min >= max)
        {
            return min;
        }
        else
        {
            return rand.Next(min, max);
        }
    }

    public static Color HexToColor(string hex, byte alpha = 255)
    {
        try
        {
            if (hex.Length == 7)
            {
                hex = hex.Substring(1, 6);
            }

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, alpha);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return Color.white;
        }
    }

    public static string ColorToHex(Color32 color)
    {
        return "#" + (color.r).ToString("X2") + (color.g).ToString("X2") + (color.b).ToString("X2");
    }

    public static int HexToInt(string hex)
    {
        if (hex.Length == 7)
        {
            hex = hex.Substring(1, 6);
        }

        return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }

    public static Color IntToColor(int colorValue)
    {
        string hex = "#" + colorValue.ToString("X6");
        Debug.LogError(hex);
        return HexToColor(hex);
    }

    public static int ColorToInt(Color color)
    {
        string hex = ColorToHex(color);
        return HexToInt(hex);
    }



    /// <summary>
    /// Ham random vi tri xung quanh vi tri duoc cung cap
    /// </summary>
    /// <returns>Tra ve vi tri random</returns>
    /// <param name="curPos">vi tri trung tam </param>
    /// <param name="radius">ban kinh random</param>
    public static Vector3 RandomPositionAroundPos(Vector3 curPos, float radius)
    {
        Vector3 result = Vector3.zero;

        return result;
    }

    public static void DestroyAllChild(Transform t)
    {
        var children = new List<GameObject>();

        foreach (Transform child in t)
        {
            children.Add(child.gameObject);
        }
        t.DetachChildren();

        children.ForEach(child => UnityEngine.Object.Destroy(child));
    }

    public static void DeleteAllChilds(Transform trans)
    {
        int childNum = trans.childCount;
        for (int i = childNum - 1; i >= 0; i--)
            GameObject.Destroy(trans.GetChild(i).gameObject);
        trans.DetachChildren();
    }
    public static Sprite ConvertToSprite(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
    public static string GetFullPath(Transform go)
    {
        string path = "";

        if (go == null)
        {
            return path;
        }
        else
        {
            if (go.transform.parent != null)
                path = GetFullPath(go.transform.parent);
            path += "/" + go.name;
        }

        return path;
    }

    public static IEnumerator LoadImageWWW(string url, Action<Texture2D> callback)
    {
        int countRequestImageTimeOut = 0;
        while (true)
        {
            countRequestImageTimeOut++;
            if (countRequestImageTimeOut >= 10)
            {
                callback(null);
                break;
            }
            using (var www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (!www.isNetworkError)
                {
                    var texture = DownloadHandlerTexture.GetContent(www);
                    callback(texture);
                    break;
                }
            }
        }
        yield return null;
    }

    [System.Diagnostics.Conditional("ASSERT")]
    public static void Assert(bool expression, string msgLabel = "", UnityEngine.Object c = null)
    {
        string contextName = c != null ? c.GetType().Name : "ASSERT";
        Debug.Log(string.Format("<b><color=#0080ff>[{3}]</color></b> <i><color=#ffff00>{0}</color></i>: <b><color={1}>{2}</color></b>", msgLabel, (expression ? "#00ff00" : "#ff5655"), (expression ? "OK" : "FAILED"), contextName), c);
    }

    [System.Diagnostics.Conditional("ASSERT")]
    public static void AssertEquals(this object obj, object other, string msgLabel = "", UnityEngine.Object c = null)
    {
        Assert(obj == other, "AssertEquals " + msgLabel, c);
    }

    [System.Diagnostics.Conditional("ASSERT")]
    public static void AssertNotEquals(this object obj, object other, string msgLabel = "", UnityEngine.Object c = null)
    {
        Assert(obj != other, "AssertNotEquals " + msgLabel, c);
    }

    public static string CreatLongUID()
    {
        return System.Guid.NewGuid().ToString();
    }

    public static string CreatShortUID()
    {
        string enc = Convert.ToBase64String(System.Guid.NewGuid().ToByteArray());
        enc = enc.Replace("/", "_");
        enc = enc.Replace("+", "-");
        return enc.Substring(0, 22);
    }

    public static Transform FindChildWithName(string name, Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
                return child;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindChildWithName(name, child);
            if (result != null)
                return result;
        }
        return null;
    }

    public static T FindCompChildByName<T>(string name, Transform parent) where T : Component
    {
        Transform trans = FindChildWithName(name, parent);
        if (trans != null)
            return trans.GetComponent<T>();
        return null;
    }

    public static string SecondsToHMS(int total, int format = 0)
    {
        int hours = total / 3600;
        int minutes = (total % 3600) / 60;
        int second = (total % 3600) % 60;

        string result = "";
        if (format == 0)
        {

            if (hours > 0)
                result += string.Format("{0:00}", hours) + ":";

            return result + string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", second);
        }
        else if (format == 1)
        {
            if (hours > 0)
                result += hours + "h ";
            if (minutes > 0)
                result += minutes + "m ";
            if (second > 0)
                result += second + "s ";
        }

        return result;
    }


    public static string MinutesToDHM(int total, string format = null)
    {
        int days = total / 1440;
        int hours = (total % 1440) / 60;
        int mins = (total % 1440) % 60;

        string result = "";
        if (format == null)
        {
            if (days > 0)
                result += days + "d ";
            if (hours > 0)
                result += hours + "h ";

            result += mins + "m ";
        }
        else
            result = string.Format(format, days, hours, mins);
        return result;
    }

    public static string CropString(string source, int len)
    {
        if (string.IsNullOrEmpty(source))
        {
            return "";
        }
        if (len >= source.Length - 3)
            return source;
        return source.Substring(0, len) + "...";
    }

    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static void SetSpriteLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Renderer trans in go.GetComponentsInChildren<Renderer>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static string MD5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    public static bool IsLowPerformanceDevice()
    {
        var ram = SystemInfo.systemMemorySize;
#if UNITY_ANDROID
        if (ram < 1500)
            return true;
        return false;
#elif UNITY_WEBGL
        if (ram < 2048)
            return true;
        return false;
#else//ios
        if (ram < 900)
            return true;
        return false;
#endif
    }


    public static Vector2 GetPivot(Sprite sprite)
    {
        Bounds bounds = sprite.bounds;
        var pivotX = -bounds.center.x / bounds.extents.x / 2 + 0.5f;
        var pivotY = -bounds.center.y / bounds.extents.y / 2 + 0.5f;
        return new Vector2(pivotX, pivotY);
    }

    public static Stack<T> CloneStack<T>(this Stack<T> original)
    {
        var arr = new T[original.Count];
        original.CopyTo(arr, 0);
        Array.Reverse(arr);
        return new Stack<T>(arr);
    }

    public static string GenerateUid()
    {
        return Guid.NewGuid().ToString();
    }

    public static Sprite GetSkillIcon(int id)
    {
        return Resources.Load<Sprite>("UI/SkillIcon/" + id);
    }

    public static bool IsEqualFloat(float a, float b, float diff = 0.01f)
    {
        var delta = Math.Abs(a - b);
        //Debug.LogError(delta < diff);
        return delta < diff;
    }

    public static string GetBundleVersionCode()
    {
#if UNITY_EDITOR && UNITY_ANDROID
        return "." + UnityEditor.PlayerSettings.Android.bundleVersionCode;
#elif UNITY_EDITOR && UNITY_IOS
        return "." + UnityEditor.PlayerSettings.iOS.buildNumber;
#elif UNITY_ANDROID
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
        var pInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", Application.identifier, 0);
        return "." + pInfo.Get<int>("versionCode");
#else
        return string.Empty;
#endif
    }

    public static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = rand.Next(i + 1);
            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }

    public static void FitImageSprite(Image img, float maxWidth, float maxHeight)
    {
        if (img.sprite != null)
        {
            if (img.sprite.rect.width < maxWidth && img.sprite.rect.height < maxHeight)
                img.SetNativeSize();
            else
                img.rectTransform.SetSize(new Vector2(maxWidth, maxHeight));
        }
        else
            img.rectTransform.SetSize(new Vector2(maxWidth, maxHeight));
    }

    public static Dictionary<int, float> ParseStringToDicInt_Float(string str)
    {
        var result = new Dictionary<int, float>();
        if (!string.IsNullOrEmpty(str))
        {

            var items = str.Split(';');

            if (items != null)
            {
                foreach (var iter in items)
                {
                    //Debug.LogError (iter);
                    var subs = iter.Split('_');
                    if (subs.Length == 2)
                    {
                        int id;
                        if (int.TryParse(subs[0], out id))
                        {
                            float quantity = 0;
                            if (float.TryParse(subs[1], out quantity))
                                result[id] = quantity;
                        }
                    }

                }
            }
        }
        return result;
    }

    public static Dictionary<int, int> ParseStringToDicInt(string str)
    {
        var result = new Dictionary<int, int>();
        if (!string.IsNullOrEmpty(str))
        {

            var items = str.Split(';');

            if (items != null)
            {
                foreach (var iter in items)
                {
                    //Debug.LogError (iter);
                    var subs = iter.Split('_');
                    if (subs.Length == 2)
                    {
                        int id;
                        if (int.TryParse(subs[0], out id))
                        {
                            int quantity = 0;
                            if (int.TryParse(subs[1], out quantity))
                                result[id] = quantity;
                        }
                    }

                }
            }
        }
        return result;
    }

    public static List<int> ParseStringToListInt(string str, char splitChar)
    {
        var result = new List<int>();
        if (!string.IsNullOrEmpty(str))
        {
            var items = str.Split(splitChar);

            if (items != null)
            {
                foreach (var iter in items)
                {
                    if (string.IsNullOrEmpty(iter))
                        continue;

                    int id;
                    if (int.TryParse(iter, out id))
                        result.Add(id);
                }
            }
        }
        return result;
    }

    public static List<string> ParseStringToList(string str, char splitChar)
    {
        var result = new List<string>();
        if (!string.IsNullOrEmpty(str))
        {
            var items = str.Split(splitChar);
            if (items != null)
            {
                foreach (var iter in items)
                {
                    if (!string.IsNullOrEmpty(iter))
                        result.Add(iter);
                }
            }
        }
        return result;
    }

    public static bool PolyContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            //if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
            //(p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
            //inside = !inside;

            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x <= (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }

    public static void ResizeBoxCollider2dFollowSpriteRenderer(BoxCollider2D boxCollider, SpriteRenderer sprRenderer)
    {
        if (boxCollider == null || sprRenderer == null)
            return;
        boxCollider.size = sprRenderer.bounds.size;
        Vector3 pivotOffsetWorldSpace = sprRenderer.bounds.center - sprRenderer.transform.position;
        pivotOffsetWorldSpace.x *= sprRenderer.transform.localScale.x;
        boxCollider.offset = pivotOffsetWorldSpace;
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }



    public static string ReverseString(string s)
    {
        char[] arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }



}


