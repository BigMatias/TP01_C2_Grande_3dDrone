using TMPro;
using UnityEngine;

public class UIPlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedMeter;
    [SerializeField] private PlayerController playerController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speedMeter.text = playerController.CurrentSpeed().ToString("0");
    }
}
