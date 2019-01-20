using UnityEngine;
using UnityEngine.UI;


public class Directory : MonoBehaviour {
    public Text Text;
    private PhotoClass PhotoClass;
    private GameOutput GameOutput;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("File")) { return; }
        
        var photo = collision.gameObject.GetComponent<Photo>();
        GameOutput.Points += PhotoClass == photo.PhotoInfo.PhotoClass ? 15 : -20;
        if (photo.seleciton.enabled) {
            GameOutput.Assign(PhotoClass, photo.PhotoInfo);
        }
        Destroy(photo.gameObject);
    }

    public static Directory Spawn(Directory directory, PhotoClass PhotoClass, GameOutput GameOutput, Vector2 Position, Transform Parent) {
        directory.PhotoClass = PhotoClass;
        directory.GameOutput = GameOutput;
        directory.Text.text = PhotoClass.ClassName;
        return directory;
    }

}
