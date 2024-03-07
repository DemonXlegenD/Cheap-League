using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{

    [Header("Player")]
    [SerializeField] private GameObject player;

    private CarController CarController;

    // Start is called before the first frame update
    void Start()
    {
        CarController = player.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        float progressValue = Mathf.Clamp01(CarController.boostAmount / 100);
        GetComponent<Slider>().value = progressValue;
    }
}
