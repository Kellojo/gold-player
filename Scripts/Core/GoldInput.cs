using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public abstract class GoldInput : MonoBehaviour
    {
        /// <summary>
        /// Returns true while the virtual button identified by buttonName is held down.
        /// </summary>
        public virtual bool GetButton(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns true during the frame the user pressed down the virtual button identified by buttonName.
        /// </summary>
        public virtual bool GetButtonDown(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns true the first frame the user releases the virtual button identified by buttonName.
        /// </summary>
        public virtual bool GetButtonUp(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName.
        /// </summary>
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [System.Obsolete("Only used with the Input Manager. Will do nothing with the new Input System.")]
#endif
        public virtual float GetAxis(string axisName)
        {
            return 0;
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
        /// </summary>
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [System.Obsolete("Only used with the Input Manager. Will do nothing with the new Input System.")]
#endif
        public virtual float GetAxisRaw(string axisName)
        {
            return 0;
        }

#if !ENABLE_INPUT_SYSTEM
        [System.Obsolete("Only used in the new Input System. Will do nothing with Input Manager.")]
#endif
        public virtual Vector2 GetVector2(string actionName)
        {
            return Vector2.zero;
        }
    }
}
