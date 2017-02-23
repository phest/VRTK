// Teleport Disable On Controller Obscured|Locomotion|20050
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The purpose of the Teleport Disable On Controller Obscured script is to detect when the headset does not have a line of sight to the controllers and prevent teleportation from working. This is to ensure that if a user is clipping their controllers through a wall then they cannot teleport to an area beyond the wall.
    /// </summary>
    [RequireComponent(typeof(VRTK_HeadsetControllerAware))]
    public class VRTK_TeleportDisableOnControllerObscured : MonoBehaviour
    {
        [Header("Custom Settings")]

        [Tooltip("The Teleporter to utilise. If the script is being applied on to the same GameObject as the teleport script then this parameter can be left blank as it will be auto populated at runtime.")]
        public VRTK_BasicTeleport teleporter;
        private Coroutine enableScript;

        private VRTK_HeadsetControllerAware headset;

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

            if (headset)
            {
                headset.ControllerObscured -= new HeadsetControllerAwareEventHandler(DisableTeleport);
                headset.ControllerUnobscured -= new HeadsetControllerAwareEventHandler(EnableTeleport);
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

            headset = VRTK_ObjectCache.registeredHeadsetControllerAwareness;
            if (headset)
            {
                headset.ControllerObscured += new HeadsetControllerAwareEventHandler(DisableTeleport);
                headset.ControllerUnobscured += new HeadsetControllerAwareEventHandler(EnableTeleport);
            }
        }

        private void DisableTeleport(object sender, HeadsetControllerAwareEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(false);
        }

        private void EnableTeleport(object sender, HeadsetControllerAwareEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(true);
        }
    }
}