using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Item_Potion" || collision.tag == "Item_Bullet" || collision.tag == "Enemy" 
            || collision.tag == "Bullet")
        {
            collision.gameObject.SetActive(false);
        }
    }
}
