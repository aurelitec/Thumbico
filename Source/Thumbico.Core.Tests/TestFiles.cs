// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;
using System.Drawing.Imaging;

namespace Thumbico.Tests;

/// <summary>
/// Sample shell items the tests render. Files are created once per run under a unique temporary
/// directory and deliberately left behind, so a failing run can be inspected afterwards.
/// </summary>
internal static class TestFiles
{
    private static readonly Lazy<string> Directory = new(CreateDirectory);

    private static readonly Lazy<string> Png = new(() => CreateImage("square.png", 512, 512));

    private static readonly Lazy<string> WideImage = new(() => CreateImage("wide.png", 512, 256));

    private static readonly Lazy<string> RedTopBlueBottom = new(CreateRedTopBlueBottomImage);

    private static readonly Lazy<string> RedTopBlueBottomIcon = new(CreateRedTopBlueBottomIcon);

    private static readonly Lazy<string> Text = new(CreateTextFile);

    /// <summary>Gets an image file, which the shell can render as a real thumbnail.</summary>
    internal static string PngFile => Png.Value;

    /// <summary>Gets a non-square image, used to prove that rotation swaps the dimensions.</summary>
    internal static string WideImageFile => WideImage.Value;

    /// <summary>Gets an image with a red band at the top and a blue one at the bottom, so that a
    /// vertically flipped result is unmistakable.</summary>
    internal static string RedTopBlueBottomFile => RedTopBlueBottom.Value;

    /// <summary>Gets the same red-over-blue image as an icon file, so that the icon path can be
    /// checked for orientation as precisely as the thumbnail path.</summary>
    internal static string RedTopBlueBottomIconFile => RedTopBlueBottomIcon.Value;

    /// <summary>Gets a plain text file, which has no thumbnail and so falls back to an icon.</summary>
    internal static string TextFile => Text.Value;

    /// <summary>Returns an unused path in the run's temporary directory.</summary>
    internal static string NewTempPath(string extension)
        => Path.Combine(Directory.Value, $"{Guid.NewGuid():N}.{extension}");

    private static string CreateDirectory()
    {
        string path = Path.Combine(Path.GetTempPath(), $"thumbico-tests-{Guid.NewGuid():N}");
        System.IO.Directory.CreateDirectory(path);
        Console.WriteLine($"Test files: {path}");

        return path;
    }

    private static string CreateImage(string name, int width, int height)
    {
        string path = Path.Combine(Directory.Value, name);

        using Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.CornflowerBlue);
            graphics.FillEllipse(Brushes.Orange, width / 4, height / 4, width / 2, height / 2);
        }

        bitmap.Save(path, ImageFormat.Png);

        return path;
    }

    private static string CreateRedTopBlueBottomImage()
    {
        string path = Path.Combine(Directory.Value, "red-top-blue-bottom.png");

        using Bitmap bitmap = new(400, 400, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            graphics.FillRectangle(Brushes.Red, 0, 0, 400, 120);
            graphics.FillRectangle(Brushes.Blue, 0, 280, 400, 120);
        }

        bitmap.Save(path, ImageFormat.Png);

        return path;
    }

    /// <summary>
    /// Writes a single-image icon file wrapping a PNG, which is the modern icon layout and the
    /// simplest one to author.
    /// </summary>
    private static string CreateRedTopBlueBottomIcon()
    {
        string path = Path.Combine(Directory.Value, "red-top-blue-bottom.ico");

        using Bitmap bitmap = new(256, 256, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            graphics.FillRectangle(Brushes.Red, 0, 0, 256, 70);
            graphics.FillRectangle(Brushes.Blue, 0, 186, 256, 70);
        }

        using MemoryStream png = new();
        bitmap.Save(png, ImageFormat.Png);
        byte[] image = png.ToArray();

        using BinaryWriter writer = new(File.Create(path));
        writer.Write((ushort)0);
        writer.Write((ushort)1);
        writer.Write((ushort)1);
        writer.Write((byte)0);
        writer.Write((byte)0);
        writer.Write((byte)0);
        writer.Write((byte)0);
        writer.Write((ushort)1);
        writer.Write((ushort)32);
        writer.Write(image.Length);
        writer.Write(22);
        writer.Write(image);

        return path;
    }

    private static string CreateTextFile()
    {
        string path = Path.Combine(Directory.Value, "document.txt");
        File.WriteAllText(path, "Thumbico test file.");

        return path;
    }
}
