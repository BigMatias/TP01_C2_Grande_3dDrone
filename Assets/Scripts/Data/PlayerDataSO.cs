using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Data")]

public class PlayerDataSO : ScriptableObject
{
    [Header("Controls")]
    public KeyCode GoUp;
    public KeyCode GoDown;
    public KeyCode RotateLeft;
    public KeyCode RotateRight;
    [Header("General Configs")]
    public float Speed;
    public float MouseSens;
    public float Acceleration;
    public float MaxSpeed;
    [Header("Damage Depending on Speed")]
    public float FirstSpeedThreshold;
    public float SecondSpeedThreshold;
    public float ThirdSpeedThreshold;
    public float CrashDamage1;
    public float CrashDamage2;
    public float CrashDamage3;

}
