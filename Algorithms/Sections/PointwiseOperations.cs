using System;
using System.Linq;
using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        #region Contrast and Brightness
        public static Image<Gray, byte> ConstrastAndBrightness(Image<Gray, byte> inputImage, double alpha, double beta)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            if (alpha < 0) alpha = 0;

            var LUT = new byte[256];
            
            for (int r = 0; r < 256; r++)
            {
                double s = alpha * r + beta;
                if (s < 0) s = 0;
                else if (s > 255) s = 255;
                LUT[r] = (byte)Math.Round(s);
            }

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte src = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = LUT[src];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ContrastAndBrightness(Image<Bgr, byte> inputImage, double alpha, double beta)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            
            var LUT = new byte[256];
            
            for (int r = 0; r < 256; r++)
            {
                double s = alpha * r + beta;
                if (s < 0) s = 0;
                else if (s > 255) s = 255;
                LUT[r] = (byte)Math.Round(s);
            }

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int c = 0; c < inputImage.NumberOfChannels; ++c)
                    {
                        byte src = inputImage.Data[y, x, c];
                        result.Data[y, x, c] = LUT[src];
                    }
                }
            }

            return result;
        }
        #endregion
        
        #region Gamma operator
        public static Image<Gray, byte> GammaOperator(Image<Gray, byte> inputImage, double gamma)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            
            var LUT = new byte[256];

            double C = Math.Pow(255, 1 - gamma);
            
            if (gamma < 0) gamma = 0;
            
            for (int r = 0; r < 256; r++)
            {
                double s = C * Math.Pow(r, gamma);
                if (s < 0) s = 0;
                else if (s > 255) s = 255;
                LUT[r] = (byte)Math.Round(s);
            }

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte src = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = LUT[src];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> GammaOperator(Image<Bgr, byte> inputImage, double gamma)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            
            var LUT = new byte[256];

            double C = Math.Pow(255, 1 - gamma);
            
            if (gamma < 0) gamma = 0;
            
            for (int r = 0; r < 256; r++)
            {
                double s = C * Math.Pow(r, gamma);
                if (s < 0) s = 0;
                else if (s > 255) s = 255;
                LUT[r] = (byte)Math.Round(s);
            }

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int c = 0; c < inputImage.NumberOfChannels; ++c)
                    {
                        byte src = inputImage.Data[y, x, c];
                        result.Data[y, x, c] = LUT[src];
                    }
                }
            }

            return result;
        }
        #endregion
        
        #region Normalized histogram

        public static double[] NormalizedHistogram(Image<Gray, byte> inputImage)
        {
            int[] histogram = Utils.ComputeHistogram(inputImage);
            int totalPixels = inputImage.Width * inputImage.Height;

            double[] normalized = new double[256];

            for (int i = 0; i < 256; i++)
                normalized[i] = (double)histogram[i] / totalPixels;

            return normalized;
        }
        #endregion
    }
}