using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject bullet_Lev1_Prefeb;
    public GameObject bullet_Lev2_Prefeb;
    public GameObject bullet_Lev3_Prefeb;
    public GameObject bullet_Lev4_Prefeb;

    public GameObject enemy_Basic1_Prefeb;
    public GameObject enemy_Basic2_Prefeb;
    public GameObject enemy_Boss_Prefeb;
    public GameObject boom_Prefeb;

    public GameObject item_Bullet_Prefeb;
    public GameObject item_Potion_Prefeb;


    GameObject[] bullet_Lev1;
    GameObject[] bullet_Lev2;
    GameObject[] bullet_Lev3;
    GameObject[] bullet_Lev4;

    GameObject[] enemy_Basic1;
    GameObject[] enemy_Basic2;
    GameObject[] enemy_Boss;
    GameObject[] boom;

    GameObject[] item_Bullet;
    GameObject[] item_Potion;

    void Start()
    {
        bullet_Lev1 = CreateArray(bullet_Lev1_Prefeb, 12);
        bullet_Lev2 = CreateArray(bullet_Lev2_Prefeb, 7);
        bullet_Lev3 = CreateArray(bullet_Lev3_Prefeb, 6);
        bullet_Lev4 = CreateArray(bullet_Lev4_Prefeb, 5);

        enemy_Basic1 = CreateArray(enemy_Basic1_Prefeb, 6);
        enemy_Basic2 = CreateArray(enemy_Basic2_Prefeb, 6);
        enemy_Boss = CreateArray(enemy_Boss_Prefeb, 2);
        boom = CreateArray(boom_Prefeb, 6);

        item_Bullet = CreateArray(item_Bullet_Prefeb, 3);
        item_Potion = CreateArray(item_Potion_Prefeb, 3);
    }

    GameObject[] CreateArray(GameObject tarPre, int count)
    {
        GameObject[] tarArr = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            GameObject makeObj = Instantiate(tarPre);
            makeObj.SetActive(false);
            tarArr[i] = makeObj;
        }

        return tarArr;
    }

    public GameObject getObj(int itemNum)
    {
        GameObject[] tarObjs = null;

        switch (itemNum)
        {
            case 1:
                tarObjs = bullet_Lev1;
                break;
            case 2:
                tarObjs = bullet_Lev2;
                break;
            case 3:
                tarObjs = bullet_Lev3;
                break;
            case 4:
                tarObjs = bullet_Lev4;
                break;
            case 5:
                tarObjs = enemy_Basic1;
                break;
            case 6:
                tarObjs = enemy_Basic2;
                break;
            case 7:
                tarObjs = boom;
                break;
            case 8:
                tarObjs = enemy_Boss;
                break;
            case 9:
                tarObjs = item_Bullet;
                break;
            case 10:
                tarObjs = item_Potion;
                break;
        }

        for (int i = 0; i < tarObjs.Length; i++)
        {
            if (!tarObjs[i].activeSelf)
            {
                return tarObjs[i];
            }
        }

        return null;
    }
}
