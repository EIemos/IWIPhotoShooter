using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Game : MonoBehaviour {

    public Transform class1Pos, class2Pos;
    public List<Transform> spawnPoints;
    public List<PhotoData> toSpawn = Level.getPhotos();
    public float lastSpawn;
    public float spawnTime;
    public Text score, time;

    // Use this for initialization
    void Start () {
        lastSpawn = Time.time - 10;

        var classes = Level.getClasses();

        spawn(classes[0], class1Pos.position);
        spawn(classes[1], class2Pos.position);
 

        foreach (var asset in new List<string>() { "c1", "c2", "c3" })
        {
            var p = new PhotoData();
            p.id = "cat";
            p.texture = Resources.Load<Texture2D>(asset);
            toSpawn.Add(p);
        }

        foreach (var asset in new List<string>() { "d1", "d2", "d3" })
        {
            var p = new PhotoData();
            p.id = "dog";
            p.texture = Resources.Load<Texture2D>(asset);
            toSpawn.Add(p);
        }

    }
	
	// Update is called once per frame
	void Update () {
        score.text = Level.points.ToString();
        time.text = ( spawnTime - Time.time + lastSpawn).ToString();
        trySpawn();

        if(Level.points > 100)
        {
            spawnTime = 1;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Main Menu");

    }

    public void trySpawn()
    {
        if (Time.time - lastSpawn > spawnTime)
        {
            lastSpawn = Time.time;
            if (toSpawn.Count == 0)
                toSpawn.AddRange(Level.getPhotos());
            if (toSpawn.Count != 0)
            {
                var data = toSpawn[0];
                toSpawn.RemoveAt(0);
                toSpawn.Add(data);

                if (!spawn(data))
                {
                    SceneManager.LoadScene("GameOver");
                }
            }

        }
    }

    public Transform getSpawnPoint()
    {
        var objects = GameObject.FindGameObjectsWithTag("File");
        var found = spawnPoints.Find(point => {
                return Physics2D.OverlapCircleAll(point.position, 50).Count() == 0;
            });

        return found;
    }

    bool IsPointInRT(Vector2 point, RectTransform rt)
    {
        // Get the rectangular bounding box of your UI element
        Rect rect = rt.rect;

        // Get the left, right, top, and bottom boundaries of the rect
        float leftSide = rt.anchoredPosition.x - rect.width / 2 - 1;
        float rightSide = rt.anchoredPosition.x + rect.width / 2 +1;
        float topSide = rt.anchoredPosition.y + rect.height / 2 + 1;
        float bottomSide = rt.anchoredPosition.y - rect.height / 2 - 1;

        //Debug.Log(leftSide + ", " + rightSide + ", " + topSide + ", " + bottomSide);

        // Check to see if the point is in the calculated bounds
        if (point.x >= leftSide &&
            point.x <= rightSide &&
            point.y >= bottomSide &&
            point.y <= topSide)
        {
            return true;
        }
        return false;
    }

    public bool spawn(PhotoData data)
    {
        var point = getSpawnPoint();
        if(point == null) {
            return false;
        }

        var e = Instantiate(Resources.Load<GameObject>("File"), gameObject.transform).GetComponent<PhotoFile>();
        e.set(data, point.position);
        e.gameObject.transform.SetParent(this.transform);

        return true;
    }

    public bool spawn(ClassData data, Vector2 pos)
    {
        var e = Instantiate(Resources.Load<GameObject>("Folder"), gameObject.transform).GetComponent<Folder>();
        e.set(data, pos);
        e.gameObject.transform.SetParent(this.transform);

        return true;
    }
}
