using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfflineConnection : IConnection {

    private class OfflinePhotoInfo : IPhotoInfo {
        public Texture2D Texture {
            get; set;
        }
        public string ID {
            get; set;
        }
    }

    private class OfflinePhotoClass : IPhotoClass {
        public string ReadableName {
            get; set;
        }

        public bool DoesBelong(IPhotoInfo info) {
            var offlineInfo = info as OfflinePhotoInfo;
            return Equals(offlineInfo.ID, ReadableName);
        }
    }

    private class OfflineGameInput : IGameInput {
        public IPhotoClass Class1 { get; set; }
        public IPhotoClass Class2 { get; set; }
        public List<IPhotoInfo> Photos { get; set; }
    }

    public IGameInput GetGameInput() {
        var class1Name = "Cats";
        var class1 = new OfflinePhotoClass {
            ReadableName = class1Name
        };

        var class2Name = "Dogs";
        var class2 = new OfflinePhotoClass {
            ReadableName = class2Name
        };

        var photos = new List<IPhotoInfo>();
        foreach (var asset in new List<string>() { "c1", "c2", "c3" }) {
            var photo = new OfflinePhotoInfo {
                Texture = Resources.Load<Texture2D>(asset),
                ID = class1Name
            };
            photos.Add(photo);
        }

        foreach (var asset in new List<string>() { "d1", "d2", "d3" }) {
            var photo = new OfflinePhotoInfo {
                Texture = Resources.Load<Texture2D>(asset),
                ID = class2Name
            };
            photos.Add(photo);
        }

        return new OfflineGameInput {
            Photos = photos,
            Class1 = class1,
            Class2 = class2
        };
    }

    public void HandleOutput(GameOutput output, IGameInput input) {

        if (output.Points > 100) {
            Debug.Log("Archivement \"Get 100 Points.\" Unlocked!");
        }

        var class1 = input.Class1;
        var class2 = input.Class2;
        var selected1 = output.Class1Selection.Distinct();
        var selected2 = output.Class2Selection.Distinct();

        var toSend1 = selected1.Except(selected2);
        var toSend2 = selected2.Except(selected1);

        Debug.Log(string.Format("Class {0} classified {1} photos", class1.ReadableName, toSend1.Count()));
        Debug.Log(string.Format("Class {0} classified {1} photos", class2.ReadableName, toSend2.Count()));
        return;
    }

    public bool LogIn(string username, string password) {
        Debug.Log(string.Format("Login {0} Password {1}", username, password));
        return true;
    }
}
