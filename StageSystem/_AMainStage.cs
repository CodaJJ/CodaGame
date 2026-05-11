// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// A "Main" stage: the primary playable scene. At most one Main stage is loaded at a time.
    /// </summary>
    /// <remarks>
    /// <para>The Stage system enforces mutual exclusion via load-and-swap: loading a new Main loads the new
    /// scene first, then atomically swaps (Active Scene + <c>currentMain</c>) and unloads the previous one.
    /// If the newly loaded scene fails type validation, it is unloaded and the existing Main is unchanged.</para>
    /// <para>When a Main stage is loaded it becomes the Unity Active Scene — newly instantiated GameObjects
    /// default into it, and its Lighting/Skybox/RenderSettings take effect.</para>
    /// <para>Load via <c>Stage.LoadMain</c>. Main lifetime is owned by the framework — to switch, load a
    /// different Main.</para>
    /// </remarks>
    public abstract class _AMainStage : _AStage
    {
    }
}
