using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameManaer gameManaer;
    ObjectManager objectManager;
    BoxCollider2D boxCollider;
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer sprite;
    AudioSource audioSource;
    Sprite originalSprite;

    public int speed;
    public int maxHealth;
    public int health;

    private void Start()
    {
        health = maxHealth;

        boxCollider = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        gameManaer = GameObject.Find("GameManager").GetComponent<GameManaer>();
        originalSprite = sprite.sprite;
    }

    private void Update()
    {
        CheckAnimeEnd();
    }

    void CheckAnimeEnd()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("isHit", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("isDead", false);
            ResetEnemySetting();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Bullet":
                int bulletDamage = collision.gameObject.GetComponent<Bullet>().damage;

                if (bulletDamage != 0)
                {
                    animator.SetBool("isHit", true);
                    health -= bulletDamage;
                }

                if (health <= 0)
                {
                    audioSource.Play();
                    boxCollider.enabled = false;
                    animator.SetBool("isDead", true);
                    int itemPercent = Random.Range(0, 100);

                    if (itemPercent < 50 && itemPercent > 5) // Æ÷¼Ç È®·ü
                    {
                        GameObject potion = objectManager.getObj(10);
                        potion.SetActive(true);
                        potion.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 50);
                    }
                    if (itemPercent < 50) // ¾÷±×·¹ÀÌµå È®·ü
                    {
                        GameObject bulletLvUp = objectManager.getObj(9);
                        bulletLvUp.SetActive(true);
                        bulletLvUp.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 70);
                    }
                    gameManaer.score++;
                }

                collision.gameObject.SetActive(false); // ÃÑ¾Ë ¾ø¾Ö±â
                break;
            case "Border":
                ResetEnemySetting();
                break;
        }
    }

    void ResetEnemySetting()
    {
        health = maxHealth;
        this.gameObject.SetActive(false);
        rigid.AddForce(Vector3.zero);
        sprite.sprite = originalSprite;
        boxCollider.enabled = true;
        transform.localScale = new Vector3(0.15f, 0.15f, 1f);
        if(this.gameObject.name.Equals("Enemy_Boss"))
        {
            transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        }
    }
}
