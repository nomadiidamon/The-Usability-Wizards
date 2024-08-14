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

    [SerializeField] int HP;
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

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHealth;
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);

        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && canSeePlayer())
        {


        }


    }

    int getHealth()
    {
        return HP;
    }

    bool canSeePlayer ()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //Debug.Log(angleToPlayer);
        //Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                if (!isShooting)
                    StartCoroutine(shoot());

                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();

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
        HP -= amount;

        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
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

    void updateHPBar()
    {
        if (hpbar != null)
        {
            hpbar.fillAmount = (float)HP / startingHealth;
        }
    }

}

