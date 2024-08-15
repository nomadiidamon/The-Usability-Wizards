using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary }
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

    [SerializeField] float damageDelay;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamageable = true;
    int originalLayer;
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

        if (damage != null )
        {
            damage.takeDamage(damageAmount);
            StartCoroutine(delayedDamage());
        }
        if (type == damageType.bullet)
        { Destroy(gameObject); }

    }

    IEnumerator delayedDamage()
    {
        gameManager.instance.player.layer = immune;              // layer 9 is immune
        yield return new WaitForSeconds(damageDelay);
        gameManager.instance.player.layer = originalLayer;              // layer 3 is player
    }
}
