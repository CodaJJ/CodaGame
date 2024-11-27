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
        private static uint _g_safeAsyncObjectGetterSerialize = 0;
        private static uint _g_unsafeAsyncObjectGetterSerialize = 0;
        private static uint _g_safeSyncObjectGetterSerialize = 0;
        private static uint _g_unsafeSyncObjectGetterSerialize = 0;
        private static uint _g_objectHandleGetterSerialize = 0;
        private static uint _g_actionTaskSerialize = 0;
        private static uint _g_continuousTaskSerialize = 0;
        private static uint _g_delayActionTaskSerialize = 0;
        private static uint _g_timeIntervalContinuousTaskSerialize = 0;
        private static uint _g_frameIntervalContinuousTaskSerialize = 0;
        private static uint _g_frameDelayActionTaskSerialize = 0;
        private static uint _g_nextFrameActionTaskSerialize = 0;
        
        
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
        internal static uint NextSafeAsyncObjectGetter()
        {
            return _g_safeAsyncObjectGetterSerialize++;
        }
        internal static uint NextUnsafeAsyncObjectGetter()
        {
            return _g_unsafeAsyncObjectGetterSerialize++;
        }
        internal static uint NextSafeSyncObjectGetter()
        {
            return _g_safeSyncObjectGetterSerialize++;
        }
        internal static uint NextUnsafeSyncObjectGetter()
        {
            return _g_unsafeSyncObjectGetterSerialize++;
        }
        internal static uint NextObjectHandleGetter()
        {
            return _g_objectHandleGetterSerialize++;
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
            return _g_nextFrameActionTaskSerialize++;
        }
    }
}