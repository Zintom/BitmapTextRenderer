# BitmapTextRenderer
A text renderer that uses GDI+ instead of the default SpriteFont rendering technique used by MonoGame/Xna; this is at the sacrifice of transparency due to the nature of GDI+, but makes the text oh so beautiful.

Provides an easy abstraction layer above GDI+ so that you never have to interact with System.Drawing outside of this class(conversions between bitmap/texture2d, color/brushes etc are all done for you).
