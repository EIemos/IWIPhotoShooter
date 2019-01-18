using UnityEngine;
using UnityEngine.UI;

public class Photo : MonoBehaviour {
    private static int id = 1;
    private Vector3 offset;
    private bool selected;
    public float velLimit = 500, angularLimit = 180;
    public new Rigidbody2D rigidbody;
    public Image image;
    public Text Text;
    public Image seleciton;
    public PhotoInfo PhotoInfo;

    private void Update() {
        if (!selected) { return; }

        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = -transform.position + cursorPos;
        rigidbody.AddForceAtPosition(30 * direction, transform.position - offset);

        if (rigidbody.velocity.magnitude > velLimit) {
            rigidbody.velocity = rigidbody.velocity.normalized * velLimit;
        }


        if (rigidbody.angularVelocity > angularLimit) {
            rigidbody.angularVelocity = angularLimit;
        }

        if (rigidbody.angularVelocity < -angularLimit) {
            rigidbody.angularVelocity = -angularLimit;
        }

        if (Input.GetMouseButtonUp(0)) {
            selected = false;
        }
    }

    private void OnMouseOver() {
        if (!Input.GetMouseButtonDown(0)) { return; }
        selected = true;
        seleciton.enabled = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Photo Spawn(PhotoInfo PhotoInfo, Vector2 Position, Transform Parent) {
        var photo = Instantiate(Resources.Load<GameObject>("File"), Parent).GetComponent<Photo>();
        photo.PhotoInfo = PhotoInfo;
        photo.transform.position = Position;
        photo.Text.text = id++.ToString() + ".jpg";
        photo.image.sprite = Sprite.Create(PhotoInfo.Texture, new Rect(0, 0, PhotoInfo.Texture.width, PhotoInfo.Texture.height), new Vector2(0, 0));
        photo.gameObject.transform.SetParent(Parent);
        return photo;
    }
}
