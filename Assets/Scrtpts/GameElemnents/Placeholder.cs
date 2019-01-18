using UnityEngine;
using UnityEngine.UI;

public class Placeholder : MonoBehaviour {

    public Text text;
    public Image image;

    public static Placeholder Spawn(Vector2 Position, Transform Parent) {
        var placeholder = Instantiate(Resources.Load<GameObject>("Placeholder"), Parent).GetComponent<Placeholder>();
        placeholder.transform.position = Position;
        placeholder.gameObject.transform.SetParent(Parent);
        return placeholder;
    }
}
