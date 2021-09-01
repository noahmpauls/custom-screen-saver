using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CustomScreenSaver.Animations
{
    class MandelbrotAnimation : IAnimation
    {
        private int borderSize = 0;
        private int cellSize = 2;
        private readonly Rectangle bounds;
        private int rows;
        private int cols;
        private int iterMax;
        private double zoom;
        private double shiftX, shiftY;
        private double shiftShiftX, shiftShiftY;
        private int itersPerFrame = 4;
        private ISet<int> uniqueIters = new HashSet<int>();

        private int r, c;

        public int Framerate { get => 60; }

        public MandelbrotAnimation(Rectangle bounds) {
            this.bounds = bounds;
            rows = (bounds.Height - borderSize) / (borderSize + cellSize) + 1;
            cols = (bounds.Width - borderSize) / (borderSize + cellSize) + 1;
            r = 0;
            c = 0;
            iterMax = 1000;
            zoom = 1;
            shiftX = 0;
            shiftY = 0;

            shiftShiftX = (new Random().NextDouble() * 100) + 200;
            shiftShiftY = (new Random().NextDouble() * 100) + 100;
        }

        public void DrawFrame(double t, Graphics g, Rectangle b) {
            for (int i = 0; i < itersPerFrame; i++) {
                for (r = 0; r < rows; r++) {

                    int x = (borderSize + cellSize) * c + borderSize;
                    int y = (borderSize + cellSize) * r + borderSize;

                    double px = ((double) (x - (bounds.Width / 2)) / zoom) + (bounds.Width / 2) - (shiftX / zoom);
                    double py = ((double) (y - (bounds.Height / 2)) / zoom) + (bounds.Height / 2) - (shiftY / zoom);

                    int iters = MandelbrotIterations(px, py, iterMax);
                    uniqueIters.Add(iters);

                    int color = (int) Math.Round((double) iters / (double) iterMax * 255);
                    SolidBrush brush = new SolidBrush(Color.FromArgb(
                            color, color, color
                        ));
                    g.FillRectangle(brush, x, y, cellSize, cellSize);
                }
                c = (c + 1);
                if (c % cols == 0) {
                    c = c % cols;
                    if (uniqueIters.Count > iterMax / 2) { 
                        shiftX += shiftShiftX * zoom;
                        shiftY += shiftShiftY * zoom;
                        zoom *= 2;
                        uniqueIters.Clear();
                    } else {
                        shiftX = 0;
                        shiftY = 0;
                        shiftShiftX = new Random().NextDouble() * 100 + 180;
                        shiftShiftY = new Random().NextDouble() * 100 + 30;
                        zoom = 1;
                        uniqueIters.Clear();
                    }
                }
            }
        }

        private int MandelbrotIterations(double px, double py, int max) {
            int i = 0;
            double scaleX = ((px / (double) bounds.Width) * 3.5) - 2.5;
            double scaleY = ((py / (double) bounds.Height) * 2) - 1;
            double x = 0;
            double y = 0;
            while ((x * x) + (y * y) <= (2*2) && i < max) {
                double temp = (x*x) - (y*y) + scaleX;
                y = (2 * x * y) + scaleY;
                x = temp;
                i++;
            }

            return i;
        }
    }
}