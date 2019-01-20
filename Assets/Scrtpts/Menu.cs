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
    public GameObject loginMenu, playerMenu;
    public TMP_Text playerName;
    public Image avatar;

    public void LogInButtonOnClick() {
        StartCoroutine(LogIn());
    }

    public void PlayButtonOnClick() {
        Loading.GoToScene();
    }

    public void LogoutButtonOnClick() {
        Connection.loginData = null;
        setLoginMenu();
    }

    public void QuitButtonOnClick() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void setPlayerMenu() {
        loginMenu.SetActive(false);
        playerMenu.SetActive(true);
        playerName.text = Connection.loginData.login;
        if (Connection.loginData.texture != null) {
            var t = Connection.loginData.texture;
            avatar.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        } else {
            StartCoroutine(Connection.GetPhoto(t => {
                Connection.loginData.texture = t;
                avatar.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            },
                Connection.Server + "/" + Connection.loginData.avatar.Replace("\\", string.Empty)));
        }


    }

    public void setLoginMenu() {
        loginInput.text = "";
        passwordInput.text = "";
        loginMenu.SetActive(true);
        playerMenu.SetActive(false);
    }

    private IEnumerator LogIn() {
        string login = loginInput.text;
        string password = passwordInput.text;

        var interactable = new List<Selectable>() { passwordInput, loginInput, loginButton };

        interactable.ForEach(s => s.interactable = false);

        if (Connection.DeviceId == null) {
            yield return Connection.RegisterDevice(login);
        }

        yield return Connection.LogIn(login, password);
        interactable.ForEach(s => s.interactable = true);

        if (Connection.loginData == null) {
            loginFailedText.gameObject.SetActive(true);
            yield return null;
        } else {
            loginFailedText.gameObject.SetActive(false);
            setPlayerMenu();
        }
    }

    private void Start() {
        if (Connection.loginData != null) {
            setPlayerMenu();
        } else {
            setLoginMenu();
        }
    }

}
