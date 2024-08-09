using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.SetPlayersCurrentHealth(gameManager.instance.playerScript.HPOrig);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.playerScript.GetCurrentHealth() < gameManager.instance.playerScript.HPOrig)
        {
            gameManager.instance.SetPlayersCurrentHealth(gameManager.instance.playerScript.GetCurrentHealth());
        }
    }
}
