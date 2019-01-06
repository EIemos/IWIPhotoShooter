using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoFile : MonoBehaviour {

    private bool selected;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(selected)
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cursorPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            selected = false;
        }
	}

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected = true;
        }
    }
}
