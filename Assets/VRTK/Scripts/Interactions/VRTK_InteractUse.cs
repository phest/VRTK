// Interact Use|Interactions|30060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Use script is attached to a Controller object and requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for using and stop using interactable game objects.
    /// </summary>
    /// <remarks>
    /// It listens for the `AliasUseOn` and `AliasUseOff` events to determine when an object should be used and should stop using.
    ///
    /// The Controller object also requires the `VRTK_InteractTouch` script to be attached to it as this is used to determine when an interactable object is being touched. Only valid touched objects can be used.
    ///
    /// An object can be used if the Controller touches a game object which contains the `VRTK_InteractableObject` script and has the flag `isUsable` set to `true`.
    ///
    /// If a valid interactable object is usable then pressing the set `Use` button on the Controller (default is `Trigger`) will call the `StartUsing` method on the touched interactable object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/006_Controller_UsingADoor` simulates using a door object to open and close it. It also has a cube on the floor that can be grabbed to show how interactable objects can be usable or grabbable.
    ///
    /// `VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that objects can be grabbed with one button and used with another (e.g. firing a gun).
    /// </example>
    public class VRTK_InteractUse : MonoBehaviour
    {
        [Header("Custom Settings")]

        [Tooltip("The controller to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controllerEvents;
        [Tooltip("The controller to perform actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerActions controllerActions;
        [Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTouch;
        [Tooltip("An optional Interact Grab to listen for grabs on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractGrab interactGrab;

        private GameObject usingObject = null;

        /// <summary>
        /// Emitted when a valid object starts being used.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUseInteractableObject;
        /// <summary>
        /// Emitted when a valid object stops being used.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

        public virtual void OnControllerUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUseInteractableObject != null)
            {
                ControllerUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUnuseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUnuseInteractableObject != null)
            {
                ControllerUnuseInteractableObject(this, e);
            }
        }

        /// <summary>
        /// The GetUsingObject method returns the current object being used by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being used by this controller.</returns>
        public GameObject GetUsingObject()
        {
            return usingObject;
        }

        /// <summary>
        /// The ForceStopUsing method will force the controller to stop using the currently touched object and will also stop the object's using action.
        /// </summary>
        public void ForceStopUsing()
        {
            if (usingObject != null)
            {
                StopUsing();
            }
        }

        /// <summary>
        /// The ForceResetUsing will force the controller to stop using the currently touched object but the object will continue with it's existing using action.
        /// </summary>
        public void ForceResetUsing()
        {
            if (usingObject != null)
            {
                UnuseInteractedObject(false);
            }
        }

        protected virtual void OnEnable()
        {
            controllerEvents = (controllerEvents ?? GetComponent<VRTK_ControllerEvents>());
            if (!controllerEvents)
            {
                Debug.LogError(VRTK_SharedMethods.GetCommonString("REQUIRED_SCRIPT_MISSING_FROM_GAMEOBJECT", new string[] { "VRTK_ControllerEvents", "VRTK_InteractGrab", "controllerEvents" }));
                return;
            }

            controllerActions = (controllerActions ?? GetComponent<VRTK_ControllerActions>());
            if (!controllerActions)
            {
                Debug.LogError(VRTK_SharedMethods.GetCommonString("REQUIRED_SCRIPT_MISSING_FROM_GAMEOBJECT", new string[] { "VRTK_ControllerActions", "VRTK_InteractGrab", "controllerActions" }));
                return;
            }

            interactTouch = (interactTouch ?? GetComponent<VRTK_InteractTouch>());
            if (!interactTouch)
            {
                Debug.LogError(VRTK_SharedMethods.GetCommonString("REQUIRED_SCRIPT_MISSING_FROM_GAMEOBJECT", new string[] { "VRTK_InteractTouch", "VRTK_InteractGrab", "interactTouch" }));
                return;
            }

            interactGrab = (interactGrab ?? GetComponent<VRTK_InteractGrab>());

            controllerEvents.AliasUseOn += new ControllerInteractionEventHandler(DoStartUseObject);
            controllerEvents.AliasUseOff += new ControllerInteractionEventHandler(DoStopUseObject);
        }

        protected virtual void OnDisable()
        {
            ForceStopUsing();
            controllerEvents.AliasUseOn -= new ControllerInteractionEventHandler(DoStartUseObject);
            controllerEvents.AliasUseOff -= new ControllerInteractionEventHandler(DoStopUseObject);

            controllerEvents = null;
            controllerActions = null;
            interactTouch = null;
            interactGrab = null;
        }

        private bool IsObjectUsable(GameObject obj)
        {
            return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<VRTK_InteractableObject>().isUsable);
        }

        private bool IsObjectHoldOnUse(GameObject obj)
        {
            if (obj)
            {
                var objScript = obj.GetComponent<VRTK_InteractableObject>();
                return (objScript && objScript.holdButtonToUse);
            }
            return false;
        }

        private int GetObjectUsingState(GameObject obj)
        {
            if (obj)
            {
                var objScript = obj.GetComponent<VRTK_InteractableObject>();
                if (objScript)
                {
                    return objScript.usingState;
                }
            }
            return 0;
        }

        private void SetObjectUsingState(GameObject obj, int value)
        {
            if (obj)
            {
                var objScript = obj.GetComponent<VRTK_InteractableObject>();
                if (objScript)
                {
                    objScript.usingState = value;
                }
            }
        }

		private void AttemptHaptics()
		{
			if (usingObject) 
			{ 
				var doHaptics = usingObject.GetComponentInParent<VRTK_InteractHaptics>();
				if (doHaptics)
				{
                    doHaptics.HapticsOnTouch(VRTK_DeviceFinder.GetControllerIndex(controllerEvents.GetTrackedHand()));
                }
			}
        }

        private void ToggleControllerVisibility(bool visible)
        {
            if (usingObject)
            {
                var controllerAppearanceScript = usingObject.GetComponentInParent<VRTK_InteractControllerAppearance>();
                if (controllerAppearanceScript)
                {
                    controllerAppearanceScript.ToggleControllerOnUse(visible, controllerActions, usingObject);
                }
            }
        }

        private void UseInteractedObject(GameObject touchedObject)
        {
            if ((usingObject == null || usingObject != touchedObject) && IsObjectUsable(touchedObject))
            {
                usingObject = touchedObject;
                var usingObjectScript = usingObject.GetComponent<VRTK_InteractableObject>();

                if (!usingObjectScript.IsValidInteractableController(controllerEvents.gameObject, usingObjectScript.allowedUseControllers))
                {
                    usingObject = null;
                    return;
                }

                usingObjectScript.StartUsing(controllerEvents.gameObject);
                ToggleControllerVisibility(false);
                AttemptHaptics();
                OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
            }
        }

        private void UnuseInteractedObject(bool completeStop)
        {
            if (usingObject != null)
            {
                var usingObjectCheck = usingObject.GetComponent<VRTK_InteractableObject>();
                if (usingObjectCheck && completeStop)
                {
                    usingObjectCheck.StopUsing(controllerEvents.gameObject);
                }
                ToggleControllerVisibility(true);
                OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                usingObject = null;
            }
        }

        private GameObject GetFromGrab()
        {
            if (interactGrab)
            {
                return interactGrab.GetGrabbedObject();
            }
            return null;
        }

        private void StopUsing()
        {
            SetObjectUsingState(usingObject, 0);
            UnuseInteractedObject(true);
        }

        private void DoStartUseObject(object sender, ControllerInteractionEventArgs e)
        {
            GameObject touchedObject = interactTouch.GetTouchedObject();
            if (touchedObject == null)
            {
                touchedObject = GetFromGrab();
            }

            if (touchedObject != null && interactTouch.IsObjectInteractable(touchedObject))
            {
                var interactableObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                if (interactableObjectScript.useOnlyIfGrabbed && !interactableObjectScript.IsGrabbed())
                {
                    return;
                }

                UseInteractedObject(touchedObject);
                if (usingObject && !IsObjectHoldOnUse(usingObject))
                {
                    SetObjectUsingState(usingObject, GetObjectUsingState(usingObject) + 1);
                }
            }
        }

        private void DoStopUseObject(object sender, ControllerInteractionEventArgs e)
        {
            if (IsObjectHoldOnUse(usingObject) || GetObjectUsingState(usingObject) >= 2)
            {
                StopUsing();
            }
        }
    }
}