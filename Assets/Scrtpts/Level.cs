using System.Collections.Generic;
using UnityEngine;

public class PhotoData {
    public string id;
    public Texture2D texture;
}

public class ClassData {
    public string id;
}

public static class Level {
    public static int points = 0;
    private static List<PhotoData> photos = new List<PhotoData>();

    static Level() {
    }

    public static List<ClassData> getClasses() {
        var dogs = new ClassData {
            id = "dog"
        };

        var cats = new ClassData {
            id = "cat"
        };

        return new List<ClassData>() { dogs, cats };
    }

    public static List<PhotoData> getPhotos() {
        var list = new List<PhotoData>(photos);
        //photos.Clear();
        return list;
    }


    public static bool inFolder(PhotoData photoData, ClassData classData) {
        Debug.Log("Got to folder");
        return string.Equals(photoData.id, classData.id);
    }

    public static void inTrash(PhotoData photoData) {
        Debug.Log("Photo discarded");
    }


}
