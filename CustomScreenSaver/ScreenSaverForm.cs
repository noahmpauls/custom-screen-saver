using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomScreenSaver
{
    /**
     * Form animations:
     *  https://stackoverflow.com/questions/188349/simple-animation-using-c-windows-forms
     * Using BufferedGraphics:
     *  https://www.codeproject.com/Articles/24664/Animation-on-Windows-Forms
     */
    public partial class ScreenSaverForm : Form
    {
        private BufferedGraphics bufferedGraphics;
        private Point mouseLocation;

        // Animation object to use.
        private IAnimation animation;

        // Desired FPS, determined by the selected animation.
        private readonly int framerate;

        // Stopwatch to keep track of elapsed real time.
        private Stopwatch stopwatch;

        // Construct the form; takes in an index, which should be the index of
        // the screen this form is displayed on.
        public ScreenSaverForm(IAnimation animation, Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            this.animation = animation;
            framerate = animation.Framerate;

            stopwatch = new Stopwatch();
        }

        // Perform setup when the screensaver loads.
        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            // Hide the cursor and make this the primary window.
            Cursor.Hide();
            TopMost = true;

            // Create the BufferedGraphics. Buffered allows drawing actions to
            // be given to the graphics object in batches, only writing them to
            // the screen when Render is called.
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            bufferedGraphics = context.Allocate(
                this.CreateGraphics(),
                new Rectangle(0, 0, this.Width, this.Height)
            );

            // Optimize the form for animation. Unsure if this actually improves
            // jitter.
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Start the interval timer with the desired framerate.
            MoveTimer.Interval = 1000 / framerate;
            MoveTimer.Tick += new EventHandler(MoveTimer_Tick);
            MoveTimer.Start();

            // Start real time once the form loads.
            stopwatch.Start();
        }

        // Exit the screensaver on a large enough mouse movement.
        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                    Math.Abs(mouseLocation.Y - e.Y) > 5)
                {
                    Application.Exit();
                }
            }
            mouseLocation = e.Location;
        }

        // Exit the screensaver on a mouse click.
        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        // Exit the screensaver on a key press.
        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            Application.Exit();
        }

        // Animate a new frame.
        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            Animate();
        }

        // Perform an animation step.
        private void Animate()
        {
            // Get the elapsed time and create a new graphics object.
            double time = (double)stopwatch.ElapsedMilliseconds / 1000;
            Graphics graphics = bufferedGraphics.Graphics;

            // Animate a new frame with the graphics and the elapsed time.
            animation.DrawFrame(time, graphics, Bounds);
            bufferedGraphics.Render(Graphics.FromHwnd(this.Handle));
        }
    }
}
