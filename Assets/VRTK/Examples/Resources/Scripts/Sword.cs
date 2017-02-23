namespace VRTK.Examples
{
    using UnityEngine;

    public class Sword : VRTK_InteractableObject
    {
        private VRTK_ControllerEvents controllerEvents;
        private float impactMagnifier = 120f;
        private float collisionForce = 0f;
        private float maxCollisionForce = 4000f;

        public float CollisionForce()
        {
            return collisionForce;
        }

        public override void Grabbed(GameObject grabbingObject)
        {
            base.Grabbed(grabbingObject);
            controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();
        }

        protected override void Awake()
        {
            base.Awake();
            interactableRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (controllerEvents && IsGrabbed())
            {
                collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerEvents.GetTrackedHand()).magnitude * impactMagnifier;
                var hapticStrength = collisionForce / maxCollisionForce;
                VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(controllerEvents.GetTrackedHand()), hapticStrength, 0.5f, 0.01f);
            }
            else
            {
                collisionForce = collision.relativeVelocity.magnitude * impactMagnifier;
            }
        }
    }
}