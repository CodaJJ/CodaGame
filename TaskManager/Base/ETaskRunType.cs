
namespace UnityGameFramework.Base
{
    /// <summary>
    /// Enum for choose type you want run task.
    /// </summary>
    public enum ETaskRunType
    {
        /// <summary>
        /// Will run in Unity's Update, and use Time.deltaTime.
        /// </summary>
        Update,
        /// <summary>
        /// Will run in Unity's FixedUpdate, and use Time.fixedDeltaTime.
        /// </summary>
        FixedUpdate,
        /// <summary>
        /// Will run in Unity's FixedUpdate, and use Time.fixedUnscaledDeltaTime.
        /// </summary>
        UnscaledFixedUpdate,
        /// <summary>
        /// Will run in Unity's LateUpdate, and use Time.deltaTime.
        /// </summary>
        LateUpdate,
        /// <summary>
        /// Will run in Unity's Update, and use Time.unscaledDeltaTime.
        /// </summary>
        UnscaledTimeUpdate,
        /// <summary>
        /// Will run in Unity's LateUpdate, and use Time.unscaledDeltaTime.
        /// </summary>
        UnscaledTimeLateUpdate,
    }
}