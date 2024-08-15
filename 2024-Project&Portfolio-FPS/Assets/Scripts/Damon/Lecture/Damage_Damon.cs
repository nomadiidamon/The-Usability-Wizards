using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, impact }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    public int GetDamageAmount()
    {
        return damageAmount;
    }
    public void SetDamageAmount(int amount)
    {
        damageAmount = amount;
    }
//<<<<<<< HEAD
//=======

//>>>>>>> WorkingBackup/NewMain
    [SerializeField] float damageDelay;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamageable = true;
//<<<<<<< HEAD
    int player = 3;
//=======
    int originalLayer;
//>>>>>>> WorkingBackup/NewMain
    int immune = 9;                // layer 9 is immune

    // Start is called before the first frame update
    void Start()
    {
        originalLayer = gameManager.instance.player.layer;
        if (type == damageType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || !isDamageable) 
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();

        if (damage != null)
        {
            damage.takeDamage(damageAmount);
//<<<<<<< HEAD
            if (type == damageType.stationary)
            { StartCoroutine(delayedDamage()); }
//=======
//            StartCoroutine(delayedDamage());
//>>>>>>> WorkingBackup/NewMain
        }
        if (type == damageType.bullet)
        { Destroy(gameObject); }

    }
//<<<<<<< HEAD
//=======

//>>>>>>> WorkingBackup/NewMain
    IEnumerator delayedDamage()
    {
        gameManager.instance.player.layer = immune;              // layer 9 is immune
        yield return new WaitForSeconds(damageDelay);
//<<<<<<< HEAD
        gameManager.instance.player.layer = player;              // layer 3 is player
//=======
        gameManager.instance.player.layer = originalLayer;              // layer 3 is player
//>>>>>>> WorkingBackup/NewMain
    }
}
