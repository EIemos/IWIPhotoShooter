using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PhotoClass {
    public PhotoClass(string ClassName) { this.ClassName = ClassName; }
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
    public PhotoClass MainClass { get; set; }
    public PhotoClass OtherClass { get; set; }
    public List<PhotoInfo> Photos { get; set; }
}

public class GameOutput {
    public int Points { get; set; }
    public List<PhotoInfo> MainClassSelection = new List<PhotoInfo>();
    public List<PhotoInfo> OtherClassSelection = new List<PhotoInfo>();
    private readonly Dictionary<PhotoClass, List<PhotoInfo>> Selection = new Dictionary<PhotoClass, List<PhotoInfo>>();

    public GameOutput(GameInput input) {
        Selection.Add(input.MainClass, MainClassSelection);
        Selection.Add(input.OtherClass, OtherClassSelection);
    }

    public void Assign(PhotoClass photoClass, PhotoInfo photoInfo) {
        Selection[photoClass].Add(photoInfo);
    }
}

public static class Connection {
    public static bool IsLogged = false;
    public static string DeviceId = null;
    public static LoginData loginData = null;

    public static readonly string Server = "https://kask.eti.pg.gda.pl/cenhive";
    public static readonly string LoginURL = Server + "/api.php?o=login";
    public static readonly string GetManyDataURL = Server + "/api.php?o=getmanydata";
    public static readonly string SendManyAnswersURL = Server + "/api.php?o=sendmanyanswers";
    public static readonly string RegisterURL = Server + "/api.php?o=register";
    public static readonly string Version = "3.0";
    public static readonly string Collection = "faces";
    public static readonly int NumberOfPictures = 25;


    [Serializable]
    public class LoginData {
        [NonSerialized]
        public Texture2D texture;
        public int id;
        public string login;
        public string avatar;
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

        public Dictionary<int, string> getMainClassPhotoUrls(PhotoClass photoClass) {
            var dict = new Dictionary<int, string>();
            phrases.Where(p => correct_phrases.Contains(p.id)).ToList().ForEach(p => {
                var link = p.media[0].data.Replace("\\", string.Empty);
                var id = p.id;
                dict.Add(id, link);
            });
           
            return dict;
        }

        public Dictionary<int, string> getOtherClassPhotoUrls() {
            var dict = new Dictionary<int, string>();
            phrases.Where(p => !correct_phrases.Contains(p.id)).ToList().ForEach(p => {
                var link = p.media[0].data.Replace("\\", string.Empty);
                var id = p.id;
                dict.Add(id, link);
            });

            return dict;
        }

    }

    private static IEnumerator GetDataFromServer(Action<JsonResponse> callback) {
        WWWForm form = new WWWForm();
        form.AddField("collection", Collection);
        form.AddField("device_id", DeviceId);
        form.AddField("phrases_count", NumberOfPictures);

        using (UnityWebRequest www = UnityWebRequest.Post(GetManyDataURL, form)) {
            yield return www.SendWebRequest();
            if (www.isNetworkError) {
                Debug.Log(www.error);
            } else {
                Debug.Log(www.downloadHandler.text);
                var response = JsonUtility.FromJson<JsonResponse>(www.downloadHandler.text);
                callback(response); 
            }
        }
    }

    private static IEnumerator SendCorrectPictures(IEnumerable<int> selected, IEnumerable<int> notSelected) {
        WWWForm form = new WWWForm();
        form.AddField("collection", Collection);
        form.AddField("device_id", /*DeviceId*/ 1 /*throws error without 1*/);
        form.AddField("answer_time", 15523);
        foreach (var number in selected) {
            form.AddField("selected_phrases[]", number);
        }

        foreach (var number in notSelected) {
            form.AddField("not_selected_phrases[]", number);
        }

        Debug.Log(form.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(SendManyAnswersURL, form)) {
            yield return www.SendWebRequest();
            if (www.isNetworkError) {
                Debug.Log(www.error);
            } else {
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

    public static IEnumerator GetPhoto(System.Action<Texture2D> callback, string url) {
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

    public static IEnumerator GetEnumeratorDataInput(System.Action<GameInput> callback) {
        JsonResponse response = null;
        yield return GetDataFromServer(r => response = r);

        var faces = new PhotoClass("Faces");
        var facePhotos = new List<PhotoInfo>();
        foreach (var pair in response.getMainClassPhotoUrls(faces)) {
            yield return GetPhoto(texture => facePhotos.Add(new PhotoInfo(texture, faces, pair.Key)), pair.Value);
        }

        var other = new PhotoClass("Other");
        var otherPhotos = new List<PhotoInfo>();
        foreach (var pair in response.getOtherClassPhotoUrls()) {
            yield return GetPhoto(texture => otherPhotos.Add(new PhotoInfo(texture, other, pair.Key)), pair.Value);
        }

        var photos = facePhotos.Concat(otherPhotos).ToList();

        var input = new GameInput {
            Photos = photos,
            MainClass = faces,
            OtherClass = other
        };

        callback(input);
    }

    public static IEnumerator HandleOutput(GameOutput output, GameInput input) {

        if (output.Points > 100) {
            Debug.Log("Archivement \"Get 100 Points.\" Unlocked!");
        }

        var class1 = input.MainClass;
        var class2 = input.OtherClass;
        var mainSelected = output.MainClassSelection.Distinct();
        var otherSelected = output.OtherClassSelection.Distinct();

        var main = mainSelected.Except(otherSelected).Select(p => p.ID);
        var others = otherSelected.Except(mainSelected).Select(p => p.ID);

        yield return SendCorrectPictures(main, others);

        Debug.Log(string.Format("Class {0} classified {1} photos", class1.ClassName, main.Count()));
        Debug.Log(string.Format("Class {0} classified {1} photos", class2.ClassName, others.Count()));
    }

    public static IEnumerator LogIn(string login, string password) {
        WWWForm form = new WWWForm();
        form.AddField("login", login);
        form.AddField("password", password);
        form.AddField("device_id", DeviceId);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginURL, form)) {
            yield return www.SendWebRequest();
            if (!www.isHttpError && !www.isNetworkError) {
                Debug.Log(www.downloadHandler.text);
                loginData = JsonUtility.FromJson<LoginData>(www.downloadHandler.text);
            } else {
                Debug.Log(www.error);
            }
        }
    }

    public static IEnumerator RegisterDevice(string username) {
        WWWForm form = new WWWForm();
        form.AddField("name", username);
        form.AddField("version", Version);
        using (UnityWebRequest www = UnityWebRequest.Post(RegisterURL, form)) {
            yield return www.SendWebRequest();
            if (!www.isHttpError && !www.isNetworkError) {
                Debug.Log(www.downloadHandler.text);
                DeviceId = www.downloadHandler.text;
            } else {
                Debug.Log(www.error);
            }
        }
    }
}


