using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Main.Scripts
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField]
        private float speed = 5;

        private void Update()
        {
            if (!IsOwner || !IsSpawned) return;

            var multiplier = speed * Time.deltaTime;

            if (Keyboard.current.aKey.isPressed)
            {
                transform.position += new Vector3(-multiplier, 0, 0);
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                transform.position += new Vector3(multiplier, 0, 0);
            }
        }
    }
}
