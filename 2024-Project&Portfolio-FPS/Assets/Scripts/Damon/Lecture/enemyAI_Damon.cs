using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Collider meleeCollider;

    [SerializeField] int HP;
    [SerializeField] int startingHealth;
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;

    [SerializeField] Image hpbar;
    [SerializeField] int roamDistance;
    [SerializeField] int roamTimer;
    [SerializeField] int animSpeedTrans;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    //lecture shoot angle
    [SerializeField] int shootAngle;

    bool isShooting;
    bool playerInRange;
    bool isRoaming;

    float angleToPlayer;
    float stoppingDistanceOriginal;

    Vector3 playerDir;
    Vector3 startingPosition;

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHealth;
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animSpeedTrans));

        if (playerInRange && !canSeePlayer())
        {
            if(!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(roam());
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(roam());
        }
  

    }

    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;
        Vector3 randomDistance = Random.insideUnitSphere * roamDistance;
        randomDistance += startingPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDistance, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
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

                agent.stoppingDistance = stoppingDistanceOriginal;
                return true;
            }
        }
        agent.stoppingDistance = 0;
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
        agent.SetDestination(gameManager.instance.player.transform.position);
        StopCoroutine(roam());

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
        animator.SetTrigger("Shoot");
        //createProjectile();
        //Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        //direction.Normalize();
        //Quaternion bulletRotation = Quaternion.LookRotation(direction);
        //Instantiate(bullet, shootPos.transform.position, bulletRotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void createProjectile()
    {
        Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        direction.Normalize();
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        //Debug.Log("Pew!");
        Instantiate(bullet, shootPos.transform.position, bulletRotation);
    }

    public void weaponColliderOn()
    {
        meleeCollider.enabled = true;
    }

    public void weaponColliderOff()
    {
        meleeCollider.enabled = false;

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
            agent.stoppingDistance = 0;
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

