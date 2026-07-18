// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// Options that shape how the shell renders a thumbnail or icon.
/// </summary>
/// <remarks>
/// The default, <see cref="None"/>, shrinks the image to fit the requested size while preserving
/// its aspect ratio. Each option maps to a SIIGBF flag; see the mapping in ShellImage.
/// </remarks>
[Flags]
public enum ThumbicoOptions
{
    /// <summary>Shrink to fit the requested size, preserving aspect ratio.</summary>
    None = 0,

    /// <summary>Accept an image larger than requested, leaving any scaling to the caller.</summary>
    AllowLargerSize = 1 << 0,

    /// <summary>Return the image only if it is already in memory.</summary>
    InMemoryOnly = 1 << 1,

    /// <summary>Allow disk access, but only to read an already cached image.</summary>
    InCacheOnly = 1 << 2,

    /// <summary>Crop the image to a square.</summary>
    CropToSquare = 1 << 3,

    /// <summary>Stretch and crop the image to a 0.7 aspect ratio.</summary>
    WideAspect = 1 << 4,

    /// <summary>For icons, paint the background in the associated app's registered color.</summary>
    IconBackground = 1 << 5,

    /// <summary>Stretch a smaller image up so both dimensions fill the requested size.</summary>
    ScaleUp = 1 << 6,
}
