using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] Vector3 startingPoint;
    [SerializeField] Vector3 endingPoint;
    [SerializeField] float speed;
    [SerializeField] int timeUntilReturn;



    // Start is called before the first frame update
    void Start()
    {
            movePlatform();
    }

    // Update is called once per frame
    void Update()
    {
        movePlatform();
    }

    public void goUp()
    {
        this.transform.position += startingPoint * (speed*speed) * (Time.deltaTime);

    }

    public void goDown()
    {
        this.transform.position -= endingPoint * (speed * speed) * (Time.deltaTime);

    }

    IEnumerator movePlatform()
    {
        goUp();
        yield return new WaitForSeconds(timeUntilReturn);
        goDown();
    }
}
