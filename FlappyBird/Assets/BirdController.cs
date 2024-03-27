using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdController : MonoBehaviour
{
    public float jumpForce;
    public Rigidbody2D rigidbody;

    public int Points;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            rigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SceneManager.LoadScene("FlappyBird_Gameplay");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("ScorePoint"))
        {
            Points++;
        }
    }
}
