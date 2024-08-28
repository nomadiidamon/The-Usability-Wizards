using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{

    [SerializeField] gunStats gun;
    //public BoxCollider myCollider;


    private void OnTriggerEnter(Collider other)
    {
        SphereCollider coll = this.GetComponent<SphereCollider>();
        if (other.CompareTag("Player") && coll.bounds.Contains(gameManager.instance.player.transform.position))
        {
            // give the palyer the gun
            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }


}
