using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint; // tetap di pusat (0,0)
    private EnemyController currentTarget;
    public float rotateSpeed = 5f;

    void Update()
    {
        // Rotasi ke arah target jika ada

        if (currentTarget != null)
        {
            Vector3 dir = currentTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // tambahkan offset di sini
            Quaternion targetRot = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }

    }

    public void SetTarget(EnemyController target)
    {
        currentTarget = target;
    }

    public void FireAtTarget()
    {
        if (currentTarget == null) return;
        // Peluru keluar dari pusat (firePoint), mengikuti rotasi player
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        bullet.GetComponent<Projectile>().SetTarget(currentTarget.transform.position);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Peluru");
        }
    }
}
