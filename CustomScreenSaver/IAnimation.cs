using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CustomScreenSaver
{
    public interface IAnimation
    {
        // The framerate at which the animation should be rendered.
        public int Framerate { get; }

        // Animate a new frame on the provided graphics instance.
        public void DrawFrame(double time, Graphics graphics, Rectangle bounds);
    }
}
