using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManaer : MonoBehaviour
{
    AudioSource audioSource;

    public Camera gameCamera;
    public ObjectManager objectManager;

    public GameObject[] enemySpawnPoint;
    public GameObject[] gameOverUI;

    public int score;
    public int enemySpawnTime;

    List<GameObject> spawningBooms = new List<GameObject>(); 
    float enemySpawnDelay;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    void Update()
    {
        EnemySpawn();
        ClickBomb();
    }

    void ClickBomb()
    {
        if(Input.GetMouseButtonDown(0))
        {
            for(int i = 0; i < spawningBooms.Count; i++)
            {
                Vector3 boomPos = spawningBooms[i].transform.position;
                Vector3 mousePos = Input.mousePosition;
                mousePos = gameCamera.ScreenToWorldPoint(mousePos); 

                if ( (boomPos.x + 1 > mousePos.x && boomPos.x - 1 < mousePos.x) &&
                     (boomPos.y + 1 > mousePos.y && boomPos.y - 1 < mousePos.y))
                {
                    SpriteRenderer spriteRenderer = spawningBooms[i].GetComponent<SpriteRenderer>();
                    spriteRenderer.color = new Color(1, 1, 1, 0);
                    ParticleSystem boom_Par = spawningBooms[i].transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    AudioSource boom_audio = spawningBooms[i].GetComponent<AudioSource>();
                    boom_Par.Play();
                    boom_audio.Play();

                    StartCoroutine(ReturnBoomForm(spawningBooms[i], spriteRenderer));
                }
            }
        }
    }

    IEnumerator ReturnBoomForm(GameObject boom, SpriteRenderer spriteRenderer)
    {
        yield return new WaitForSeconds(1);
        spriteRenderer.color = new Color(1, 1, 1, 1);
        boom.SetActive(false);
    }

    void EnemySpawn()
    {
        enemySpawnDelay += Time.deltaTime;

        if (enemySpawnTime < enemySpawnDelay)
        {
            int ranEnemyPos = Random.Range(0, 9);
            Vector3 enemyPos = enemySpawnPoint[ranEnemyPos].transform.position;
            int moveDir = 0;
            if(ranEnemyPos == 5 || ranEnemyPos == 6) // ¿ÞÂÊ
                moveDir = 1;
            else if (ranEnemyPos == 7 || ranEnemyPos == 8) // ¿ÞÂÊ
                moveDir = -1;

            int ranEnemyNum = Random.Range(5, 8); // ÆøÅº Æ÷ÇÔ
            GameObject enemy = objectManager.getObj(ranEnemyNum);
            enemy.transform.position = enemyPos;
            enemy.SetActive(true);

            int enemySpeed = 90;
            if(ranEnemyNum == 7)
                spawningBooms.Add(enemy);

            else
                enemySpeed = enemy.GetComponent<Enemy>().speed;
            Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
            enemyRigid.AddForce(new Vector2(moveDir, -1) * enemySpeed, ForceMode2D.Force);

            int bossSpawn = Random.Range(0, 100);
            if(bossSpawn < 2)
            {
                GameObject boss = objectManager.getObj(8);
                boss.transform.position = new Vector3(0, 6);
                boss.SetActive(true);

                int bossSpeed = boss.GetComponent<Enemy>().speed;
                Rigidbody2D bossRigid = boss.GetComponent<Rigidbody2D>();
                bossRigid.AddForce(Vector2.down * bossSpeed, ForceMode2D.Force);
            }

            enemySpawnDelay = 0;
        }
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(1);
    }
}
