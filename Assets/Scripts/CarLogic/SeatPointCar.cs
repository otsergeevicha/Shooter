using PlayerLogic;
using UnityEngine;

namespace CarLogic
{
    public class SeatPointCar : Car
    {
        [SerializeField] private DoorDriver _door;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent(out Player player)) 
                player.Get<Player>().InitSeatCar(transform, _door);
        }
    }
}