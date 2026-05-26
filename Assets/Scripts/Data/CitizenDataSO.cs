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
    // Warning: [Media] - Las cantidades son CONTEOS (no admiten decimales). Definirlas como float permite que el diseñador escriba "10.5" 
    public float BulletInstantiateQuantity;
    public float EnemySpawnQuantityLevel1;
    public float EnemySpawnQuantityLevel2;
    public float EnemySpawnQuantityLevel3;
    [Header("Civilian Settings")]
    // Warning: [Media] - Mismo problema: conteos de spawn deben ser int.
    public float CivilianSpawnQuantityLevel1;
    public float CivilianSpawnQuantityLevel2;
    public float CivilianSpawnQuantityLevel3;

}
