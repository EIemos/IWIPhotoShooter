using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Folder : MonoBehaviour
{
    public Text text;
    public ClassData classData;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("File"))
        {
            if(Level.inFolder(col.gameObject.GetComponent<PhotoFile>().photoData, classData))
            {
                Level.points += 15;
            }
            else
            {
                Level.points -= 20;
            }
            Destroy(col.gameObject);
        }
    }

    public void set(ClassData data, Vector2 position)
    {
        classData = data;
        text.text = classData.id;
        transform.position = position;
    }

}
