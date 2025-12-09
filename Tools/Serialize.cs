// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// A class that generates a serialize number for you
    /// </summary>
    public static class Serialize
    {
        private static uint _g_serialize = 0;
        private static uint _g_asyncParallelSerialize = 0;
        private static uint _g_asyncWaterfallSerialize = 0;
        private static uint _g_stateMachineSerialize = 0;
        private static uint _g_safeAsyncObjectPoolSerialize = 0;
        private static uint _g_unsafeAsyncObjectPoolSerialize = 0;
        private static uint _g_safeSyncObjectPoolSerialize = 0;
        private static uint _g_unsafeSyncObjectPoolSerialize = 0;
        private static uint _g_objectHandlePoolSerialize = 0;
        private static uint _g_actionTaskSerialize = 0;
        private static uint _g_continuousTaskSerialize = 0;
        private static uint _g_delayActionTaskSerialize = 0;
        private static uint _g_timeIntervalContinuousTaskSerialize = 0;
        private static uint _g_frameIntervalContinuousTaskSerialize = 0;
        private static uint _g_frameDelayActionTaskSerialize = 0;
        private static uint _g_frameActionTaskSerialize = 0;
        private static uint _g_limitedValueRecoverTaskSerialize = 0;
        private static uint _g_cameraValueBehaviourSerialize = 0;
        private static uint _g_cameraValueOffsetSerialize = 0;
        private static uint _g_cameraValueConstraintSerialize = 0;


        /// <summary>
        /// Get the next serialize number
        /// </summary>
        public static uint Next()
        {
            return _g_serialize++;
        }
        
        
        internal static uint NextAsyncParallel()
        {
            return _g_asyncParallelSerialize++;
        }
        internal static uint NextAsyncWaterfall()
        {
            return _g_asyncWaterfallSerialize++;
        }
        internal static uint NextStateMachine()
        {
            return _g_stateMachineSerialize++;
        }
        internal static uint NextSafeAsyncObjectPool()
        {
            return _g_safeAsyncObjectPoolSerialize++;
        }
        internal static uint NextUnsafeAsyncObjectPool()
        {
            return _g_unsafeAsyncObjectPoolSerialize++;
        }
        internal static uint NextSafeSyncObjectPool()
        {
            return _g_safeSyncObjectPoolSerialize++;
        }
        internal static uint NextUnsafeSyncObjectPool()
        {
            return _g_unsafeSyncObjectPoolSerialize++;
        }
        internal static uint NextObjectHandlePool()
        {
            return _g_objectHandlePoolSerialize++;
        }
        internal static uint NextActionTask()
        {
            return _g_actionTaskSerialize++;
        }
        internal static uint NextContinuousTask()
        {
            return _g_continuousTaskSerialize++;
        }
        internal static uint NextDelayActionTask()
        {
            return _g_delayActionTaskSerialize++;
        }
        internal static uint NextTimeIntervalContinuousTask()
        {
            return _g_timeIntervalContinuousTaskSerialize++;
        }
        internal static uint NextFrameIntervalContinuousTask()
        {
            return _g_frameIntervalContinuousTaskSerialize++;
        }
        internal static uint NextFrameDelayActionTask()
        {
            return _g_frameDelayActionTaskSerialize++;
        }
        internal static uint NextNextFrameActionTask()
        {
            return _g_frameActionTaskSerialize++;
        }
        internal static uint NextNextLimitedValueRecoverTask()
        {
            return _g_limitedValueRecoverTaskSerialize++;
        }
        internal static uint NextCameraValueBehaviour()
        {
            return _g_cameraValueBehaviourSerialize++;
        }
        internal static uint NextCameraValueOffset()
        {
            return _g_cameraValueOffsetSerialize++;
        }
        internal static uint NextCameraValueConstraint()
        {
            return _g_cameraValueConstraintSerialize++;
        }
    }
}