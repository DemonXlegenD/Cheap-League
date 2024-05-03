using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{

    [Header("Player")]
    [SerializeField] private GameObject player;

    private CarController CarController;

    void Start()
    {
        CarController = player.GetComponent<CarController>();
    }

    void Update()
    {
        float progressValue = Mathf.Clamp01(CarController.boostAmount / 100);
        GetComponent<Slider>().value = progressValue;
    }
}
