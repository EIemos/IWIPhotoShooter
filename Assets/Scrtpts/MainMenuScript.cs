using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {
    
    // Use this for initialization
    void Start()
    {
        GameObject.Find( "Login_failed_text" ).GetComponent<TMP_Text>().enabled = false;
    }

    public void LogInButtonOnClick()
    {
        StartCoroutine(LogIn());
    }

    IEnumerator LogIn()
    {
        string login = GetComponentInChildren<TMP_InputField >().text;
        string password = GetComponentInChildren<TMP_InputField >().text;

        GetComponentInChildren<Button>().interactable = false;
        bool credentialsValid = verifyCredentials( login, password );
        yield return new WaitForSeconds( 2 );

        GetComponentInChildren<Button>().interactable = true;
        //if( credentialsValid )
            SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1 );
        //else
        //    GameObject.Find( "Login_failed_text" ).GetComponent<TMP_Text>().enabled = true;
    }

    bool verifyCredentials(string login, string password)
    {
        //Call remote API
        if( login == "pass" )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
