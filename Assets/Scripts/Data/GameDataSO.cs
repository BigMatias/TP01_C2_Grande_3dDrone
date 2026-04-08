using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Data")]

public class GameDataSO : ScriptableObject
{
    [Header("Levels")]
    public float CurrentScore;
    public float ScoreLevel1;
    public float ScoreLevel2;
    public float ScoreLevel3;
    public float CurrentLevel;

}
