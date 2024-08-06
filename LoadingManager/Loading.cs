
using System;
using UnityGameFramework.Base;

namespace UnityGameFramework
{
    /// <summary>
    /// The class for loading with a loading show.
    /// </summary>
    public static class Loading
    {
        /// <summary>
        /// Load with a loading show.
        /// </summary>
        /// <remarks>
        /// <para>The loading show will be shown before the load function starts running, and will be hidden after the load function completes, then the '_loadingShowEndCallback' will be called.</para>
        /// <para>For the same loading show, the load function will be added to the same loading process.</para>
        /// </remarks>
        /// <param name="_loadingShow">The loading show interface.</param>
        /// <param name="_loadFunction">The asynchronous load function.</param>
        /// <param name="_loadingShowEndCallback">The callback to be invoked when the loading show ends.</param>
        public static void Load(_ILoadingShow _loadingShow, AsyncFunction _loadFunction, Action _loadingShowEndCallback = null)
        {
            LoadingManager.instance.Load(_loadingShow, _loadFunction, _loadingShowEndCallback);
        }
    }
}