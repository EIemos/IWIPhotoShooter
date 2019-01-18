using UnityEngine;

public class Trash : MonoBehaviour {
    public GameOutput GameOutput;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("File")) {
            GameOutput.Points -= 10;
            Destroy(collision.gameObject);
        }
    }
}
