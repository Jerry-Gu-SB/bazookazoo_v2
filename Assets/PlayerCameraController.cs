using Unity.Netcode;
using Unity.Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    public CinemachineCamera virtualCamera;

    private void Start()
    {
        if (IsOwner)
        {
            virtualCamera.Priority = 10;
            virtualCamera.gameObject.SetActive(true);
        }
        else
        {
            virtualCamera.Priority = 0;
            virtualCamera.gameObject.SetActive(false);
        }
    }
}