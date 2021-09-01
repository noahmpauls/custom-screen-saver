using System;
using System.Drawing;

namespace CustomScreenSaver.Animations
{
    /**
     * Animation is heavily based on FrankForce's Mini Black Hole:
     * https://frankforce.com/dissecting-a-dweet-black-hole/
     */
    class BlackHoleAnimation : IAnimation
    {

        public int Framerate { get => 24; }

        public void DrawFrame(double time, Graphics graphics, Rectangle bounds)
        {
            // Tweakable parameters
            double tStart = time + 7;
            double spiralSpeed = 150;
            double widthFactor = 1;
            double tiltFactor = 0.4;
            double spiralDepth = 4000;
            double twinkleFrequency = 0.7;
            double maxTwinkle = 7;
            double distanceFactor = 500;
            double yAdjust = -50;
            int trailLength = 180;

            // Fill in semi-transparent background
            SolidBrush backBrush = new SolidBrush(
                Color.FromArgb(255 - trailLength, 0, 0, 0)
            );
            graphics.FillRectangle(backBrush, 0, 0, bounds.Width, bounds.Height);

            // Create stars
            for (int i = 1; i < 1750; i+=1)
            {

                double spiralWidth = Math.Sin(i * i) * widthFactor;

                // Star position and size
                double x = Math.Round(bounds.Width / 2 + (i * Math.Sin(spiralSpeed * (tStart / i) + spiralWidth)));
                double y = Math.Round(bounds.Height / 2 + tiltFactor * ((i * Math.Cos(spiralSpeed * (tStart / i) + spiralWidth) + spiralDepth / i)) - yAdjust);
                int size = (int)Math.Abs(Math.Round(((y + distanceFactor) / (bounds.Height + distanceFactor)) * Math.Cos((twinkleFrequency * time) + (100 * i * i)) * maxTwinkle));

                SolidBrush brush = new SolidBrush(Color.FromArgb(
                    Math.Min(10 * i + 30, 255),
                    Math.Min(1 * i + 30, 255),
                    Math.Min(2 * i + 30, 255)
                ));
                graphics.FillRectangle(brush, (int)x, (int)y, size, size);
            }
        }
    }
}
