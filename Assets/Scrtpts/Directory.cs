using UnityEngine;
using UnityEngine.UI;


public class Directory : MonoBehaviour {
    public Text Text;
    private IPhotoClass PhotoClass;
    private GameOutput GameOutput;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("File")) { return; }
        
        var photo = collision.gameObject.GetComponent<Photo>();
        GameOutput.Points += PhotoClass.DoesBelong(photo.PhotoInfo) ? 15 : -20;
        GameOutput.Assign(PhotoClass, photo.PhotoInfo);
        Destroy(photo.gameObject);
    }

    public static Directory Spawn(IPhotoClass PhotoClass, GameOutput GameOutput, Vector2 Position, Transform Parent) {
        var directory = Instantiate(Resources.Load<GameObject>("Folder"), Parent).GetComponent<Directory>();
        directory.PhotoClass = PhotoClass;
        directory.GameOutput = GameOutput;
        directory.transform.position = Position;
        directory.Text.text = PhotoClass.ReadableName;
        directory.gameObject.transform.SetParent(Parent);
        return directory;
    }

}
