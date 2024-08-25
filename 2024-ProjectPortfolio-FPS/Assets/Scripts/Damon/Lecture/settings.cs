using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour
{

    [SerializeField] Slider sensSlider;
    [SerializeField] cameraController cameraController_Damon;



    // Start is called before the first frame update
    void Start()
    {

        sensSlider.value = cameraController_Damon.sens;

        sensSlider.onValueChanged.AddListener(cameraController_Damon.setSens);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
