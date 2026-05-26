using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Data")]

public class GameDataSO : ScriptableObject
{
    [Header("Levels")]
    public float CurrentScore;
    public float TotalScore;
    public float PointsOnKill;
    public float PointsReducedOnKill;
    // Warning: [Alta] - CurrentLevel, EnemiesLeft y EnemiesNeededToKill son CONTEOS / IDs enteros usados en switch/case y con += 1. Tenelos como int.
    public float CurrentLevel;
    public float EnemiesLeft;
    public float EnemiesNeededToKill;

}
