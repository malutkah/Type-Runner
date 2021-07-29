using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public BoxCollider2D collider2D;
    public Rigidbody2D rb2D;

    private float width;
    private float scrollSpeed = -2f;
  
    // Start is called before the first frame update
    void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        width = collider2D.size.x;
        collider2D.enabled = false;

        rb2D.velocity = new Vector2(scrollSpeed, 0);

    }

    // Update is called once per frame
    void Update()
    {
        ResetBackgroundPosition();
    }

    private void ResetBackgroundPosition()
    {
        if (transform.position.x < -width)
        {
            Vector2 resetPosition = new Vector2(width * 2f, 0);
            transform.position = (Vector2)transform.position + resetPosition;
        }
    }
}
