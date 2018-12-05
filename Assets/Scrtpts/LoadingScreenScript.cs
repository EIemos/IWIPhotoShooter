using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine( MimicDownloadingAssets() );
	}
	
    IEnumerator MimicDownloadingAssets()
    {
        yield return new WaitForSeconds( 5 );
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
