using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIPlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedMeter;
    [SerializeField] private TextMeshProUGUI pointsTxt;
    [SerializeField] private PlayerController playerController;
    int totalPoints = 0;

    private void Awake()
    {
        FsmManager.onCivilianDied += FsmManager_onCivilianDied;
        FsmManager.onEnemyDied += FsmManager_onEnemyDied; ;
    }

    void Start()
    {
        pointsTxt.text = totalPoints.ToString();
    }

    void Update()
    {
        speedMeter.text = playerController.CurrentSpeed().ToString("0");
    }

    private void FsmManager_onEnemyDied()
    {
        totalPoints += 10;
        pointsTxt.text = totalPoints.ToString();
    }

    private void FsmManager_onCivilianDied()
    {
        totalPoints -= 10;
        pointsTxt.text = totalPoints.ToString();
    }
}
