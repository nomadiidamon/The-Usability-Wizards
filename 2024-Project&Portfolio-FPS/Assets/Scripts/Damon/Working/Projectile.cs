using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject projectile;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 location = gameManager.instance.player.GetComponent<playerController>().transform.position;
        Quaternion rotation = gameManager.instance.player.GetComponent<playerController>().transform.rotation;
        Instantiate(projectile, location, rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
