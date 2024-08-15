using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretControl : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;

    [SerializeField] int health;
    [SerializeField] int startingHealth;
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] Image hpbar;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;

    float angleToPlayer;
    Vector3 playerDir;
    Color colorOriginal;

    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
        colorOriginal = model.material.color;
        UpdateHealthBar();
        gameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && canSeePlayer())
        {
            
        }
    }

    void UpdateHealthBar()
    {
        if (hpbar != null)
        {
            hpbar.fillAmount = (float)health / startingHealth;
        }
    }

    public void takeDamage(int amount)
    {
        Debug.Log("Turret took damage");
        health -= amount;

        UpdateHealthBar();  // Update the health bar after taking damage

        StartCoroutine(flashRed());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - shootPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                return true;
            }
                
        }

        return false;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(2.0f);
        model.material.color = colorOriginal;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        direction.Normalize();

        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        Instantiate(bullet, shootPos.transform.position, bulletRotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
