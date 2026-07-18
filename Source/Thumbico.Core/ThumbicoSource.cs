// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// Which kind of image the shell should return for an item.
/// </summary>
public enum ThumbicoSource
{
    /// <summary>Return the thumbnail if the item has one, otherwise its icon.</summary>
    Auto,

    /// <summary>Return only a thumbnail. Fails for items that have none.</summary>
    ThumbnailOnly,

    /// <summary>Return only an icon, never a thumbnail.</summary>
    IconOnly,
}
