using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotoInfo {
    public Texture2D Texture { get; set; }
    public string ClassName { get; set; }
}

public class PhotoClass {
    public string ClassName { get; set; }
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
    public static readonly IConnection Connection = new OfflineConnection();
}

public class OfflineConnection : IConnection {

    public GameInput GetGameInput() {
        var class1 = new PhotoClass { ClassName = "Cats" };
        var photos1 = new[] { "c1", "c2", "c3" }.Select(name => new PhotoInfo { Texture = Resources.Load<Texture2D>(name), ClassName = class1.ClassName });

        var class2 = new PhotoClass { ClassName = "Dogs" };
        var photos2 = new[] { "d1", "d2", "d3" }.Select(name => new PhotoInfo { Texture = Resources.Load<Texture2D>(name), ClassName = class2.ClassName });

        return new GameInput {
            Photos = photos1.Concat(photos2).ToList(),
            Class1 = class1,
            Class2 = class2
        };
    }

    public void HandleOutput(GameOutput output, GameInput input) {

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
        return;
    }

    public bool LogIn(string username, string password) {
        Debug.Log(string.Format("Login {0} Password {1}", username, password));
        return true;
    }
}


public class TestOnlineConnection : IConnection {

    public GameInput GetGameInput() {
        var class1 = new PhotoClass { ClassName = "Cats" };
        var photos1 = new[] { "c1", "c2", "c3" }.Select(name => new PhotoInfo { Texture = Resources.Load<Texture2D>(name), ClassName = class1.ClassName });

        var class2 = new PhotoClass { ClassName = "Dogs" };
        var photos2 = new[] { "d1", "d2", "d3" }.Select(name => new PhotoInfo { Texture = Resources.Load<Texture2D>(name), ClassName = class2.ClassName });

        return new GameInput {
            Photos = photos1.Concat(photos2).ToList(),
            Class1 = class1,
            Class2 = class2
        };
    }

    public void HandleOutput(GameOutput output, GameInput input) {

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
        return;
    }

    public bool LogIn(string username, string password) {
        Debug.Log(string.Format("Login {0} Password {1}", username, password));
        return true;
    }
}


