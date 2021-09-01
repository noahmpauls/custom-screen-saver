using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CustomScreenSaver.Animations
{
    class ConwayAnimation : IAnimation
    {

        private int borderSize = 2;
        private int cellSize = 8;
        private int rows;
        private int cols;
        private ICollection<(int, int)> live;

        public int Framerate { get => 20; }

        public ConwayAnimation(Rectangle bounds)
        {
            rows = (bounds.Height - borderSize) / (borderSize + cellSize) + 1;
            cols = (bounds.Width - borderSize) / (borderSize + cellSize) + 1;

            live = new HashSet<(int, int)>();

            Random rand = new Random();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int color = rand.Next(0, 8);
                    if (color == 0) live.Add((r, c));
                }
            }
        }

        public void DrawFrame(double t, Graphics g, Rectangle b)
        {
            g.Clear(Color.Black);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int x = (borderSize + cellSize) * c + borderSize;
                    int y = (borderSize + cellSize) * r + borderSize;
                    Brush brush = IsLive((r, c)) ? Brushes.White : Brushes.Black;
                    g.FillRectangle(brush, x, y, cellSize, cellSize);
                }
            }

            UpdateState();
        }

        private void UpdateState()
        {
            ICollection<(int, int)> nextLive = new HashSet<(int, int)>();
            ICollection<(int, int)> deadAndDealtWith = new HashSet<(int, int)>();
            foreach ((int, int) cell in live) {
                int liveNeighbors = 0;
                foreach ((int, int) neighbor in Neighbors(cell))
                {
                    if (IsLive(neighbor)) liveNeighbors++;
                    else if (!deadAndDealtWith.Contains(neighbor))
                    {
                        if (TryPopulate(neighbor)) nextLive.Add(neighbor);
                        deadAndDealtWith.Add(neighbor);
                    }
                }
                if (liveNeighbors == 2 || liveNeighbors == 3) nextLive.Add(cell);
            }

            live = nextLive;
        }

        private bool TryPopulate((int, int) cell)
        {
            int liveNeighbors = 0;
            foreach ((int, int) neighbor in Neighbors(cell))
            {
                if (IsLive(neighbor)) liveNeighbors++;
            }
            return liveNeighbors == 3;
        }

        private ICollection<(int, int)> Neighbors((int, int) cell)
        {
            ICollection<(int, int)> neighbors = new List<(int, int)>();
            foreach (int i in new int[] { -1, 0, 1 })
            {
                foreach (int j in new int[] { -1, 0, 1 })
                {
                    (int, int) neighbor = WrapAround((cell.Item1 + i, cell.Item2 + j));
                    if (neighbor != cell) neighbors.Add(neighbor);
                }
            }
            return neighbors;
        }

        private bool IsLive((int, int) cell)
        {
            return live.Contains(cell);
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
