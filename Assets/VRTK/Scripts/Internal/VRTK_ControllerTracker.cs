namespace VRTK
{
    using UnityEngine;

    public struct ControllerTrackerEventArgs
    {
        public uint controllerIndex;
        public Collider collider;
    }

    public delegate void ControllerTrackerEventHandler(object sender, ControllerTrackerEventArgs e);

    public class VRTK_ControllerTracker : MonoBehaviour
    {
        public event ControllerTrackerEventHandler ControllerTrackerOnTriggerEnter;
        public event ControllerTrackerEventHandler ControllerTrackerOnTriggerStay;
        public event ControllerTrackerEventHandler ControllerTrackerOnTriggerExit;

        protected VRTK_TrackedController trackedController;

        public virtual uint GetControllerIndex()
        {
            return trackedController.index;
        }

        protected virtual void OnEnable()
        {
            var actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            if (actualController)
            {
                trackedController = actualController.GetComponent<VRTK_TrackedController>();
            }
        }

        protected virtual void Update()
        {
            if (trackedController && transform.parent != trackedController.transform)
            {
                var transformLocalScale = transform.localScale;
                transform.SetParent(trackedController.transform);
                transform.localPosition = Vector3.zero;
                transform.localScale = transformLocalScale;
                transform.localRotation = Quaternion.identity;
            }
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            OnControllerTrackerOnTriggerEnter(SetControllerTrackerEvent(collider));
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            OnControllerTrackerOnTriggerStay(SetControllerTrackerEvent(collider));
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            OnControllerTrackerOnTriggerExit(SetControllerTrackerEvent(collider));
        }

        protected virtual void OnControllerTrackerOnTriggerEnter(ControllerTrackerEventArgs e)
        {
            if (ControllerTrackerOnTriggerEnter != null)
            {
                ControllerTrackerOnTriggerEnter(this, e);
            }
        }

        protected virtual void OnControllerTrackerOnTriggerStay(ControllerTrackerEventArgs e)
        {
            if (ControllerTrackerOnTriggerStay != null)
            {
                ControllerTrackerOnTriggerStay(this, e);
            }
        }

        protected virtual void OnControllerTrackerOnTriggerExit(ControllerTrackerEventArgs e)
        {
            if (ControllerTrackerOnTriggerExit != null)
            {
                ControllerTrackerOnTriggerExit(this, e);
            }
        }

        protected virtual ControllerTrackerEventArgs SetControllerTrackerEvent(Collider collider)
        {
            ControllerTrackerEventArgs e;
            e.controllerIndex = GetControllerIndex();
            e.collider = collider;
            return e;
        }
    }
}