
namespace UnityGameFramework
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
    }
}