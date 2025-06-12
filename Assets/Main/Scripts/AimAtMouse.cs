using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class AimAtMouse : NetworkBehaviour
    {
        [SerializeField] private Camera mainCamera;
        private void Awake()
        {
            if(!mainCamera) mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (!IsOwner || !IsSpawned) return;
            
            var mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
