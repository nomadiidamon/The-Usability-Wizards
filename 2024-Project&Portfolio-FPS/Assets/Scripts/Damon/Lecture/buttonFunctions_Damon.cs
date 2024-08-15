using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    public void resume()
    {
        Debug.Log("Resumed");

        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        Debug.Log("Restarted");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
        gameManager.instance.CheckpointReached = false;
        gameManager.instance.respawns = gameManager.instance.respawnsOriginal;
    }

    public void respawn()
    {
        if (!gameManager.instance.CheckpointReached || gameManager.instance.respawns == 0) { return; }
        gameManager.instance.updateRespawnCount(-1);
        Debug.Log("Second Chance!");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.SetPlayerLocation();
        gameManager.instance.playerScript.SetHealth(gameManager.instance.playerScript.HPOrig / 3);
        gameManager.instance.playerScript.updatePlayerUI();
        gameManager.instance.stateUnpause();
        gameManager.instance.respawns--;
    }

    public void quit()
    {
        Debug.Log("Quitting");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }



}
