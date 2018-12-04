using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotosFetcher : MonoBehaviour {

    private int cacheMaxSize;
    private int photosPackSize;
    private bool generatePicsOffline;
    private bool isFetching;
    private Queue<PhotoDescriptor> cache;
    private PhotosOfflineGenerator generator;

	// Use this for initialization
	void Start () {
        cacheMaxSize = 15;
        photosPackSize = 1;
        generatePicsOffline = true;
        isFetching = false;
        cache = new Queue<PhotoDescriptor>();
        generator = new PhotosOfflineGenerator();

        if( generatePicsOffline )
            generator.LoadTextures();
        //else make a connectioin?
	}
	
	// Update is called once per frame
	void Update () {
		if(cache.Count + photosPackSize <= cacheMaxSize)
        {
            if( !isFetching )
            {
                Fetch();
            }
        }
	}

    public PhotoDescriptor GetPhoto()
    {
        return cache.Dequeue();
    }

    private void Fetch()
    {
        isFetching = true;
        //make this async
        if(generatePicsOffline)
            cache.Enqueue( generator.GeneratePhoto() );

        //TODO set to false only after async is done
        isFetching = false;
    }
}