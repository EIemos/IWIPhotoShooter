using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photo : MonoBehaviour {

    public float fadeRate;
    public float xSpeed;
    public float ySpeed;
    public PhotoDescriptor photoDescriptor;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D m_collider;
    private float xSpeedRandomRange;
    private float ySpeedRandomRange;
    private bool isAlive = true;

    // Use this for initialization
    void Start ()
    {
        fadeRate = 1.2f;
        xSpeed = 0.1f;
        ySpeed = -1.0f;
        xSpeedRandomRange = xSpeed/3.0f;
        ySpeedRandomRange = ySpeed/3.0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        ySpeed += Random.Range(-ySpeedRandomRange, ySpeedRandomRange);
        xSpeed += Random.Range(-xSpeedRandomRange, xSpeedRandomRange);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if( isAlive )
            transform.Translate( xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0f );
        else
            spriteRenderer.color = new Color( spriteRenderer.color.r,
                                             spriteRenderer.color.g,
                                             spriteRenderer.color.b,
                                             spriteRenderer.color.a - fadeRate * Time.deltaTime );
	}

    public void Die()
    {
        m_collider.enabled = false;
        isAlive = false;
        xSpeed = 0;
        ySpeed = 0;
        StartCoroutine(DisappearSec( 0.7f ));
        //TODO Release textures
    }

    IEnumerator DisappearSec(float time)
    {
        yield return new WaitForSeconds( time );
        Destroy( gameObject );
    }

    public void SetPhotoDescriptor(PhotoDescriptor pd)
    {
        photoDescriptor = pd;
        spriteRenderer.sprite = pd.sprite;
        m_collider.size = spriteRenderer.sprite.bounds.size;
        m_collider.offset = new Vector2(0, 0);
    }
}
