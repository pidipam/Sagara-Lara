using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 targetPos;

    public void SetTarget(Vector3 target)
    {
        targetPos = target;
        Vector3 dir = (targetPos - transform.position).normalized;
        GetComponent<Rigidbody2D>().velocity = dir * speed;
        Destroy(gameObject, 2f); // auto destroy
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}
