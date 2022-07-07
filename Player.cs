using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    public GameManaer gameManaer;
    public ObjectManager objectManager;

    public ParticleSystem bullet_Upgrade_Par;
    public ParticleSystem potion_Par;
    public ParticleSystem hit_Par;
    public ParticleSystem[] heathDown_Par;
    public ParticleSystem dead_Par;

    public GameObject[] healthImage;
    public GameObject[] bulletsImage;

    public AudioClip shotSound;
    public AudioClip hitSound;
    public AudioClip lvUpSound;
    public AudioClip healthUpSound;
    public AudioClip deadSound;

    public int health;
    public int speed;
    public int nowBulletLev;
    
    int bulletSpeed;
    float bulletShotDelay;
    bool[] allowBullets = new bool[5];
    bool isShot;
    bool unDeadMode;

    private void Start()
    {
        allowBullets[nowBulletLev] = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Move();
        Attack();
        InputChangeBullet();
        CheckAnimeEnd();
        CheckExitGame();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.position += new Vector3(h, v, 0) * Time.deltaTime * speed;
    }

    void Attack()
    {
        if(bulletShotDelay == 0 || bulletSpeed == 0)
        {
            Bullet bullet = objectManager.getObj(nowBulletLev).GetComponent<Bullet>();
            bulletShotDelay = bullet.shotDelay;
            bulletSpeed = bullet.speed;
            bulletsImage[nowBulletLev].SetActive(true);
            bulletsImage[nowBulletLev].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        if (Input.GetKey(KeyCode.Space) && !isShot)
        {
            ChangeSound(shotSound);
            isShot = true;
            GameObject bullet = objectManager.getObj(nowBulletLev);
            bullet.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            bullet.SetActive(true);

            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            bulletRigid.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Force);

            Invoke("AllowShot", bulletShotDelay); // 총알 발사 딜레이
        }
    }

    void InputChangeBullet()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeBullet(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeBullet(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeBullet(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeBullet(4);
    }

    void ChangeBullet(int pressNum)
    {
        if (allowBullets[pressNum])
        {
            Bullet bullet = objectManager.getObj(pressNum).GetComponent<Bullet>();
            bulletShotDelay = bullet.shotDelay;
            bulletSpeed = bullet.speed;
            bulletsImage[nowBulletLev].GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
            bulletsImage[pressNum].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            nowBulletLev = pressNum;
        }
    }

    void CheckAnimeEnd()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0 &&
            !unDeadMode)
        {
            animator.SetBool("isHit", false);
        }
    }

    void CheckExitGame()
    {
        Vector3 bounceWall = new Vector3(0, 0, 0);
        if (transform.position.x >= 3)
            bounceWall = new Vector3(-0.1f, 0, 0);
        else if (transform.position.x <= -3)
            bounceWall = new Vector3(0.1f, 0, 0);

        if (transform.position.y >= 4.2)
            bounceWall = new Vector3(0, -0.1f, 0);
        else if (transform.position.y <= -4.2)
            bounceWall = new Vector3(0, 0.1f, 0);

        transform.position += bounceWall;
    }

    void AllowShot()
    {
        isShot = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !unDeadMode)
        {
            health--;
            ChangeSound(hitSound);

            if (health <= 0)
            {
                spriteRenderer.sprite = null;
                dead_Par.Play();
                Invoke("GamePause", 2f);
            }

            animator.SetBool("isHit", true);
            hit_Par.Play();
            OnUnDeadMode();
            healthImage[health].SetActive(false);
            heathDown_Par[health].Play();

            if(collision.gameObject.name.Equals("Bomb(Clone)")) // 폭탄 충돌 (마우스 클릭) 애니메이션 시작
            {
                SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(1, 1, 1, 0);
                ParticleSystem boom_Par = collision.gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
                AudioSource boom_audio = collision.gameObject.GetComponent<AudioSource>();
                boom_Par.Play();
                boom_audio.Play();
                StartCoroutine(ReturnBoomForm(collision.gameObject, spriteRenderer));
            }
        }

        if (collision.tag == "Border")
        {
            int backX = -1;
            int backY = -1;

            if (transform.position.x < 0)
                backX = 1;
            if (transform.position.y < 0)
                backY = 1;

            transform.position += new Vector3(backX, backY, 0) * 0.2f;
        }

        if (collision.tag == "Item_Bullet" && !allowBullets[4])
        {
            ChangeSound(lvUpSound);
            collision.gameObject.SetActive(false);
            nowBulletLev++;

            Bullet bullet = objectManager.getObj(nowBulletLev).GetComponent<Bullet>();
            bulletShotDelay = bullet.shotDelay;
            bulletSpeed = bullet.speed;
            bulletsImage[nowBulletLev].SetActive(true);
            allowBullets[nowBulletLev] = true;
            bulletsImage[nowBulletLev].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            bulletsImage[nowBulletLev - 1].GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);

            bullet_Upgrade_Par.Play();
        }

        if (collision.tag == "Item_Potion" && health < 3)
        {
            ChangeSound(healthUpSound);
            healthImage[health].SetActive(true);
            collision.gameObject.SetActive(false);
            health += 1;
            potion_Par.Play();
        }
    }

    IEnumerator ReturnBoomForm(GameObject boom, SpriteRenderer boom_SpriteRenderer)
    {
        yield return new WaitForSeconds(1);
        boom_SpriteRenderer.color = new Color(1, 1, 1, 1);
        boom.SetActive(false);
    }

    void ChangeSound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void OnUnDeadMode()
    {
        unDeadMode = true;
        Invoke("OffUnDeadMode", 3f);
    }

    void OffUnDeadMode()
    {
        unDeadMode = false;
    }

    void GamePause()
    {
        Time.timeScale = 0;
        for(int i = 0; i < gameManaer.gameOverUI.Length; i++)
            gameManaer.gameOverUI[i].SetActive(true);
        gameManaer.gameOverUI[2].GetComponent<TextMeshPro>().text = gameManaer.score.ToString() + " Score";
    }
}
