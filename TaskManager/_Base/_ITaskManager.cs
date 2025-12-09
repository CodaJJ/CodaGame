// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    internal interface _ITaskManager
    {
        void AddUpdateContinuousTask(_AContinuousTask _task);
        void AddUnscaledTimeUpdateContinuousTask(_AContinuousTask _task);
        void AddFixedUpdateContinuousTask(_AContinuousTask _task);
        void AddUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task);
        void AddLateUpdateContinuousTask(_AContinuousTask _task);
        void AddUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task);
        void RemoveUpdateContinuousTask(_AContinuousTask _task);
        void RemoveUnscaledTimeUpdateContinuousTask(_AContinuousTask _task);
        void RemoveFixedUpdateContinuousTask(_AContinuousTask _task);
        void RemoveUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task);
        void RemoveLateUpdateContinuousTask(_AContinuousTask _task);
        void RemoveUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task);

        void AddUpdateFrameDelayTask(_AFrameDelayTask _task);
        void AddFixedUpdateFrameDelayTask(_AFrameDelayTask _task);
        void AddLateUpdateFrameDelayTask(_AFrameDelayTask _task);
        void RemoveUpdateFrameDelayTask(_AFrameDelayTask _task);
        void RemoveFixedUpdateFrameDelayTask(_AFrameDelayTask _task);
        void RemoveLateUpdateFrameDelayTask(_AFrameDelayTask _task);

        void AddUpdateTimeDelayTask(_ATimeDelayTask _task);
        void AddUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task);
        void AddFixedUpdateTimeDelayTask(_ATimeDelayTask _task);
        void AddUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task);
        void AddLateUpdateTimeDelayTask(_ATimeDelayTask _task);
        void AddUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task);
        void RemoveUpdateTimeDelayTask(_ATimeDelayTask _task);
        void RemoveUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task);
        void RemoveFixedUpdateTimeDelayTask(_ATimeDelayTask _task);
        void RemoveUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task);
        void RemoveLateUpdateTimeDelayTask(_ATimeDelayTask _task);
        void RemoveUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task);
    }
}