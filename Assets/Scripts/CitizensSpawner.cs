using System.Collections.Generic;
using UnityEngine;

public class CitizensSpawner : MonoBehaviour
{
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private CitizenDataSO citizenDataSO;
    [SerializeField] private GameObject spawnPointsHolder;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private GameObject civilian;
    [SerializeField] private Transform civilianParent;
    [SerializeField] private Transform bulletContainer;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    private Queue<GameObject> civilianPool = new Queue<GameObject>();
    private Transform[] spawnPoints;

    void Start()
    {
        spawnPoints = new Transform[spawnPointsHolder.transform.childCount];
        for (int i = 0; i < spawnPointsHolder.transform.childCount; i++)
        {
            spawnPoints[i] = spawnPointsHolder.transform.GetChild(i);
        }
        // Warning: [Baja] - Las tres colas ya se inicializan inline en la declaración (líneas 16-18).
        enemyPool = new Queue<GameObject>();
        civilianPool = new Queue<GameObject>();
        bulletPool = new Queue<GameObject>();

        InitializePool();
        InstantiateBullets();
    }

    // Suggestion: [Alta] - Clase monolítica: tres cases del switch son casi idénticos (60+ líneas duplicadas por nivel).
    // [Visto en clase]
    private void InitializePool()
    {
        switch (gameDataSO.CurrentLevel)
        {
            case 1:
                {
                    for (int i = 0; i < citizenDataSO.EnemySpawnQuantityLevel1; i++)
                    {
                        GameObject obj = Instantiate(enemy, enemiesParent);
                        obj.SetActive(false);
                        enemyPool.Enqueue(obj);

                        int index = Random.Range(0, spawnPoints.Length);
                        Transform spawnPoint = spawnPoints[index];

                        // Error: [Alta] - Anti-patrón: Enqueue inmediatamente seguido de Dequeue del MISMO objeto.
                        obj = enemyPool.Dequeue();

                        obj.transform.position = spawnPoint.position;

                        obj.SetActive(true);
                    }

                    for (int i = 0; i < citizenDataSO.CivilianSpawnQuantityLevel1; i++)
                    {
                        // Bug: [Media] - En el caso 1, civilian se Instancia SIN SetActive(false) inicial (a diferencia de Level2/3). Diferencia silenciosa entre niveles → comportamiento inconsistente.
                        GameObject obj = Instantiate(civilian, civilianParent);
                        civilianPool.Enqueue(obj);

                        int index = Random.Range(0, spawnPoints.Length);
                        Transform spawnPoint = spawnPoints[index];

                        obj = civilianPool.Dequeue();

                        obj.transform.position = spawnPoint.position;

                        obj.SetActive(true);
                    }
                    break;
                }
            case 2:
                {
                    for (int i = 0; i < citizenDataSO.EnemySpawnQuantityLevel2; i++)
                    {
                        GameObject obj = Instantiate(enemy, enemiesParent);
                        obj.SetActive(false);
                        enemyPool.Enqueue(obj);

                        int index = Random.Range(0, spawnPoints.Length);
                        Transform spawnPoint = spawnPoints[index];

                        obj = enemyPool.Dequeue();

                        obj.transform.position = spawnPoint.position;

                        obj.SetActive(true);
                    }

                    for (int i = 0; i < citizenDataSO.CivilianSpawnQuantityLevel2; i++)
                    {
                        GameObject obj = Instantiate(civilian, civilianParent);
                        obj.SetActive(false);
                        civilianPool.Enqueue(obj);

                        int index = Random.Range(0, spawnPoints.Length);
                        Transform spawnPoint = spawnPoints[index];

                        obj = civilianPool.Dequeue();

                        obj.transform.position = spawnPoint.position;

                        obj.SetActive(true);
                    }
                    break;
                }
            case 3:
                {
                    for (int i = 0; i < citizenDataSO.EnemySpawnQuantityLevel3; i++)
                    {
                        GameObject obj = Instantiate(enemy, enemiesParent);
                        obj.SetActive(false);
                        enemyPool.Enqueue(obj);

                        int index = Random.Range(0, spawnPoints.Length);
                        Transform spawnPoint = spawnPoints[index];

                        obj = enemyPool.Dequeue();

                        obj.transform.position = spawnPoint.position;

                        obj.SetActive(true);
                    }

                    for (int i = 0; i < citizenDataSO.CivilianSpawnQuantityLevel3; i++)
                    {
                        GameObject obj = Instantiate(civilian, civilianParent);
                        obj.SetActive(false);
                        civilianPool.Enqueue(obj);

                        int index = Random.Range(0, spawnPoints.Length);
                        Transform spawnPoint = spawnPoints[index];

                        obj = civilianPool.Dequeue();

                        obj.transform.position = spawnPoint.position;

                        obj.SetActive(true);
                    }
                    break;
                }
        }

    }

    private void InstantiateBullets()
    {
        for (int i = 0; i < citizenDataSO.BulletInstantiateQuantity; i++)
        {
            GameObject obj = Instantiate(bullet, bulletContainer);
            obj.SetActive(false);
            bulletPool.Enqueue(obj);
        }
    }

    public void ReturnEnemyToPool(GameObject obj)
    {
        enemyPool.Enqueue(obj);
        obj.SetActive(false);
    }

    public void ReturnCivilianToPool(GameObject obj)
    {
        civilianPool.Enqueue(obj);
        obj.SetActive(false);
    }

    public void ReturnBulletToPool(GameObject obj)
    {
        bulletPool.Enqueue(obj);
        obj.SetActive(false);
    }

    public GameObject GetBullet()
    {
        // Bug: [Crítica] - Sin chequeo de cola vacía. Si los enemigos disparan más rápido de lo que las balas vuelven.
        GameObject bullet = bulletPool.Dequeue();
        return bullet;
    }

}
