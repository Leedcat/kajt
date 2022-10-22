using System.Drawing.Imaging;
using System.Drawing;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace kajt
{
    // TODO: Come up with a better name than `DetectionHandler`
    // TODO: Make DetectionHandler use another thread for loop
    class DetectionHandler
    {
        private static Logger logger = new Logger("screen");
        private Process process;
        private Rectangle bounds;
        private Thread thread;
        private bool running;

        public DetectionHandler(Process process)
        {
            DetectionHandler.logger.Debug("Creating DetectionHandler");
            this.process = process;

            this.UpdateProcessBounds();

            DetectionHandler.logger.Debug("Creating thread");
            this.thread = new Thread(new ThreadStart(this.Loop));
            this.running = true;
            this.thread.Start();
            DetectionHandler.logger.Debug("Started thread");
        }

        private void UpdateProcessBounds()
        {
            // Get the bounds of the process' window
            User32.RECT rect;
            User32.GetWindowRect(this.process.MainWindowHandle, out rect);
            this.bounds = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private Bitmap? GrabScreenPart()
        {
            try
            {
                Bitmap bitmap = new Bitmap(this.bounds.Width, this.bounds.Height, PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(this.bounds.Left, this.bounds.Top, 0, 0, this.bounds.Size);
                graphics.Dispose();

                return bitmap;
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public void Dispose()
        {
            this.running = false;
            this.thread.Join();
        }

        private void Loop()
        {
            while (this.running)
            {
                if (this.GrabScreenPart() == null)
                {
                    this.UpdateProcessBounds();
                }
                else
                {
                    DetectionHandler.logger.Debug("Grabbed screen");
                }

                // TODO: Check if the process no longer exists, exit the loop if it doesn't
            }
        }
    }
}