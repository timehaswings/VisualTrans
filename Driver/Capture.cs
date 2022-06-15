using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Swan.Logging;

namespace VisualTrans.Driver
{
    class Capture
    {
        private readonly EncoderParameter qualityParam;
        private readonly ImageCodecInfo imageCodec;
        private readonly EncoderParameters parameters;
        private readonly long quality = 90L;

        public Capture()
        {
            qualityParam = new EncoderParameter(Encoder.Quality, quality);
            imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
            parameters = new EncoderParameters(1);
            parameters.Param[0] = qualityParam;
        }

        public byte[] GetScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            try
            {
                using var ms = new MemoryStream();
                using var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                using (var gfx = Graphics.FromImage(bitmap))
                {
                    gfx.CopyFromScreen(0, 0, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                    CURSORINFO pci;
                    pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                    if (GetCursorInfo(out pci))
                    {
                        if (pci.flags == CURSOR_SHOWING)
                        {
                            DrawIcon(gfx.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                            gfx.ReleaseHdc();
                        }
                    }
                }
                bitmap.Save(ms, imageCodec, parameters);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;
    }
}
