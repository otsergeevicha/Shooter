using UnityEngine;

public static class Constants
{
    public const int RateCurtain = 3;
    public const float RateAlfaCurtain = .03f;
    
    public const int MillisecondsDelay = 1000;
    
    public const int MaxTriggerRadius = 25;
    public const int MinTriggerRadius = 0;
    
    public const string InitialScene = "Initial";
    public const string OpenCity = "OpenCity";
    
    public const string LayerPlayer = "Player";

    public static readonly int Kick = Animator.StringToHash("Kick");
    public static readonly int FistBump = Animator.StringToHash("Boxing");
    public static readonly int GrenadeCast = Animator.StringToHash("GrenadeCast");
    public static readonly int Shots = Animator.StringToHash("Shots");
    public static readonly int EnterDoor = Animator.StringToHash("EnterDoor");
    public static readonly int ExitDoor = Animator.StringToHash("ExitDoor");
    
    public static readonly int BumpPlayer = Animator.StringToHash("BumpPlayer");
    public static readonly int Walk = Animator.StringToHash("Walk");
    public static readonly int Run = Animator.StringToHash("Run");
}