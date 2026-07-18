// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// A rotation or flip that can be applied to a thumbico.
/// </summary>
public enum ThumbicoTransform
{
    /// <summary>Rotate 90 degrees counter-clockwise.</summary>
    RotateLeft,

    /// <summary>Rotate 90 degrees clockwise.</summary>
    RotateRight,

    /// <summary>Mirror horizontally.</summary>
    FlipHorizontal,

    /// <summary>Mirror vertically.</summary>
    FlipVertical,
}
