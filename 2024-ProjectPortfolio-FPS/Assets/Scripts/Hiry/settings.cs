using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour
{
    [SerializeField] cameraController cameraController_Damon;
    [SerializeField] playerController playerController_Damon;

    [SerializeField] Slider sensSlider;
    [SerializeField] Toggle sprintToggle;



    // Start is called before the first frame update
    void Start()
    {


        sensSlider.value = cameraController_Damon.sens;
        sensSlider.onValueChanged.AddListener(cameraController_Damon.setSens);

        sprintToggle.isOn = playerController_Damon.sprintToggle;
        sprintToggle.onValueChanged.AddListener(playerController_Damon.SetSprintToggle);

    }

    
   
}
