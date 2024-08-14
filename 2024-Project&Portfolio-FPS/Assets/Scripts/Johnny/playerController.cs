using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    //[SerializeField] LineRenderer lr;

    [SerializeField] int health;
    [SerializeField] int moveSpeed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;


    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [SerializeField] float flashTime;

    Vector3 move;
    Vector3 playerVelocity;

    int jumpCount;
    int healthOriginal;

    bool isSprinting;
    bool isShooting;
  

    // Start is called before the first frame update
    void Start()
    {
        healthOriginal = health;
        updatePlayerUI();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            movement();
        }

        sprint();
    }

    void movement()
    {
        int origSpeed = moveSpeed;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveSpeed * Time.deltaTime * move;

        move = Input.GetAxis("Vertical") * transform.forward +
               Input.GetAxis("Horizontal") * transform.right;
        controller.Move(moveSpeed * Time.deltaTime * move);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if(Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(shoot());
        }
        
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            moveSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        //lr.useWorldSpace = true;
        //lr.SetPosition(0, Camera.main.transform.position);
        

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {

            //lr.SetPosition(1, hit.point);
            //Debug.Log(hit.collider.name);
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, flashTime);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        //lr.useWorldSpace = false;
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        // I'm dead!!
        if (health <= 0)
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
        gameManager.instance.playerHPBar.fillAmount = (float)health / healthOriginal;
    }
}
