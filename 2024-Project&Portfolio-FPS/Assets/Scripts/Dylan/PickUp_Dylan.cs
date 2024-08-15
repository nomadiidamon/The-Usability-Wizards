using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Dylan : MonoBehaviour
{
    [SerializeField] enum pickUpType { health, speed, damage }              // types of pickups
    [SerializeField] pickUpType type;
    [SerializeField] Rigidbody rbPickup;                                   // rigid body needed for collision detection

    Color color = Color.white;           // default pick up color

    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case pickUpType.health:
                {
                    color = Color.red;
                    break;
                }
            case pickUpType.speed:
                {
                    color = Color.blue;
                    break;
                }
            case pickUpType.damage:
                {
                    color = Color.yellow;
                    break;
                }
            default:
                break;
        }
        SetObjectColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 1f, 0f));

    }
    void SetObjectColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();        // get access to the renderer on the pickup game object
        renderer.material.color = color;                     // set color to new color
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case pickUpType.health:
                    {
                        if (gameManager.instance.playerScript.GetHealth() < gameManager.instance.playerScript.HPOrig)
                        {
                            gameManager.instance.playerScript.SetHealth(gameManager.instance.playerScript.GetHealth() + 1);
                            gameManager.instance.playerScript.updatePlayerUI();
                            Destroy(gameObject);
                        }
                        break;
                    }
                case pickUpType.speed:
                    {
                        gameManager.instance.playerScript.SetSpeed(gameManager.instance.playerScript.GetSpeed() + 3);
                        Destroy(gameObject);
                        break;
                    }
                case pickUpType.damage:
                    {
                        gameManager.instance.playerScript.SetDamage(gameManager.instance.playerScript.GetDamage() + 1);
                        //gameManager.instance.playerScript.GetComponent<Damage>().SetDamageAmount(gameManager.instance.playerScript.GetComponent<Damage>().GetDamageAmount() + 1);
                        Destroy(gameObject);
                        break;
                    }
                default:
                    break;

            }
        }
    }
}
