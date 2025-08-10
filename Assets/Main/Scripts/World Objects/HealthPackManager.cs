using UnityEngine;

namespace Main.Scripts.World_Objects
{
    public class HealthPackManager : MonoBehaviour
    {
        public int healthValue = 50;

        [SerializeField]
        private Transform healthPackTransform;
        
        private bool _goingUp = true;
        private float _initialHeight;
        private const float MaxHeight = .1f;
        private const float HoverSpeed = .1f;

        private void Start()
        {
            _initialHeight = healthPackTransform.localPosition.y;
        }
        private void Update()
        {
            if (_goingUp)
            {
                Vector3 newPosition = healthPackTransform.localPosition + new Vector3(0, Time.deltaTime * HoverSpeed, 0);
                healthPackTransform.position = newPosition;
                if (healthPackTransform.localPosition.y > _initialHeight + MaxHeight)
                {
                    _goingUp = false;
                }
            }
            else
            {
                Vector3 newPosition = healthPackTransform.localPosition + new Vector3(0, Time.deltaTime * -HoverSpeed, 0);
                healthPackTransform.position = newPosition;
                if (healthPackTransform.localPosition.y < _initialHeight)
                {
                    _goingUp = true;
                }
            }
        }
    }
}
