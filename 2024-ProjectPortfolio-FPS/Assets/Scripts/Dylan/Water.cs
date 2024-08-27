using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] int gravity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.SetGravity(gravity);
            gameManager.instance.playerScript.isSwimming = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.SetGravity(gameManager.instance.worldGravity);
            gameManager.instance.playerScript.isSwimming = false;
        }
    }
}
