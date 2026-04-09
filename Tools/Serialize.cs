// Copyright (c) 2024 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Threading;

namespace CodaGame
{
    /// <summary>
    /// A class that generates a serialize number for you
    /// </summary>
    public static class Serialize
    {
        // All counters start at -1 so that Interlocked.Increment returns 0 on the first call,
        // preserving the original `field++` (post-increment) semantics.
        private static int _g_serialize = -1;
        private static int _g_asyncParallelSerialize = -1;
        private static int _g_asyncWaterfallSerialize = -1;
        private static int _g_stateMachineSerialize = -1;
        private static int _g_safeAsyncObjectPoolSerialize = -1;
        private static int _g_unsafeAsyncObjectPoolSerialize = -1;
        private static int _g_safeSyncObjectPoolSerialize = -1;
        private static int _g_unsafeSyncObjectPoolSerialize = -1;
        private static int _g_objectHandlePoolSerialize = -1;
        private static int _g_actionTaskSerialize = -1;
        private static int _g_continuousTaskSerialize = -1;
        private static int _g_delayActionTaskSerialize = -1;
        private static int _g_timeIntervalContinuousTaskSerialize = -1;
        private static int _g_frameIntervalContinuousTaskSerialize = -1;
        private static int _g_frameDelayActionTaskSerialize = -1;
        private static int _g_frameActionTaskSerialize = -1;
        private static int _g_limitedValueRecoverTaskSerialize = -1;
        private static int _g_cameraValueBehaviourSerialize = -1;
        private static int _g_cameraValueOffsetSerialize = -1;
        private static int _g_cameraValueConstraintSerialize = -1;
        private static int _g_eventKeySerialize = -1;


        /// <summary>
        /// Get the next serialize number
        /// </summary>
        public static uint Next()
        {
            return (uint)Interlocked.Increment(ref _g_serialize);
        }


        internal static uint NextAsyncParallel()
        {
            return (uint)Interlocked.Increment(ref _g_asyncParallelSerialize);
        }
        internal static uint NextAsyncWaterfall()
        {
            return (uint)Interlocked.Increment(ref _g_asyncWaterfallSerialize);
        }
        internal static uint NextStateMachine()
        {
            return (uint)Interlocked.Increment(ref _g_stateMachineSerialize);
        }
        internal static uint NextSafeAsyncObjectPool()
        {
            return (uint)Interlocked.Increment(ref _g_safeAsyncObjectPoolSerialize);
        }
        internal static uint NextUnsafeAsyncObjectPool()
        {
            return (uint)Interlocked.Increment(ref _g_unsafeAsyncObjectPoolSerialize);
        }
        internal static uint NextSafeSyncObjectPool()
        {
            return (uint)Interlocked.Increment(ref _g_safeSyncObjectPoolSerialize);
        }
        internal static uint NextUnsafeSyncObjectPool()
        {
            return (uint)Interlocked.Increment(ref _g_unsafeSyncObjectPoolSerialize);
        }
        internal static uint NextObjectHandlePool()
        {
            return (uint)Interlocked.Increment(ref _g_objectHandlePoolSerialize);
        }
        internal static uint NextActionTask()
        {
            return (uint)Interlocked.Increment(ref _g_actionTaskSerialize);
        }
        internal static uint NextContinuousTask()
        {
            return (uint)Interlocked.Increment(ref _g_continuousTaskSerialize);
        }
        internal static uint NextDelayActionTask()
        {
            return (uint)Interlocked.Increment(ref _g_delayActionTaskSerialize);
        }
        internal static uint NextTimeIntervalContinuousTask()
        {
            return (uint)Interlocked.Increment(ref _g_timeIntervalContinuousTaskSerialize);
        }
        internal static uint NextFrameIntervalContinuousTask()
        {
            return (uint)Interlocked.Increment(ref _g_frameIntervalContinuousTaskSerialize);
        }
        internal static uint NextFrameDelayActionTask()
        {
            return (uint)Interlocked.Increment(ref _g_frameDelayActionTaskSerialize);
        }
        internal static uint NextNextFrameActionTask()
        {
            return (uint)Interlocked.Increment(ref _g_frameActionTaskSerialize);
        }
        internal static uint NextNextLimitedValueRecoverTask()
        {
            return (uint)Interlocked.Increment(ref _g_limitedValueRecoverTaskSerialize);
        }
        internal static uint NextCameraValueBehaviour()
        {
            return (uint)Interlocked.Increment(ref _g_cameraValueBehaviourSerialize);
        }
        internal static uint NextCameraValueOffset()
        {
            return (uint)Interlocked.Increment(ref _g_cameraValueOffsetSerialize);
        }
        internal static uint NextCameraValueConstraint()
        {
            return (uint)Interlocked.Increment(ref _g_cameraValueConstraintSerialize);
        }
        internal static uint NextEventKey()
        {
            return (uint)Interlocked.Increment(ref _g_eventKeySerialize);
        }
    }
}
