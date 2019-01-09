using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public Text text;

	// Use this for initialization
	void Start () {
        text.text = "Game Over: " + Level.points.ToString();
        StartCoroutine(test());
    }
	
	// Update is called once per frame
	void Update () {
           
	}

    IEnumerator test()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main Menu");
    }
}
