using CustomScreenSaver.Animations;
using System.Drawing;
using System.Windows.Forms;

namespace CustomScreenSaver
{
    /**
     * Creating a Screen Saver in C#:
     *  https://sites.harding.edu/fmccown/screensaver/screensaver.html
     */
    static class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                    secondArgument = args[1];

                if (firstArgument == "/c")
                {

                }
                else if (firstArgument == "/p")
                {
                    Application.Exit();
                }
                else if (firstArgument == "/s")
                {
                    ShowScreenSaver();
                    Application.Run();
                }
                else
                {
                    MessageBox.Show(
                        "Invalid argument \"" + firstArgument + "\".",
                        "ScreenSaver",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation
                    );
                }
            }
            else
            {

            }
        }

        static void ShowScreenSaver()
        {
            // Show the screensaver on all screens.
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                // Pass in the desired animation.
                Rectangle bounds = Screen.AllScreens[i].Bounds;
                IAnimation animation = new LangtonPlusAnimation(bounds);

                ScreenSaverForm screensaver = new ScreenSaverForm(animation, bounds);
                screensaver.Show();
            }
        }
    }
}
