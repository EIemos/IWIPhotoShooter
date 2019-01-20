using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PhotoClass {

    public PhotoClass(string ClassName) {
        this.ClassName = ClassName;
    }

    public string ClassName { get; set; }
}

public class PhotoInfo {
    public PhotoInfo(Texture2D texture, PhotoClass photoClass) {
        Texture = texture;
        PhotoClass = photoClass;
    }

    public Texture2D Texture { get; set; }
    public PhotoClass PhotoClass { get; set; }
}

public class GameInput {
    public PhotoClass Class1 { get; set; }
    public PhotoClass Class2 { get; set; }
    public List<PhotoInfo> Photos { get; set; }
}

public class GameOutput {
    public int Points { get; set; }
    public List<PhotoInfo> Class1Selection = new List<PhotoInfo>();
    public List<PhotoInfo> Class2Selection = new List<PhotoInfo>();
    private readonly Dictionary<PhotoClass, List<PhotoInfo>> Selection = new Dictionary<PhotoClass, List<PhotoInfo>>();

    public GameOutput(GameInput input) {
        Selection.Add(input.Class1, Class1Selection);
        Selection.Add(input.Class2, Class2Selection);
    }

    public void Assign(PhotoClass photoClass, PhotoInfo photoInfo) {
        Selection[photoClass].Add(photoInfo);
    }
}

public interface IConnection {
    bool LogIn(string username, string password);
    GameInput GetGameInput();
    void HandleOutput(GameOutput output, GameInput input);
}

public static class Config {
    public static readonly TestOnlineConnection Connection = new TestOnlineConnection();
}

//public class OfflineConnection  {

//    public GameInput GetGameInput() {
//        var class1 = new PhotoClass { ClassName = "Cats" };
//        var photos1 = new[] { "c1", "c2", "c3" }.Select(name => new PhotoInfo { Texture = Resources.Load<Texture2D>(name), PhotoClass = class1.ClassName });

//        var class2 = new PhotoClass { ClassName = "Dogs" };
//        var photos2 = new[] { "d1", "d2", "d3" }.Select(name => new PhotoInfo { Texture = Resources.Load<Texture2D>(name), PhotoClass = class2.ClassName });

//        return new GameInput {
//            Photos = photos1.Concat(photos2).ToList(),
//            Class1 = class1,
//            Class2 = class2
//        };
//    }

//    public void HandleOutput(GameOutput output, GameInput input) {

//        if (output.Points > 100) {
//            Debug.Log("Archivement \"Get 100 Points.\" Unlocked!");
//        }

//        var class1 = input.Class1;
//        var class2 = input.Class2;
//        var selected1 = output.Class1Selection.Distinct();
//        var selected2 = output.Class2Selection.Distinct();

//        var toSend1 = selected1.Except(selected2);
//        var toSend2 = selected2.Except(selected1);

//        Debug.Log(string.Format("Class {0} classified {1} photos", class1.ClassName, toSend1.Count()));
//        Debug.Log(string.Format("Class {0} classified {1} photos", class2.ClassName, toSend2.Count()));
//        return;
//    }

//    public bool LogIn(string username, string password) {
//        Debug.Log(string.Format("Login {0} Password {1}", username, password));
//        return true;
//    }
//}



public class TestOnlineConnection {
    private static readonly string VERSION = "3.0";
    private string deviceId;

    private string getDeviceId(string username)
    {
        if (deviceId == null)
        {
            this.deviceId = RegisterDevice(username);
        }
        return deviceId;
    }

    [Serializable]
    public class JsonResponse {
        [Serializable]
        public class Question {
            public string type;
            public string data;
        }

        [Serializable]
        public class Phrases {
            public int id;
            public Question[] media;
        }

         public Question question;
         public int[] correct_phrases;
         public Phrases[] phrases;

        public List<string> getMainClassPhotoUrls() {
            var correctID = correct_phrases;
            var links = phrases.Where(p => correctID.Contains(p.id)).Select(p => p.media[0].data.Replace("\\", string.Empty)).ToList();
            return links;
        }

        public List<string> getOtherClassPhotoUrls() {
            var correctID = correct_phrases;
            var links = phrases.Where(p => !correctID.Contains(p.id)).Select(p => p.media[0].data.Replace("\\", string.Empty)).ToList();
            return links;
        }

    }

    IEnumerator GetDataFromServer(Action<JsonResponse> callback, string collection, int number) {
        WWWForm form = new WWWForm();
        form.AddField("collection", collection);
        form.AddField("phrases_count", number.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post("https://kask.eti.pg.gda.pl/cenhive/api.php?o=getmanydata", form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError) {
                Debug.Log(www.error);
            } else {
                Debug.Log("POST successful!");
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders()) {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
                var response = JsonUtility.FromJson<JsonResponse>(www.downloadHandler.text);
                callback(response);
                
            }
        }
    }



    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight) {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        var nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        return nTex;

    }

    private IEnumerator GetPhoto(System.Action<Texture2D> callback, string url) {
        using (WWW www = new WWW(url)) {
            yield return www;
            var texture = www.texture;
            var maxSize = 200f;
            var max = Mathf.Min(texture.width, texture.height);
            var k = maxSize / max;
            var newWidth = (int)(texture.width * k);
            var newHeight = (int)(texture.height * k);
            texture = Resize(texture, newWidth, newHeight);
            yield return null;
            callback(texture);
        }
    }

    public IEnumerator GetEnumeratorDataInput(System.Action<GameInput> callback) {

        JsonResponse response = null;
        yield return GetDataFromServer(r => response = r, "faces", 25);

        var faces = new PhotoClass("Faces");
        var facePhotos = new List<PhotoInfo>();
        foreach (var link in response.getMainClassPhotoUrls()) {
            yield return GetPhoto(texture => facePhotos.Add(new PhotoInfo(texture, faces)), link);
        }

        var other = new PhotoClass("Other");
        var otherPhotos = new List<PhotoInfo>();
        foreach (var link in response.getOtherClassPhotoUrls()) {
            yield return GetPhoto(texture => otherPhotos.Add(new PhotoInfo(texture, other)), link);
        }

        var photos = facePhotos.Concat(otherPhotos).ToList();

        var input = new GameInput {
            Photos = photos,
            Class1 = faces,
            Class2 = other
        };

        callback(input);
    }

    public IEnumerator HandleOutput(GameOutput output, GameInput input) {

        if (output.Points > 100) {
            Debug.Log("Archivement \"Get 100 Points.\" Unlocked!");
        }

        var class1 = input.Class1;
        var class2 = input.Class2;
        var selected1 = output.Class1Selection.Distinct();
        var selected2 = output.Class2Selection.Distinct();

        var toSend1 = selected1.Except(selected2);
        var toSend2 = selected2.Except(selected1);

        Debug.Log(string.Format("Class {0} classified {1} photos", class1.ClassName, toSend1.Count()));
        Debug.Log(string.Format("Class {0} classified {1} photos", class2.ClassName, toSend2.Count()));
        yield return null;
    }

    public bool LogIn(string username, string password) {
        WWWForm form = new WWWForm();
        form.AddField("login", username);
        form.AddField("password", password);
        form.AddField("device_id", getDeviceId(username));

        using (UnityWebRequest www = UnityWebRequest.Post("https://kask.eti.pg.gda.pl/cenhive/api.php?o=login", form))
        {
            www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                return false;
            }
            else
            {
                Debug.Log("POST successful!");
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders())
                {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
                var response = JsonUtility.FromJson<JsonResponse>(www.downloadHandler.text);
                return true;
                //callback(response);

            }

        }
        
        Debug.Log(string.Format("Login {0} Password {1}", username, password));
    }
    public string RegisterDevice(string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", username);
        form.AddField("version", VERSION);

        using (UnityWebRequest www = UnityWebRequest.Post("https://kask.eti.pg.gda.pl/cenhive/api.php?o=register", form))
        {
            www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                throw new Exception("Cannot register device");
            }
            else
            {
                Debug.Log("POST successful!");
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders())
                {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
                return www.downloadHandler.text;
                //callback(response);

            }

        }
    }
}


