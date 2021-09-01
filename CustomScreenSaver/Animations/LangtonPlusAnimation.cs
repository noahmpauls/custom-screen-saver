using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CustomScreenSaver.Animations
{
    /**
     * https://en.wikipedia.org/wiki/Langton%27s_ant
     */
    class LangtonPlusAnimation : IAnimation
    {
        private double lastTime;

        private int borderSize = 0;
        private int cellSize = 4;

        private int rows, cols;

        /**
         *   0
         * 3   1
         *   2
         */
        private int antDirection;
        private (int, int) antPosition;

        private IDictionary<(int, int), int> on;

        private IList<(Brush, int)> rules = new List<(Brush, int)>
        /*
        {
            (Brushes.Black, 0),
            (Brushes.White, 0),
            (Brushes.Black, 1),
            (Brushes.White, 1),
            (Brushes.Black, 1),
            (Brushes.White, 0),
            (Brushes.Black, 1),
            (Brushes.White, 0),
            (Brushes.Black, 1),
            (Brushes.White, 0),
            (Brushes.Black, 0),
            (Brushes.White, 1),
        };
        */
        {
            (Brushes.Black,         0),
            (Brushes.OrangeRed,     1),
            (Brushes.Orange ,       1),
            (Brushes.Yellow,        1),
            (Brushes.YellowGreen,   1),
            (Brushes.Green,         1),
            (Brushes.Blue,          0),
            (Brushes.BlueViolet,    0),
            (Brushes.Purple,        1)
        };
        /*
        {
            (Brushes.Black,         0),
            (Brushes.DarkGray,     1),
            (Brushes.LightGray ,       0),
            (Brushes.White,        0),
        };
        */

        public int Framerate { get => 60; }

        public LangtonPlusAnimation(Rectangle bounds)
        {
            rows = (bounds.Height - borderSize) / (borderSize + cellSize) + 1;
            cols = (bounds.Width - borderSize) / (borderSize + cellSize) + 1;

            antPosition = (rows / 2, cols / 2);
            antDirection = 3;

            on = new Dictionary<(int, int), int>();

            lastTime = 0;
        }

        public void DrawFrame(double t, Graphics g, Rectangle b)
        {
            g.Clear(Color.White);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int x = (borderSize + cellSize) * c + borderSize;
                    int y = (borderSize + cellSize) * r + borderSize;
                    Brush brush;
                    if (on.ContainsKey((r, c)))
                    {
                        brush = rules[on[(r, c)]].Item1;
                    } 
                    else
                    {
                        brush = rules[0].Item1;
                    }
                    if (antPosition == (r, c))
                    {
                        brush = Brushes.Red;
                    }
                    
                    g.FillRectangle(brush, x, y, cellSize, cellSize);
                }
            }

            int iterations = (int) ((t - lastTime) * 5000);
            for (int i = 0; i < iterations; i++)
            {
                UpdateState();
            }

            lastTime = t;
        }

        private void UpdateState()
        {
            if (on.ContainsKey(antPosition))
            {
                int turn = rules[on[antPosition]].Item2;
                if (turn == 0) RotateClockwise(); else RotateCounter();
                on[antPosition] = (on[antPosition] + 1) % rules.Count;
                if (on[antPosition] == 0) on.Remove(antPosition);
            }
            else
            {
                int turn = rules[0].Item2;
                if (turn == 0) RotateClockwise(); else RotateCounter();
                on.Add(antPosition, 1);
            }

            MoveForward();
        }

        private void MoveForward()
        {
            switch(antDirection)
            {
                case 0:
                    antPosition = WrapAround((antPosition.Item1 - 1, antPosition.Item2));
                    break;
                case 1:
                    antPosition = WrapAround((antPosition.Item1, antPosition.Item2 + 1));
                    break;
                case 2:
                    antPosition = WrapAround((antPosition.Item1 + 1, antPosition.Item2));
                    break;
                case 3:
                    antPosition = WrapAround((antPosition.Item1, antPosition.Item2 - 1));
                    break;
            }
        }

        private void RotateClockwise()
        {
            antDirection = (antDirection + 1) % 4;
        }

        private void RotateCounter()
        {
            antDirection -= 1;
            if (antDirection < 0) antDirection = 4 + antDirection;
        }

        private bool Inbounds((int, int) cell)
        {
            return cell.Item1 >= 0 && cell.Item1 < rows
                && cell.Item2 >= 0 && cell.Item2 < cols;
        }

        private (int, int) WrapAround((int, int) cell)
        {
            int r = cell.Item1;
            int c = cell.Item2;

            if (cell.Item1 < 0)
            {
                r = rows + cell.Item1;
            }
            else if (cell.Item1 >= rows)
            {
                r = cell.Item1 - rows;
            }

            if (cell.Item2 < 0)
            {
                c = cols + cell.Item2;
            }
            else if (cell.Item2 >= cols)
            {
                c = cell.Item2 - cols;
            }

            return (r, c);
        }
    }
}
