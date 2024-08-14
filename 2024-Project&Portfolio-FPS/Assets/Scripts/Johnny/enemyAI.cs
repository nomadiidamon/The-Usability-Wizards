using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int health;
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;

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
        colorOriginal = model.material.color;
        gameManager.instance.updateGameGoal(1);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (playerInRange && canSeePlayer())
        {
            
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
                }

                return true;
            }
        }

        return false;
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    public void takeDamage(int amount)
    {
        health -= amount;

        StartCoroutine(flashRed());

        if (health <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
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
