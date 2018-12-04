using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public float spawnDelay;
    public GameObject photo;
    public PhotosFetcher photosFether;
    private float nextSpawnTime;
    private bool isGameOver;

	// Use this for initialization
	void Start () {
        spawnDelay = 3.0f;
        nextSpawnTime = 0.0f;
        isGameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isGameOver)
        {
            if(Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + spawnDelay;
                SpawnPhoto();
            }

            if(Input.GetMouseButtonDown(0))
            {
                Vector2 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickedPos, clickedPos, 0.0f);
                if(hit)Debug.Log("click!");
                if( hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Photo") )
                {
                    hit.collider.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
	}

    void SpawnPhoto()
    {
        GameObject newPhoto = Instantiate(photo, new Vector3( Random.Range(-9.9f, 9.9f), 5.4f, 0.0f), Quaternion.identity) as GameObject;
        PhotoDescriptor photos = photosFether.GetPhoto();
        newPhoto.SendMessage( "SetPhotoDescriptor", photos, SendMessageOptions.DontRequireReceiver);
    }
}
