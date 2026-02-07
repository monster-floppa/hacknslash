using UnityEngine;
using HacknSlash.Player;

namespace HacknSlash.CameraSystem
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private PlayerCombat playerCombat;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float lockOnZoom = -4.5f;

        private float yaw;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            Quaternion rot = Quaternion.Euler(0f, yaw, 0f);

            Vector3 desiredOffset = offset;
            if (playerCombat != null && playerCombat.LockOnEnabled)
            {
                desiredOffset.z = Mathf.Lerp(desiredOffset.z, lockOnZoom, Time.deltaTime * 4f);
            }

            Vector3 desiredPos = target.position + rot * desiredOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);

            if (playerCombat != null && playerCombat.LockOnEnabled && playerCombat.CurrentTarget != null)
            {
                Vector3 mid = (target.position + playerCombat.CurrentTarget.position) * 0.5f;
                transform.LookAt(mid + Vector3.up * 1.5f);
            }
            else
            {
                transform.LookAt(target.position + Vector3.up * 1.5f);
            }
        }
    }
}
