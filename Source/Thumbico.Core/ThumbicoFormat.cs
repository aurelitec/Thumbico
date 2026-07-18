// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// Image file formats a thumbico can be saved to.
/// </summary>
public enum ThumbicoFormat
{
    /// <summary>Windows bitmap.</summary>
    Bmp,

    /// <summary>Graphics Interchange Format.</summary>
    Gif,

    /// <summary>JPEG. Does not preserve transparency.</summary>
    Jpeg,

    /// <summary>Portable Network Graphics. Preserves transparency.</summary>
    Png,

    /// <summary>Tagged Image File Format.</summary>
    Tiff,
}
