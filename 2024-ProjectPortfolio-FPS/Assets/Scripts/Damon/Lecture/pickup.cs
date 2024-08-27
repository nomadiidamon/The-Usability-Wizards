using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{

    [SerializeField] gunStats gun;
    //public BoxCollider myCollider;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // give the palyer the gun
            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }


}
