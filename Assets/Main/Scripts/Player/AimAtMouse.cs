using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Player
{
    public class AimAtMouse : NetworkBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float rotationSmoothSpeed = 10f; // Adjust as needed
        private void Awake()
        {
            if(!mainCamera) mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (!IsOwner || !IsSpawned) return;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - transform.position);
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
    
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }
}
