using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    

    public GameObject flashDamageScreen;
    public GameObject restoreHealthScreen;
    public Image playersHealthPool;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text uiPrompt;


    public GameObject player;
    public playerController playerScript;
    

    public bool isPaused;

    int enemyCount;

    private GameObject currentDoor;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);

            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }

        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;

    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            Debug.Log("You Win");

            // you win!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);

        }
    }

    public void youLose()
    {
        if (isPaused)
        {
            isPaused = !isPaused;
            menuActive = null;
        }
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(isPaused);

    }

    public void UpdateUIPrompt(string message, GameObject door)
    {
        currentDoor = door;
        uiPrompt.text = message;
        uiPrompt.enabled = !string.IsNullOrEmpty(message);
    }

    public void ClearUIPrompt(GameObject door) 
    {
    
        if (currentDoor == door)
        {

            uiPrompt.enabled = false;
            currentDoor = null;

        }

    
    }

    public bool IsUIPromptActive()
    {

        return uiPrompt.enabled;


    }




}
