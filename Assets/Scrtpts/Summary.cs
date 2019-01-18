using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Summary : MonoBehaviour {
    private static GameOutput output;
    private static GameInput input;

    public static void GoToScene(GameOutput output, GameInput input) {
        Summary.output = output;
        Summary.input = input;
        SceneManager.LoadScene("Summary");
    }

    public TMP_Text text;
    public Button exitButton;

    private void Start() {
        text.text = output.Points.ToString();
        StartCoroutine(HandleGameResults(output, input));
        output = null;
        input = null;
    }

    private IEnumerator HandleGameResults(GameOutput output, GameInput input) {
        Config.Connection.HandleOutput(output, input);
        exitButton.interactable = true;
        yield return null;
    }

    public void ExitButtonOnClick() {
        Menu.GoToScene();
    }
}
