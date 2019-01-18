using System.Collections.Generic;
using UnityEngine;

public interface IPhotoInfo {
    Texture2D Texture { get; }
}

public interface IPhotoClass {
    string ReadableName { get; }
    bool DoesBelong(IPhotoInfo info);
}

public interface IGameInput {
    IPhotoClass Class1 { get; }
    IPhotoClass Class2 { get; }
    List<IPhotoInfo> Photos { get; }
}

public class GameOutput {
    public int Points { get; set; }
    public List<IPhotoInfo> Class1Selection = new List<IPhotoInfo>();
    public List<IPhotoInfo> Class2Selection = new List<IPhotoInfo>();
    private readonly Dictionary<IPhotoClass, List<IPhotoInfo>> Selection = new Dictionary<IPhotoClass, List<IPhotoInfo>>();

    public GameOutput(IGameInput input) {
        Selection.Add(input.Class1, Class1Selection);
        Selection.Add(input.Class2, Class2Selection);
    }

    public void Assign(IPhotoClass photoClass, IPhotoInfo photoInfo) {
        Selection[photoClass].Add(photoInfo);
    }
}

public interface IConnection {
    bool LogIn(string username, string password);
    IGameInput GetGameInput();
    void HandleOutput(GameOutput output, IGameInput input);
}

public static class Config {
    public static readonly IConnection Connection = new OfflineConnection();
}
