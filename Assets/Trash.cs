using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("File"))
        {
            Level.points -= 10;
            Destroy(col.gameObject);
        }
    }
}
