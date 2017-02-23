// Teleport Disable On Headset Collision|Locomotion|20040
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The purpose of the Teleport Disable On Headset Collision script is to detect when the headset is colliding with a valid object and prevent teleportation from working. This is to ensure that if a user is clipping their head into a wall then they cannot teleport to an area beyond the wall.
    /// </summary>
    public class VRTK_TeleportDisableOnHeadsetCollision : MonoBehaviour
    {
        [Header("Custom Settings")]

        [Tooltip("The Teleporter to utilise. If the script is being applied on to the same GameObject as the teleport script then this parameter can be left blank as it will be auto populated at runtime.")]
        public VRTK_BasicTeleport teleporter;

        private VRTK_HeadsetCollision headsetCollision;
        private Coroutine enableScript;

        protected virtual void OnEnable()
        {
            teleporter = (teleporter ?? GetComponent<VRTK_BasicTeleport>());
            enableScript = StartCoroutine(EnableAtEndOfFrame());
        }

        protected virtual void OnDisable()
        {
            if (teleporter == null)
            {
                return;
            }

            if (headsetCollision)
            {
                headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(EnableTeleport);
            }

            if (enableScript != null)
            {
                StopCoroutine(enableScript);
            }
            teleporter = null;
        }

        private IEnumerator EnableAtEndOfFrame()
        {
            if (teleporter == null)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();

            headsetCollision = VRTK_ObjectCache.registeredHeadsetCollider;
            if (headsetCollision)
            {
                headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        private void DisableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(false);
        }

        private void EnableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(true);
        }
    }
}