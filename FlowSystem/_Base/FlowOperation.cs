// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame.Base
{
    internal partial class FlowManager
    {
        /// <summary>
        /// Abstract base for queued flow operations.
        /// </summary>
        private abstract class _AFlowOperation
        {
            /// <summary>
            /// Execute this operation on the given manager.
            /// </summary>
            public abstract void Execute([NotNull] FlowManager _manager);
        }


        /// <summary>
        /// Queued Enter operation.
        /// </summary>
        private class EnterOperation : _AFlowOperation
        {
            [NotNull] private readonly _AFlow _m_flow;
            private readonly EFlowMode _m_mode;
            private readonly _ILoadingShow _m_loadingShow;


            public EnterOperation([NotNull] _AFlow _flow, EFlowMode _mode, _ILoadingShow _loadingShow)
            {
                _m_flow = _flow;
                _m_mode = _mode;
                _m_loadingShow = _loadingShow;
            }


            public override void Execute(FlowManager _manager)
            {
                if (_m_flow.flowState != EFlowState.None)
                {
                    Console.LogError(SystemNames.Flow, _m_flow.flowName, "Queued Enter skipped: flow has already been entered.");
                    _manager.ProcessNextOperation();
                    return;
                }

                _manager.ProcessEnter(_m_flow, _m_mode, _m_loadingShow);
            }
        }
        /// <summary>
        /// Queued Exit operation with no validation.
        /// </summary>
        private class ExitOperation : _AFlowOperation
        {
            public override void Execute(FlowManager _manager)
            {
                if (!_manager.ValidateExit(null, null))
                {
                    _manager.ProcessNextOperation();
                    return;
                }

                _manager.ProcessExit();
            }
        }
        /// <summary>
        /// Queued Exit operation with name validation.
        /// </summary>
        private class ExitWithNameOperation : _AFlowOperation
        {
            private readonly string _m_name;


            public ExitWithNameOperation(string _name)
            {
                _m_name = _name;
            }


            public override void Execute(FlowManager _manager)
            {
                if (!_manager.ValidateExit(_m_name, null))
                {
                    _manager.ProcessNextOperation();
                    return;
                }

                _manager.ProcessExit();
            }
        }
        /// <summary>
        /// Queued Exit operation with reference validation.
        /// </summary>
        private class ExitWithFlowOperation : _AFlowOperation
        {
            private readonly _AFlow _m_flow;


            public ExitWithFlowOperation(_AFlow _flow)
            {
                _m_flow = _flow;
            }


            public override void Execute(FlowManager _manager)
            {
                if (!_manager.ValidateExit(null, _m_flow))
                {
                    _manager.ProcessNextOperation();
                    return;
                }

                _manager.ProcessExit();
            }
        }
    }
}
