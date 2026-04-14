using UnityEngine;

[CreateAssetMenu(fileName = "CitizenSettings", menuName = "Citizen/Data")]

public class CitizenDataSO : ScriptableObject
{
    [Header("General Configs")]
    public float Speed;
    public float MaxSpeed;
    [Header("Enemy Settings")]
    public float EnemyBulletDamage;
    public float EnemyBulletSpeed;
    public float EnemyBulletLifeTime;
    public float EnemyShootCD;
    public float BulletInstantiateQuantity;
    public float EnemySpawnQuantityLevel1;
    public float EnemySpawnQuantityLevel2;
    public float EnemySpawnQuantityLevel3;
    [Header("Civilian Settings")]
    public float CivilianSpawnQuantityLevel1;
    public float CivilianSpawnQuantityLevel2;
    public float CivilianSpawnQuantityLevel3;

}
