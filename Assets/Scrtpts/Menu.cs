using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public static void GoToScene() {
        SceneManager.LoadScene("Menu");
    }

    public Button loginButton;
    public TMP_InputField loginInput;
    public TMP_InputField passwordInput;
    public TMP_Text loginFailedText;

    public void LogInButtonOnClick() {
        StartCoroutine(LogIn());
    }

    private IEnumerator LogIn() {
        string login = loginInput.text;
        string password = passwordInput.text;

        var interactable = new List<Selectable>() { passwordInput, loginInput, loginButton };

        interactable.ForEach(s => s.interactable = false);
        var logged = Config.Connection.LogIn(loginInput.text, passwordInput.text);
        interactable.ForEach(s => s.interactable = true);

        if (!logged) {
            loginFailedText.enabled = true;
            yield break;
        }

        Loading.GoToScene();
        yield return null;
    }
}
