using UnityEngine;

public class Slime : MonoBehaviour
{
    public GameManager gameManager;
    public float speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed *  Time.deltaTime);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.PerderVidas(1);
        }
    }
}
