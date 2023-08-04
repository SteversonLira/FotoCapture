using System;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using System.Drawing;
using System.Security.Principal;

namespace FotoCapture
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

            videoSource.Start();

            Console.ReadKey();
            videoSource.SignalToStop();
            videoSource.WaitForStop();
        }
        static void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            int width = image.Width;
            int height = image.Height;

            // Processar a imagem e convertê-la em arte de ponto (ASCII art)
            string asciiArt = ConvertToAsciiArt(image, width, height);

            Console.Clear();
            Console.WriteLine(asciiArt);
        }
        static string ConvertToAsciiArt(Bitmap image, int width, int height)
        {
            ResizeBilinear resizeFilter = new ResizeBilinear(Console.WindowWidth, Console.WindowHeight);
            Bitmap resizedImage = resizeFilter.Apply(image);

            string asciiArt = "";

            for (int y = 0; y < Console.WindowHeight; y++)
            {
                for (int x = 0; x < Console.WindowWidth; x++)
                {
                    System.Drawing.Color pixelColor = resizedImage.GetPixel(x, y);
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    char asciiChar = GetAsciiChar(grayValue);
                    asciiArt += asciiChar;
                }

                asciiArt += Environment.NewLine;
            }

            return asciiArt;
        }
        static char GetAsciiChar(int grayValue)
        {
            char[] asciiChars = { ' ', '.', ':', '-', '=', '+', '*', '#', '%', '@' };
            int step = 256 / asciiChars.Length;
            int index = grayValue / step;

            if (index >= asciiChars.Length)
                index = asciiChars.Length - 1;

            return asciiChars[index];
        }
    }
}
