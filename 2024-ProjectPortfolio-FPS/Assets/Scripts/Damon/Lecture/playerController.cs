using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] GameObject shield;

    [Header("-----Attributes-----")]
    [Range(0, 10)] [SerializeField] int HP;
    [Range(1, 50)] [SerializeField] int speed;
    [Range(2, 10)] [SerializeField] int sprintMod;
    [Range(1, 3)] [SerializeField] int jumpMax;
    [Range(8, 20)] [SerializeField] int jumpSpeed;
    [Range(15, 30)] [SerializeField] int gravity;

    [Header("-----Guns-----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] float deflectionSpeed;

    [Header("-----Sounds-----")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;


    Vector3 move;
    Vector3 playerVel;


    int jumpCount;
    public int HPOrig;
    public int GetOriginalHpAmount() {  return HPOrig; }
    bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;

    public bool sprintToggle;
    bool sprintingPressed;

    public int selectedGun;
    public bool isCreator;
    public bool isSwimming;
    public bool isDeflecting;
    public bool isHoldingShield;
    private BoxCollider myCollider;

    public GameObject objectHeld;           // object ready to shoot

    public List<gunStats> GetGunList() { return gunList; }

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
        spawnPlayer();
        myCollider = shield.GetComponent<BoxCollider>();
    }

    public void spawnPlayer()
    {
        HP = HPOrig;
        updatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPosition.transform.position;
        controller.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gameManager.instance.isPaused)
        {
            movement();
            selectGun();
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

        move = Input.GetAxis("Vertical") * transform.forward +
               Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * speed * Time.deltaTime);

        if (isSwimming && Input.GetButton("Jump"))
        {
            playerVel.y = jumpSpeed;
        }
        else if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            gameManager.instance.PlayAud(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }

        if (Input.GetButtonDown("Save Object"))
        {
            saveObjectBullet();
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (gunList.Count < 1)
        {
            
        }
        else
        {
            if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(shoot());

        }

        if (controller.isGrounded && move.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(playStep());
        }
    }

    IEnumerator playStep() 
    {
        isPlayingSteps = true;

        gameManager.instance.PlayAud(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        isPlayingSteps = false;

    }

    void sprint()
    {
        if (sprintToggle)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                if (isSprinting)
                {
                    isSprinting = false;
                }
                else if (!isSprinting)
                {
                    isSprinting = true;
                }             
            }
            if (Input.GetButtonDown("Sprint") && isSprinting)
            {
                speed *= sprintMod;
            }
            else if (Input.GetButtonDown("Sprint") && !isSprinting)
            {
                speed /= sprintMod;
            }
        }
        else
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
    }

    public void SetSprintToggle(bool toggle)
    {
        sprintToggle = toggle;

        if (!sprintToggle && isSprinting)
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator shoot()
    {
        if (!gunList[selectedGun].isShield)
        {
            isShooting = true;

            StartCoroutine(flashMuzzle());
            gameManager.instance.PlayAud(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);
            Instantiate(bullet, shootPos.position, shootPos.rotation);
            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
        else
        {
            Debug.Log("I'm trying to block!!");
            isDeflecting = true;
            isShooting = true;

            //shieldStats thing = (shieldStats)gunList[selectedGun];
            
            StartCoroutine(Deflecting(myCollider));

            Collider[] hitColliders = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, Quaternion.identity);

            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject.CompareTag("Enemy Bullet"))
                {
                    int damage = collider.gameObject.GetComponent<Damage>().GetDamageAmount();
                    gunList[selectedGun].shootDamage = damage;
                    StartCoroutine(flashDeflection());
                    Debug.Log("I did it!");
                    gameManager.instance.PlayAud(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);
                    collider.gameObject.GetComponent<Rigidbody>().velocity = collider.gameObject.transform.forward * deflectionSpeed;
                    yield return new WaitForSeconds(shootRate);
                }
            }
            isShooting = false;
            isDeflecting = false;
            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator Deflecting(BoxCollider coll)
    {
        coll.enabled = true;
        Debug.Log("Collider is active!");

        yield return new WaitForSeconds(shootRate);
        coll.enabled = false;
        Debug.Log("Collider is off!");

    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator flashDeflection()
    {
        Color colorOriginal = muzzleFlash.GetComponent<Renderer>().material.color;
        muzzleFlash.GetComponent<Renderer>().material.color = Color.blue;
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.GetComponent<Renderer>().material.color = colorOriginal;
        muzzleFlash.SetActive(false);
    }

    public void takeDamage(int amount)
    {
        if (isDeflecting)
        {
            amount = 0;
            return;
        }

        HP -= amount;
        //Debug.Log("Ouch!");
        if (HP < 0) { HP = 0; }

        gameManager.instance.PlayAud(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        updatePlayerUI();
        StartCoroutine(flashDamage());
        
        // I'm dead!
        if (HP <= 0)
        {
            gameManager.instance.isPaused = false;
            gameManager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.flashDamageScreen.SetActive(false);
    }

    IEnumerator RestoreHealthScreen()
    {
        gameManager.instance.restoreHealthScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.restoreHealthScreen.SetActive(false);
    }

    IEnumerator IncreaseDamageScreen()
    {
        gameManager.instance.increaseDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.increaseDamageScreen.SetActive(false);
    }

    IEnumerator RaiseSpeedScreen()
    {
        gameManager.instance.raiseSpeedScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.raiseSpeedScreen.SetActive(false);
    }

    public void RestoreHealth()
    {
        updatePlayerUI();
        StartCoroutine(RestoreHealthScreen());
    }

    public void IncreaseDamage()
    {
        StartCoroutine(IncreaseDamageScreen());
    }

    public void RaiseSpeed()
    {
        StartCoroutine(RaiseSpeedScreen());
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playersHealthPool.fillAmount = (float)HP / HPOrig;
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        shootDist = gun.shootDistance;
        shootRate = gun.shootRate;
        isCreator = gun.isCreator;
        isHoldingShield = gun.isShield;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count -1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        bullet.GetComponent<Damage>().SetDamageAmount(shootDamage);
        shootDist = gunList[selectedGun].shootDistance;
        shootRate = gunList[selectedGun].shootRate;
        isCreator = gunList[selectedGun].isCreator;
        isHoldingShield = gunList[selectedGun].isShield;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

    }

    public int GetHealth()
    {
        return HP;
    }
    public void SetHealth(int amount)
    {
        HP = amount;
    }
    public int GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(int amount)
    {
        speed = amount;
    }
    public int GetDamage()
    {
        return shootDamage;
    }
    public void SetDamage(int amount)
    {
        shootDamage = amount;
    }
    public int GetGravity()
    {
        return gravity; 
    }
    public void SetGravity(int amount)
    {
        gravity = amount;
        jumpSpeed = amount / 2;
    }

    public void saveObjectBullet()
    {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 50, ~ignoreMask))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.CompareTag("Creatable"))
                {
                    objectHeld = hit.collider.gameObject;
                }
            }
    }
}
