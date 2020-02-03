# BitmapTextRenderer
### Solely for MonoGame projects designed to run on windows only(makes use of GDI+)

## The problem
MonoGame/XNA uses a text rendering process which involves generating "SpriteFont"'s through their pipeline tool, it's easy enough to do and easy to draw to the screen, the issue is that the text is often very poorly drawn leaving edges of characters too sharp or too blurry(most of the time both), which doesn't make for a good user/player experience. Text is hard to read, looks awful and is not dynamic as you are required to build a new SpriteFont for every font and each individual size of that font.

## Solution
As this is for folks with Windows, we have access to all the fun stuff from WinForms etc, including GDI+ which has an excellent text rendering pipeline through Drawing.Graphics(or you can use TextRenderer if you prefer GDI).

The solution is to draw text with GDI+ and convert the output to a Texture2D for drawing with the SpriteBatch; my BitmapTextRenderer class does all this for you in a neat package which includes:

### Automatic conversion between WinForms and Xna types
When setting colours you can provide the class Xna/Monogame types and the class will automatically convert these to the relevant WinForms equivalent, so from an external point of view you can't even notice that we are using GDI+.

### Saving of size/scale properties for easy reference
Whenever you modify the state of the object it has to re-measure and render the text; usually with Xna or GDI+ you have to manually call a MeasureText function and pass all the attributes like Font etc in, it just become a pain and introduces a lot of repetative boilerplate code, so what my class does is store all these handy values from the render process in publically accessible Properties (Width, Height, Size, Scale etc).

TODO: Provide example screenshots of space saving.

## Sacrifce
Not everything can be all good and no bad, this class is no different; unfortunately due to the limitations of GDI/GDI+ you cannot paint text onto a transparent background(well you can but it looks absolutely disgusting, much worse than SpriteFont). Also the performance is hit very slightly (a few milliseconds) due to the nature of slow GDI+.

## Scenario for usage
The primary scenario I can see this class being used in, is for long spouts of text, i.e anywhere ingame where the user really needs to see what they're reading, for example copyright notices, textboxes, window labels, other game elements like books with text in etc.

I don't see this being used for small text items that flash up on screen very briefly or ones that fundamentally need transparency, i.e a hitmarker showing the amount of damage done above a players head, this can still be achieved with SpriteFont to a <i>decent</i> extent.
