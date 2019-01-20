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

    public Animator animator;
    public Animator trashAnimator;
    public bool animated = false;
    public Trash trash;
    public Transform class1Pos, class2Pos;
    public List<Transform> spawnPoints;
    public List<Transform> spawnPoints1;
    public List<Transform> currentSpawnPoints;
    public Directory dir1;
    public Directory dir2;
    public Text score, playerName;
    public bool canSpawn = true;
    public bool quit = false;
    public float spawnTime = 5;
    public float spawnBreak = 5;

    private PhotoInfo nextPhoto() {
        return input.Photos[Random.Range(0, input.Photos.Count)];
    }

    private Vector2 nextSpawnPoint() {
        var found = currentSpawnPoints.FindAll(point => {
            return Physics2D.OverlapCircleAll(point.position, 50).Count() == 0;
        });

        if (found.Count == 0) {
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }

        return found[Random.Range(0, found.Count)].position;
    }

    private IEnumerator SpawnPhoto(Transform parent, float time) {
        var position = nextSpawnPoint();
        var countdown = time;
        var placeholder = Placeholder.Spawn(position, parent);

        while (countdown != 0) {
            placeholder.text.text = countdown.ToString();
            countdown-= 0.25f;
            yield return new WaitForSeconds(0.25f);
        }

        Destroy(placeholder.gameObject);

        if (Physics2D.OverlapCircleAll(position, 50).Count() != 0) {
            quit = true;
            yield break;
        }

        var photoInfo = nextPhoto();
        Photo.Spawn(photoInfo, position, parent);
        yield return 0;
    }

    private IEnumerator SpawnTimer(float time) {
        canSpawn = false;
        yield return new WaitForSeconds(time);
        canSpawn = true;
    }

    private GameOutput output;

    private void Start() {
        if (input == null) { Loading.GoToScene(); }
        playerName.text = Connection.loginData.login;
        output = new GameOutput(input);
        trash.GameOutput = output;

        Directory.Spawn(dir1,input.MainClass, output, class1Pos.position, gameObject.transform);
        Directory.Spawn(dir2,input.OtherClass, output, class2Pos.position, gameObject.transform);
    }

    public void ReturnButtonOnClick() {
        Menu.GoToScene();
    }

    private void Update() {
        var points = output.Points;

        score.text = points.ToString();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Main Menu");
        }

        if (quit) {
            Summary.GoToScene(output, input);
        }

        if (points > 700) {
            spawnTime = 0.5f;
            spawnBreak = 0.75f;
            currentSpawnPoints = spawnPoints.Concat(spawnPoints1).ToList();
        }
        else if (points > 600) {
            spawnTime = 0.5f;
            spawnBreak = 1f;
            currentSpawnPoints = spawnPoints.Concat(spawnPoints1).ToList();
        }
        else if (points > 500) {
            spawnTime = 1f;
            spawnBreak = 1.25f;
            currentSpawnPoints = spawnPoints.Concat(spawnPoints1).ToList();
            
        } else if (points > 400) {
            spawnTime = 1.5f;
            spawnBreak = 1.75f;
            currentSpawnPoints = spawnPoints.Concat(spawnPoints1).ToList();
            trashAnimator.Play("trash");

        } else if(points > 300) {
            spawnTime = 1;
            spawnBreak = 1.5f;
            currentSpawnPoints = spawnPoints.Concat(spawnPoints1).ToList();
        }
        else if(points > 200) {
            spawnTime = 1;
            spawnBreak = 1.5f;
            currentSpawnPoints = spawnPoints;
        }
        else if(points > 100) {
            spawnTime = 1;
            spawnBreak = 1.5f;
            currentSpawnPoints = spawnPoints;
            if (!animated) {
                animator.Play("folders");
                animated = true;
            }

        } else if(points > 50) {
            spawnTime = 2;
            spawnBreak = 2.25f;
            currentSpawnPoints = spawnPoints;
        }
        else {
            currentSpawnPoints = spawnPoints;
            spawnTime = 2;
            spawnBreak = 3;
        }


        if (canSpawn) {
            StartCoroutine(SpawnTimer(spawnBreak));
            StartCoroutine(SpawnPhoto(gameObject.transform, spawnTime));
        }
    }

}
