using Plugins.MonoCache;
using UnityEngine;

namespace PlayerLogic
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(ShooterController))]
    public class Player : MonoCache
    {
    }
}