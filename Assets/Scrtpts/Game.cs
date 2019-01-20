using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    private static GameInput input = null;

    public static void GoToScene(GameInput input) {
        Game.input = input;
        SceneManager.LoadScene("Game");
    }

    public Trash trash;
    public Transform class1Pos, class2Pos;
    public List<Transform> spawnPoints;
    public Text score, time;
    public bool canSpawn = true;
    public bool quit = false;

    private PhotoInfo nextPhoto() {
        return input.Photos[Random.Range(0, input.Photos.Count)];
    }

    private Vector2 nextSpawnPoint() {
        var found = spawnPoints.FindAll(point => {
            return Physics2D.OverlapCircleAll(point.position, 50).Count() == 0;
        });

        if (found.Count == 0) {
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }

        return found[Random.Range(0, found.Count)].position;
    }

    private IEnumerator SpawnPhoto(Transform parent, float time) {
        canSpawn = false;
        var position = nextSpawnPoint();

        var countdown = time;

        var placeholder = Placeholder.Spawn(position, parent);

        while (countdown != 0) {
            placeholder.text.text = countdown.ToString();
            countdown--;
            yield return new WaitForSeconds(1);
        }

        Destroy(placeholder.gameObject);

        if (Physics2D.OverlapCircleAll(position, 50).Count() != 0) {
            quit = true;
            yield break;
        }

        var photoInfo = nextPhoto();
        Photo.Spawn(photoInfo, position, parent);
        canSpawn = true;
        yield return 0;
    }

    private GameOutput output;

    private void Start() {
        if (input == null) { Loading.GoToScene(); }

        output = new GameOutput(input);
        trash.GameOutput = output;
        Directory.Spawn(input.Class1, output, class1Pos.position, gameObject.transform);
        Directory.Spawn(input.Class2, output, class2Pos.position, gameObject.transform);
    }

    private void Update() {
        score.text = output.Points.ToString();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Main Menu");
        }

        if (quit) {
            Summary.GoToScene(output, input);
        }

        if (canSpawn) {
            var time = output.Points > 100 ? 1 : 2;
            StartCoroutine(SpawnPhoto(gameObject.transform, time));
        }
    }

}
