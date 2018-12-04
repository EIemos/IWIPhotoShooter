using System.IO;
using UnityEngine;

public enum PhotoClasifications
{
    PHOTO_GOOD,
    PHOTO_BAD
}

public struct PhotoDescriptor
{
    public Sprite sprite;
    public int ID;
    public PhotoClasifications classification;

    public PhotoDescriptor(Sprite spr, int id, PhotoClasifications photoClass )
    {
        sprite = spr;
        ID = id;
        classification = photoClass;
    }
}

public class PhotosOfflineGenerator
{
    private Texture2D badPhoto, goodPhoto;
    private int width = 300, height = 300;
    private Rect rect;
    private Vector2 pivot;
    private float badChance = 0.2f;
    
    public void LoadTextures()
    {
        badPhoto  = new Texture2D(width, height);
        goodPhoto = new Texture2D(width, height);
        badPhoto.LoadImage(  File.ReadAllBytes( Application.dataPath + "/Placeholders/negative.png" ) );
        goodPhoto.LoadImage( File.ReadAllBytes( Application.dataPath + "/Placeholders/positive.png" ) );
        rect = new Rect(0, 0, width, height);
        pivot = new Vector2(0.5f, 0.5f);
    }

    public PhotoDescriptor GeneratePhoto()
    {
        Texture2D texture;
        PhotoClasifications clasifications;
        if( Random.Range( 0.0f, 1.0f ) < badChance )
        {
            texture = badPhoto;
            clasifications = PhotoClasifications.PHOTO_BAD;
        }
        else
        {
            texture = goodPhoto;
            clasifications = PhotoClasifications.PHOTO_GOOD;
        }

        int id = Random.Range(0, int.MaxValue);
        Sprite sprite = Sprite.Create( texture, rect, pivot );
        return new PhotoDescriptor( sprite, id, clasifications );
    }
}