using UnityEngine;
using UnityEngine.UI;

public class UiLife : MonoBehaviour
{
    [SerializeField] private HealthSystem target;
    [SerializeField] private Image barLife;

    private void Awake()
    {
        target.onLifeUpdated += HealthSystem_onLifeUpdated;
        target.onDie += HealthSystem_onDie;
    }

    private void Start()
    {
        // Bug: [Alta] - Image.fillAmount está en rango [0,1]. Asignar 100 lo clampa a 1 internamente, pero indica desconocimiento.
        barLife.fillAmount = 100;
    }

    private void OnDestroy()
    {
        target.onLifeUpdated -= HealthSystem_onLifeUpdated;
        target.onDie -= HealthSystem_onDie;
    }

    public void HealthSystem_onLifeUpdated(float current, float max)
    {
        float lerp = current / (float)max;
        barLife.fillAmount = lerp;
    }

    private void HealthSystem_onDie()
    {
        barLife.fillAmount = 0;
    }
}