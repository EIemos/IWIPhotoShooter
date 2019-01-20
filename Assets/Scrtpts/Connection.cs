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
    public PhotoInfo(Texture2D texture, PhotoClass photoClass, int id) {
        Texture = texture;
        ID = id;
        PhotoClass = photoClass;
    }

    public int ID { get; set; }
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

public static class Config {
    public static bool isLogged = false;
    public static int numberOfPictures = 25;
    public static string deviceID = 6.ToString();
    public static string collection = "faces";
    public static string serverUrl = "https://kask.eti.pg.gda.pl/cenhive";
    public static string loginUrl = serverUrl + "/api.php?o=login";
    public static string getDataUrl = serverUrl + "/api.php?o=getmanydata";
    public static string sendAnwsersUrl = serverUrl + "/api.php?o=sendmanyanswers";
    public static readonly TestOnlineConnection Connection = new TestOnlineConnection();
}

public class TestOnlineConnection {

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

        public List<string> getMainClassPhotoUrls(PhotoClass photoClass) {
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
        form.AddField("collection", Config.collection);
        form.AddField("device_id", Config.deviceID);
        form.AddField("phrases_count", Config.numberOfPictures);

        using (UnityWebRequest www = UnityWebRequest.Post(Config.getDataUrl, form)) {
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

    IEnumerator SendCorrectPictures(IEnumerable<int> selected, IEnumerable<int> notSelected) {
        WWWForm form = new WWWForm();
        form.AddField("collection", Config.collection);
        form.AddField("device_id", 1);
        form.AddField("answer_time", 15523);
        foreach (var number in selected)
            form.AddField("selected_phrases[]", number);
        foreach (var number in notSelected)
            form.AddField("not_selected_phrases[]", number);


        using (UnityWebRequest www = UnityWebRequest.Post(Config.sendAnwsersUrl, form)) {
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
        foreach (var link in response.getMainClassPhotoUrls(faces)) {
            yield return GetPhoto(texture => facePhotos.Add(new PhotoInfo(texture, faces,1)), link);
        }

        var other = new PhotoClass("Other");
        var otherPhotos = new List<PhotoInfo>();
        foreach (var link in response.getOtherClassPhotoUrls()) {
            yield return GetPhoto(texture => otherPhotos.Add(new PhotoInfo(texture, other,2)), link);
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

        yield return SendCorrectPictures(new[] { 209, 237, 218 }, new[] { 222, 234, 220 });

        Debug.Log(string.Format("Class {0} classified {1} photos", class1.ClassName, toSend1.Count()));
        Debug.Log(string.Format("Class {0} classified {1} photos", class2.ClassName, toSend2.Count()));
        yield return null;
    }

    public IEnumerator LogIn(Action<bool> callbackResult, string login, string password) {
        WWWForm form = new WWWForm();
        form.AddField("login", login);
        form.AddField("password", password);
        form.AddField("device_id", Config.deviceID);
        Debug.Log("POST sucasdasdadscessful!");
        callbackResult(true);

        using (UnityWebRequest www = UnityWebRequest.Post(Config.loginUrl, form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError) {
                Debug.Log(www.error);
                callbackResult(false);
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
                callbackResult(!www.isHttpError);
            }
        }

        yield return 0;
    }
}


