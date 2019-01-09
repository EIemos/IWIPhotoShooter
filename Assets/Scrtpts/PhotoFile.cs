using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoFile : MonoBehaviour {
    private Vector3 offset;
    private bool selected;
    public float velLimit = 500, angularLimit = 180;
    public Rigidbody2D rigidbody;
    public Image image;
    public Text text;
    public Image seleciton;
    public PhotoData photoData;
    // Update is called once per frame

    void Update () {

        if (selected)
        {
       
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = -transform.position + cursorPos;
            rigidbody.AddForceAtPosition(30* direction, transform.position - offset);

            if(rigidbody.velocity.magnitude > velLimit)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * velLimit;
            }

            
            if (rigidbody.angularVelocity > angularLimit)
            {
                rigidbody.angularVelocity = angularLimit;
            }

            if (rigidbody.angularVelocity < -angularLimit)
            {
                rigidbody.angularVelocity = -angularLimit;
            }

            if (Input.GetMouseButtonUp(0))
            {
                selected = false;
            }
        }
    }

    static int id = 1;

    public void set(PhotoData p, Vector2 position)
    {
        photoData = p;
        text.text = id++.ToString();
        image.sprite = Sprite.Create(p.texture, new Rect(0, 0, p.texture.width, p.texture.height), new Vector2(0, 0));
        transform.position = position;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected = true;
            seleciton.enabled = true;
            offset = -Camera.main.ScreenToWorldPoint(Input.mousePosition)+ transform.position;
        }
            
    }
}
