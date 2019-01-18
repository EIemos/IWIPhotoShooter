using UnityEngine;
using UnityEngine.UI;


public class Directory : MonoBehaviour {
    public Text Text;
    private PhotoClass PhotoClass;
    private GameOutput GameOutput;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("File")) { return; }
        
        var photo = collision.gameObject.GetComponent<Photo>();
        GameOutput.Points += PhotoClass.ClassName == photo.PhotoInfo.ClassName ? 15 : -20;
        GameOutput.Assign(PhotoClass, photo.PhotoInfo);
        Destroy(photo.gameObject);
    }

    public static Directory Spawn(PhotoClass PhotoClass, GameOutput GameOutput, Vector2 Position, Transform Parent) {
        var directory = Instantiate(Resources.Load<GameObject>("Folder"), Parent).GetComponent<Directory>();
        directory.PhotoClass = PhotoClass;
        directory.GameOutput = GameOutput;
        directory.transform.position = Position;
        directory.Text.text = PhotoClass.ClassName;
        directory.gameObject.transform.SetParent(Parent);
        return directory;
    }

}
