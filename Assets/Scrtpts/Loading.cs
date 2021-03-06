﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {
    public static void GoToScene() {
        SceneManager.LoadScene("Loading");
    }

    private void Start() {
       
    }

    private void Awake() {
        StartCoroutine(LoadAsssets());
    }

    private IEnumerator LoadAsssets() {
        GameInput input = null;
        yield return Connection.GetEnumeratorDataInput(i => input = i);
        if(input == null) {
            Debug.Log("Loading Error");
            Menu.GoToScene();
            yield break;
        }

        Game.GoToScene(input);
    }

}
