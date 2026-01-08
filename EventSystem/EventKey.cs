// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame 
{
    /// <summary>
    /// Event key with no parameters
    /// </summary>
    public class EventKey 
    {
        // Unique identifier for the event key
        private readonly uint _m_id;
        

        public EventKey() 
        {
            _m_id = Serialize.NextEventKey();
        }
        
        
        /// <summary>
        /// Gets the unique identifier of the event key.
        /// </summary>
        internal uint id { get { return _m_id; } }
    }
    /// <summary>
    /// Event key with 1 parameter
    /// </summary>
    public class EventKey<T1> : EventKey { }
    /// <summary>
    /// Event key with 2 parameters
    /// </summary>
    public class EventKey<T1, T2> : EventKey { }
    /// <summary>
    /// Event key with 3 parameters
    /// </summary>
    public class EventKey<T1, T2, T3> : EventKey { }
    /// <summary>
    /// Event key with 4 parameters
    /// </summary>
    public class EventKey<T1, T2, T3, T4> : EventKey { }
}
