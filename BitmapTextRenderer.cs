using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Zintom.Forms.Utilities.DrawingExtensions;
using System.IO;
using System.Diagnostics;

namespace Zintom.Forms
{
    /// <summary>
    /// A text renderer that uses GDI+ instead of the default SpriteFont rendering technique used by MonoGame/Xna; this is at the sacrifice of transparency due to the nature of GDI+, but makes the text oh so beautiful.
    /// <para/>
    /// Provides an easy abstraction layer above GDI+ so that you never have to interact with System.Drawing outside of this class(conversions between bitmap/texture2d, color/brushes etc are all done for you).
    /// </summary>
    public class BitmapTextRenderer
    {
        GraphicsDevice graphics;

        private String text = "";
        private System.Drawing.Font font;
        private float scale = 1f;
        private Vector2 size = Vector2.Zero;

        private Texture2D renderSurface;
        private System.Drawing.SolidBrush foreColor = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        private System.Drawing.Color backColor = System.Drawing.Color.Black;

        // For measuring text, these stay in memory so we don't have to re-instantate a new bitmap/graphics everytime we want to measure a string.
        private static System.Drawing.Bitmap measureBitmap;
        private static System.Drawing.Graphics measureGraphics;

        /// <summary>
        /// Gets the width of the rendered text.
        /// </summary>
        public float Width
        {
            get { return size.X * scale; }
        }
        /// <summary>
        /// Gets the height of the rendered text.
        /// </summary>
        public float Height
        {
            get { return size.Y * scale; }
        }
        /// <summary>
        /// Gets the width and height of the rendered text.
        /// </summary>
        public Vector2 Size
        {
            get { return size * scale; }
        }
        /// <summary>
        /// Gets or sets the scale of the rendered text.
        /// </summary>
        public float Scale { get => scale; set => scale = value; }

        /// <summary>
        /// Gets or sets the <see cref="System.Drawing.Font"/> of this renderer; setting this property causes re-painting of the text.
        /// </summary>
        public System.Drawing.Font Font
        {
            get => font;
            set
            {
                if (font != value) // Only update if font is different
                {
                    font = value;
                    if (text != "")
                        RenderText();
                }
            }
        }
        /// <summary>
        /// Gets or sets the text of this renderer; setting this property causes re-painting of the text.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                if (text != value) // Only update if text is different
                {
                    text = value;
                    RenderText();
                }
            }
        }
        /// <summary>
        /// Gets or sets the ForeColor; setting this property causes re-painting of the text.
        /// </summary>
        public Color ForeColor {
            get { return Color.FromNonPremultiplied(foreColor.Color.R, foreColor.Color.G, foreColor.Color.B, foreColor.Color.A); }
            set { foreColor = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B)); RenderText(); } }
        /// <summary>
        /// Gets or sets the BackColor; setting this property causes re-painting of the text.
        /// </summary>
        public Color BackColor {
            get { return Color.FromNonPremultiplied(backColor.R, backColor.G, backColor.B, backColor.A); }
            set { backColor = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B); RenderText(); } }

        static BitmapTextRenderer()
        {
            measureBitmap = new System.Drawing.Bitmap(1, 1);
            measureGraphics = System.Drawing.Graphics.FromImage(measureBitmap);
        }

        private bool initialized = false; // So that RenderText doesn't get called lots of times whilst we're initially setting lots of values.
        public BitmapTextRenderer(GraphicsDevice graphics, System.Drawing.Font font, string text, Color foregroundColor, Color backgroundColor, float scale = 1f)
        {
            this.font = font;
            this.text = text;
            this.scale = scale;
            this.graphics = graphics;
            ForeColor = foregroundColor;
            BackColor = backgroundColor;

            initialized = true;
            RenderText();
        }

        //        private void RenderText()
        //        {
        //            if (!initialized) return;

        //#if DEBUG
        //            Stopwatch stopwatch = new Stopwatch();
        //            stopwatch.Start();
        //#endif

        //            var size = System.Windows.Forms.TextRenderer.MeasureText(text, font);
        //            this.size = new Vector2(size.Width, size.Height);

        //            // Create the final bitmap
        //            var bmpSurface = new System.Drawing.Bitmap(size.Width, size.Height);
        //            var g = System.Drawing.Graphics.FromImage(bmpSurface);
        //            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        //            // Draw the text to the clean bitmap
        //            System.Windows.Forms.TextRenderer.DrawText(g, text, font, System.Drawing.Point.Empty, foreColor, backColor);

        //            // Save the bitmap to a stream and then re-load it as a Texture2D.
        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                bmpSurface.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //                renderSurface = Texture2D.FromStream(graphics, stream);
        //            }

        //            bmpSurface.Dispose();
        //            g.Dispose();

        //#if DEBUG
        //            stopwatch.Stop();
        //            Console.WriteLine("BitmapTextRenderer: Rendered text in " + stopwatch.ElapsedMilliseconds + "ms");
        //#endif
        //        }

        private void RenderText()
        {
            if (!initialized) return;

#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            // Measure the text with the already instantated measureGraphics object.
            var size = measureGraphics.MeasureString(text, font);
            this.size = new Vector2(size.Width, size.Height);

            // Create the final bitmap
            var bmpSurface = new System.Drawing.Bitmap((int)size.Width, (int)size.Height);
            var g = System.Drawing.Graphics.FromImage(bmpSurface);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw the text to the clean bitmap
            g.Clear(backColor);
            g.DrawString(text, font, foreColor, System.Drawing.PointF.Empty);

            // Save the bitmap to a stream and then re-load it as a Texture2D.
            using (MemoryStream stream = new MemoryStream())
            {
                bmpSurface.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                renderSurface = Texture2D.FromStream(graphics, stream);
            }

            bmpSurface.Dispose();
            g.Dispose();

#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("BitmapTextRenderer: Rendered text in " + stopwatch.ElapsedMilliseconds + "ms");
#endif
        }

        /// <summary>
        /// Draws the pre-rendered bitmap text to the given position, at the <see cref="Scale"/> specified, at the given opacity.
        /// </summary>
        /// <param name="position">The position to draw the text to.</param>
        /// <param name="opacity">The opacity at which to draw the text.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float opacity = 1)
        {
            spriteBatch.Draw(renderSurface, position, null, Color.White * opacity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Returns the text in which this renderer will draw.
        /// </summary>
        public override string ToString()
        {
            return text;
        }

    }
}
