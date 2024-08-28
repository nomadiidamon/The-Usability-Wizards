using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, impact, enemyBullet }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem targetHitEffect;


    [SerializeField] int damageAmount;
    public int GetDamageAmount() { return damageAmount; }
    public void SetDamageAmount(int amount) { damageAmount = amount; }

    [SerializeField] float damageDelay;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamageable = true;
    int originalLayer;
    int immune = 9;                // layer 9 is immune
    bool iHitShield = false;

    // Start is called before the first frame update
    void Start()
    {
        originalLayer = gameManager.instance.player.layer;
        if (type == damageType.bullet || type == damageType.enemyBullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.transform == transform.parent) //!isDamageable) 
        {
            return;
        }

        if (other.CompareTag("Environment"))
        {
            Instantiate(targetHitEffect, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();

        if (damage != null)
        {
            damage.takeDamage(damageAmount);
            StartCoroutine(delayedDamage());
        }
        if (type == damageType.bullet)
        {
            Instantiate(gameManager.instance.playerScript.GetGunList()[gameManager.instance.playerScript.selectedGun].hitEffect, this.transform.position, Quaternion.identity);
            if (gameManager.instance.playerScript.isCreator)
            {
                if (other.CompareTag("Ground"))
                {
                    //Debug.Log("Points colliding: " + other.GetComponent<Collision>().contacts.Length);
                    //Debug.Log("Normal on the first point: " + other.GetComponent<Collision>().contacts[0].normal);
                    //foreach (var item in other.GetComponent<Collision>().contacts)
                    //{
                    //    Debug.DrawRay(item.point, item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
                    //}
                    GameObject thing = gameManager.instance.playerScript.objectHeld;

                    Instantiate(gameManager.instance.playerScript.objectHeld, this.transform.position - new Vector3(0, 0, gameManager.instance.playerScript.objectHeld.transform.localScale.z / 2),
                        this.transform.rotation * Quaternion.AngleAxis(75, Vector3.right));

                }
                else
                { Instantiate(gameManager.instance.playerScript.objectHeld, this.transform.position - new Vector3(0, 0, gameManager.instance.playerScript.objectHeld.transform.localScale.z / 2), this.transform.rotation); }
            }
            Destroy(gameObject);
        }
        if (type == damageType.enemyBullet)
        {
            if (other.CompareTag("Shield"))
            {
                this.transform.SetParent(gameManager.instance.player.transform);
                this.transform.rotation = this.transform.rotation * Quaternion.Euler(0.0f, 180f, 0.0f);
                this.rb.velocity = transform.forward * speed;
                int delfectDamage = this.GetDamageAmount();
                this.tag = "Player Bullet";
                this.gameObject.layer = 3;
                SetDamageAmount(delfectDamage);

            }
            else
            {
                Instantiate(targetHitEffect, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

    }

    IEnumerator delayedDamage()
    {
        gameManager.instance.player.layer = immune;              // layer 9 is immune
        yield return new WaitForSeconds(damageDelay);
        gameManager.instance.player.layer = originalLayer;              // layer 3 is player
    }
}
