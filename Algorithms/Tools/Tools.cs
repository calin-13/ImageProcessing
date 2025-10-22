using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, byte T = 128)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(inputImage.Data[y, x, 0] > T ? 255 : 0);
                }
            }

            return result;
        }

        #endregion

        #region Mirror

        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - 1 - x, 0];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int c = 0; c < inputImage.NumberOfChannels; ++c)
                        result.Data[y, x, c] = inputImage.Data[y, inputImage.Width - 1 - x, c];
                }
            }

            return result;
        }

        #endregion

        #region Rotate

        public static Image<Gray, byte> Rotate(Image<Gray, byte> inputImage, bool clockwise)
        {
            // When rotating 90 degrees, width and height are swapped
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (clockwise)
                        result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    else
                        result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Rotate(Image<Bgr, byte> inputImage, bool clockwise)
        {
            // When rotating 90 degrees, width and height are swapped
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int c = 0; c < inputImage.NumberOfChannels; ++c)
                    {
                        if (clockwise)
                            result.Data[x, inputImage.Height - 1 - y, c] = inputImage.Data[y, x, c];
                        else
                            result.Data[inputImage.Width - 1 - x, y, c] = inputImage.Data[y, x, c];
                    }
                }
            }

            return result;
        }

        #endregion

        #region Crop
        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, int x0, int y0, int x1, int y1, out double mean, out double std)
        {
            x0 = Math.Max(0, x0);
            y0 = Math.Max(0, y0);
            x1 = Math.Min(inputImage.Width - 1, x1);
            y1 = Math.Min(inputImage.Height - 1, y1);

            if (x0 > x1 || y0 > y1)
                throw new ArgumentException("Invalid Crop");

            int newHeight = y1 - y0 + 1;
            int newWidth = x1 - x0 + 1;

            mean = 0;
            std = 0;

            double sum = 0;
            double sumSq = 0;
            long n = (long)newWidth * newHeight;

            Image<Gray, byte> result = new Image<Gray, byte>(newWidth, newHeight);

            for (int y = 0; y < newHeight; ++y)
            {
                for (int x = 0; x < newWidth; ++x)
                {
                    byte value = inputImage.Data[y0 + y, x0 + x, 0];

                    result.Data[y, x, 0] = value;

                    sum += value;
                    sumSq += value * value;
                }
            }

            mean = sum / n;

            double variance = (sumSq / n) - (mean * mean);
            variance = variance < 0 ? 0 : variance;

            std = Math.Sqrt(variance);

            return result;
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, int x0, int y0, int x1, int y1, out double[] mean, out double[] std)
        {
            x0 = Math.Max(0, x0);
            y0 = Math.Max(0, y0);
            x1 = Math.Min(inputImage.Width - 1, x1);
            y1 = Math.Min(inputImage.Height - 1, y1);

            if (x0 > x1 || y0 > y1)
                throw new ArgumentException("Invalid Crop");

            int newHeight = y1 - y0 + 1;
            int newWidth = x1 - x0 + 1;

            mean = new double[inputImage.NumberOfChannels];
            std = new double[inputImage.NumberOfChannels];

            double[] sum = new double[inputImage.NumberOfChannels];
            double[] sumSq = new double[inputImage.NumberOfChannels];
            long n = (long)newWidth * newHeight;

            Image<Bgr, byte> result = new Image<Bgr, byte>(newWidth, newHeight);

            for (int y = 0; y < newHeight; ++y)
            {
                for (int x = 0; x < newWidth; ++x)
                {
                    for (int c = 0; c < inputImage.NumberOfChannels; ++c)
                    {
                        byte value = inputImage.Data[y0 + y, x0 + x, c];

                        result.Data[y, x, c] = value;

                        sum[c] += value;
                        sumSq[c] += value * value;
                    }
                }
            }

            for (int c = 0; c < inputImage.NumberOfChannels; ++c)
            {
                mean[c] = sum[c] / n;

                double variance = (sumSq[c] / n) - (mean[c] * mean[c]);
                variance = variance < 0 ? 0 : variance;

                std[c] = Math.Sqrt(variance);
            }

            return result;
        }

        #endregion
    }
}