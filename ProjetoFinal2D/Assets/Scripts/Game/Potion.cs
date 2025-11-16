using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] GameObject light_;
    SpriteRenderer spr;
    [SerializeField] Sprite sprite;
    [SerializeField] float range;
    [SerializeField] LayerMask playerLayer;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        RaycastHit2D sphere = Physics2D.CircleCast(transform.position, range, Vector2.zero, 0, playerLayer);

        if(sphere && Input.GetKeyDown(KeyCode.E) && light_.activeSelf && !GameManager.instance.shield)
        {
            spr.sprite = sprite;
            light_.SetActive(false);
            GameManager.instance.GainShield();
        }
    }
}
