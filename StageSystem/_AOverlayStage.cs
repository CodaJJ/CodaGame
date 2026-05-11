// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// An "Overlay" stage: a scene loaded additively on top of a Main stage without affecting it.
    /// </summary>
    /// <remarks>
    /// <para>Multiple Overlay stages can be loaded simultaneously. They participate in ref counting (multiple
    /// Load calls for the same scene asset increment the count, scene unloads only when the count reaches zero).</para>
    /// <para>Overlay stages do <b>not</b> affect the Active Scene and do <b>not</b> interact with Main stage mutual
    /// exclusion. Typical uses: detail/inspection scenes, side menus that need their own scene context, debug
    /// overlays.</para>
    /// <para>Load via <c>Stage.LoadOverlay</c>. Unload via <c>Stage.Unload</c>.</para>
    /// </remarks>
    public abstract class _AOverlayStage : _AStage
    {
    }
}
