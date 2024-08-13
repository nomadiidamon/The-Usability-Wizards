using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    //[SerializeField] LineRenderer lineRenderer;

    [SerializeField] int HP;

    public int GetCurrentHealth()
    {
        return HP;
    }

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    
    Vector3 move;
    Vector3 playerVel;

    int jumpCount;
    public int HPOrig;

    bool isSprinting;
    bool isShooting;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gameManager.instance.isPaused)
        {
            movement();

        }
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += move * speed * Time.deltaTime;

        move = Input.GetAxis("Vertical") * transform.forward +
               Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(shoot());

    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        //lineRenderer.SetPosition(0, Camera.main.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            //lineRenderer.SetPosition(1, hit.point);

            Instantiate(bullet, shootPos.position, shootPos.rotation);
            //Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        //gameManager.instance.SetPlayersCurrentHealth(HP);
        updatePlayerUI();
        StartCoroutine(flashDamage());
        // I'm dead!
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.flashDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playersHealthPool.fillAmount = (float)HP / HPOrig;
    }

}
