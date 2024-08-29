using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] int lavaDamageAmount = 1;
    [SerializeField] float damageInterval = 0.5f;

    private Dictionary<Collider, Coroutine> activeCoroutines = new Dictionary<Collider, Coroutine>();
    private bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        meleeAI_Dylan enemyMeleeScript = other.GetComponentInParent<meleeAI_Dylan>();
        enemyAI enemyScript = other.GetComponentInParent<enemyAI>();

        if (enemyMeleeScript != null && other == enemyMeleeScript.lavaCollider)
        {
            if (!activeCoroutines.ContainsKey(other))
            {
                Coroutine coroutine = StartCoroutine(ApplyLavaDamage(enemyMeleeScript));
                activeCoroutines[other] = coroutine;
            }
        }
        else if (other.CompareTag("Player"))
        {
            IDamage playerDamageable = other.GetComponent<IDamage>();
            if (playerDamageable != null)
            {
                if (!activeCoroutines.ContainsKey(other))
                {
                    Coroutine coroutine = StartCoroutine(ApplyLavaDamage(playerDamageable));
                    activeCoroutines[other] = coroutine;
                }
            }
        }
        else if (enemyScript != null && other == enemyScript.lavaCollider)
        {
            if (!activeCoroutines.ContainsKey(other))
            {
                Coroutine coroutine = StartCoroutine(ApplyLavaDamage(enemyScript));
                activeCoroutines[other] = coroutine;
            }
        }
    }

    private void Update()
    {
        if (!isActive) return;

        var coroutinesToRemove = new List<Collider>();

        foreach (var kvp in activeCoroutines)
        {
            var collider = kvp.Key;
            var coroutine = kvp.Value;

            if (collider == null || collider.gameObject == null || !collider.gameObject.activeInHierarchy)
            {
                StopCoroutine(coroutine);
                coroutinesToRemove.Add(collider);
            }
        }

        // Remove invalid coroutines from the dictionary
        foreach (var collider in coroutinesToRemove)
        {
            activeCoroutines.Remove(collider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive) return;

        if (activeCoroutines.TryGetValue(other, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(other);
        }
    }

    private IEnumerator ApplyLavaDamage(IDamage damageable)
    {
        while (isActive && damageable != null)
        {
            if (damageable is Component component && component.gameObject != null && component.gameObject.activeInHierarchy)
            {
<<<<<<< Updated upstream
                damageable.takeDamage(lavaDamageAmount);
=======
                // Check if the damageable is still a valid object
                if (damageable is enemyAI enemy && enemy != null && enemy.gameObject != null && enemy.gameObject.activeInHierarchy)
                {
                    damageable.takeDamage(lavaDamageAmount);
                }
                else if (damageable is IDamage player && player != null)
                {
                    damageable.takeDamage(lavaDamageAmount);
                }
                else
                {
                    yield break;
                }
>>>>>>> Stashed changes
                yield return new WaitForSeconds(damageInterval);
            }
            else
            {
                yield break;
            }
        }
    }

    public void StartLava()
    {
        isActive = true;
    }

    public void StopLava()
    {
        isActive = false;

        // Stop all active coroutines
        foreach (var kvp in activeCoroutines)
        {
            StopCoroutine(kvp.Value);
        }
        activeCoroutines.Clear();
    }
}
