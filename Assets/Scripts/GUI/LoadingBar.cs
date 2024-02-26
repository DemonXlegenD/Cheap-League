using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{

    [Header("Player")]
    [SerializeField]  private GameObject player;

    private FPSController fpsController;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    // Start is called before the first frame update
    void Start()
    {
        fpsController = player.GetComponent<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        float progressValue = Mathf.Clamp01(fpsController.Stamina / fpsController.MaxStamina);
        loadingSlider.value = progressValue;
            
    }
}
