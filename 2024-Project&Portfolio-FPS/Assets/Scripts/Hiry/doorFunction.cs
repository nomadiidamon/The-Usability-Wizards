using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class doorFunction : MonoBehaviour
{

    [SerializeField] GameObject door;
    [SerializeField] float doorAppearTime;
    [SerializeField] float doorDistance;
    [SerializeField] string doorButton = "Interact";
    [SerializeField] TMP_Text uiPrompt;

    Transform player;
    bool isNearDoor = false;
    bool isDoorActive = true;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindWithTag("Player").transform;
        uiPrompt.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        
        float distance = Vector3.Distance(player.position, door.transform.position);
        if (distance <= doorDistance && isDoorActive) 
        { 
        
        isNearDoor = true;
        uiPrompt.enabled = true;

            if (Input.GetButtonDown(doorButton))
            {

                StartCoroutine(OpenDoor());
            }
        
        }

        else
        {
            isNearDoor = false;
            uiPrompt.enabled = false;
        }

    }

    IEnumerator OpenDoor()
    {
        isDoorActive = false;
        door.SetActive(false);
        uiPrompt.enabled = false;
        yield return new WaitForSeconds(doorAppearTime);
        door.SetActive(true);
        isDoorActive = true;
        uiPrompt.enabled = true;
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, doorDistance);
    }

}
