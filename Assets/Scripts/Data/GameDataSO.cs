using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Data")]

public class GameDataSO : ScriptableObject
{
    [Header("Levels")]
    public float CurrentScore;
    public float TotalScore;
    public float PointsOnKill;
    public float PointsReducedOnKill;
    public float CurrentLevel;
    public float EnemiesLeft;
    public float EnemiesNeededToKill;

}
