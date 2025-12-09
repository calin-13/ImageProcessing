using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class GeometricTransformations
    {
        private static double InterpolateLinear(double x, double f0, double f1)
        {
            double fractional = x - Math.Floor(x);
            return fractional * f1 + (1 - fractional) * f0;
        }
        private static double InterpolateBilinear(double x, double y, double[] f)
        {
            double p0 = InterpolateLinear(x, f[0], f[1]);
            double p1 = InterpolateLinear(x, f[2], f[3]);
            return InterpolateLinear(y, p0, p1);
        }
        
        private static double InterpolateCubic(double x, double f_minus1, double f0, double f1, double f2)
        {
            double fractional = x - Math.Floor(x);
            double result = 0.5 * (
                (-Math.Pow(fractional, 3) + 2 * Math.Pow(fractional, 2) - fractional) * f_minus1 +
                (3 * Math.Pow(fractional, 3) - 5 * Math.Pow(fractional, 2) + 2) * f0 +
                (-3 * Math.Pow(fractional, 3) + 4 * Math.Pow(fractional, 2) + fractional) * f1 +
                (Math.Pow(fractional, 3) - Math.Pow(fractional, 2)) * f2
            );
            return result;
        }
        
        private static double InterpolateBicubic(double x, double y, double[,] f)
        {
            double p0 = InterpolateCubic(x, f[0, 0], f[0, 1], f[0, 2], f[0, 3]);
            double p1 = InterpolateCubic(x, f[1, 0], f[1, 1], f[1, 2], f[1, 3]);
            double p2 = InterpolateCubic(x, f[2, 0], f[2, 1], f[2, 2], f[2, 3]);
            double p3 = InterpolateCubic(x, f[3, 0], f[3, 1], f[3, 2], f[3, 3]);
    
            return InterpolateCubic(y, p0, p1, p2, p3);
        }
        public static Image<Gray, byte> ScaleBilinear(Image<Gray, byte> image, double sx, double sy)
        {
            int width = image.Width;
            int height = image.Height;

            int newWidth = (int)(sx * width);
            int newHeight = (int)(sy * height);
            Image<Gray, byte> result = new Image<Gray, byte>(newWidth, newHeight);

            byte[,,] imageData = image.Data;
            byte[,,] resultData = result.Data;
            for (int y_prime = 0; y_prime < newHeight; y_prime++)
            {
                for (int x_prime = 0; x_prime < newWidth; x_prime++)
                {
                    double xc = x_prime / sx;
                    double yc = y_prime / sy;

                    int x0 = (int)xc;
                    int y0 = (int)yc;

                    if (x0 >= 1 && x0 < width - 2 && y0 >= 1 && y0 < height - 2)
                    {
                        double [] f = new double[4];
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                f[2 * i + j] = imageData[y0 - 1 + i, x0 - 1 + j, 0];
                            }
                        }

                        double interpolatedValue = InterpolateBilinear(xc, yc, f);
                        resultData[y_prime, x_prime, 0] = (byte)(interpolatedValue + 0.5);
                    }
                }
            }

            return result;
        }
        
        public static Image<Gray, byte> ScaleBilinearColor(Image<Gray, byte> image, double sx, double sy)
        {
            int width = image.Width;
            int height = image.Height;

            int newWidth = (int)(sx * width);
            int newHeight = (int)(sy * height);
            Image<Gray, byte> result = new Image<Gray, byte>(newWidth, newHeight);

            byte[,,] imageData = image.Data;
            byte[,,] resultData = result.Data;
            for (int y_prime = 0; y_prime < newHeight; y_prime++)
            {
                for (int x_prime = 0; x_prime < newWidth; x_prime++)
                {
                    double xc = x_prime / sx;
                    double yc = y_prime / sy;

                    int x0 = (int)xc;
                    int y0 = (int)yc;

                    if (x0 >= 0 && x0 < width - 1 && y0 >= 0 && y0 < height - 1)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            double[] f = new double[4];
                    
                            for (int i = 0; i < 2; i++)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    f[2 * i + j] = imageData[y0 + i, x0 + j, channel];
                                }
                            }
                    
                            double interpolatedValue = InterpolateBilinear(xc, yc, f);
                            resultData[y_prime, x_prime, 0] = (byte)(interpolatedValue + 0.5);
                        }
                    }
                }
            }

            return result;
        }
        public static Image<Gray, byte> ScaleBicubic(Image<Gray, byte> image, double sx, double sy)
        {
            int width = image.Width;
            int height = image.Height;
    
            int newWidth = (int)(sx * width);
            int newHeight = (int)(sy * height);
    
            Image<Gray, byte> result = new Image<Gray, byte>(newWidth, newHeight);
            byte[,,] imageData = image.Data;
            byte[,,] resultData = result.Data;
    
            for (int y_prime = 0; y_prime < newHeight; y_prime++)
            {
                for (int x_prime = 0; x_prime < newWidth; x_prime++)
                {
                    double xc = x_prime / sx;
                    double yc = y_prime / sy;
            
                    int x0 = (int)Math.Floor(xc);
                    int y0 = (int)Math.Floor(yc);
            
                    if (x0 >= 1 && x0 < width - 2 && y0 >= 1 && y0 < height - 2)
                    {
                        double[,] f = new double[4, 4];
                
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                f[i, j] = imageData[y0 - 1 + i, x0 - 1 + j, 0];
                            }
                        }
                
                        double interpolatedValue = InterpolateBicubic(xc, yc, f);
                        resultData[y_prime, x_prime, 0] = (byte)(Math.Max(Math.Min(255, interpolatedValue), 0) + 0.5);
                    }
                }
            }
    
            return result;
        }
        
        
        public static Image<Bgr, byte> ScaleBicubicColor(Image<Bgr, byte> image, double sx, double sy)
        {
            int width = image.Width;
            int height = image.Height;
    
            int newWidth = (int)(sx * width);
            int newHeight = (int)(sy * height);
    
            Image<Bgr, byte> result = new Image<Bgr, byte>(newWidth, newHeight);
            byte[,,] imageData = image.Data;
            byte[,,] resultData = result.Data;
    
            for (int y_prime = 0; y_prime < newHeight; y_prime++)
            {
                for (int x_prime = 0; x_prime < newWidth; x_prime++)
                {
                    double xc = x_prime / sx;
                    double yc = y_prime / sy;
            
                    int x0 = (int)Math.Floor(xc);
                    int y0 = (int)Math.Floor(yc);
            
                    if (x0 >= 1 && x0 < width - 2 && y0 >= 1 && y0 < height - 2)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            double[,] f = new double[4, 4];
                    
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    f[i, j] = imageData[y0 - 1 + i, x0 - 1 + j, channel];
                                }
                            }
                    
                            double interpolatedValue = InterpolateBicubic(xc, yc, f);
                            resultData[y_prime, x_prime, channel] = (byte)(Math.Max(Math.Min(255, interpolatedValue), 0) + 0.5);
                        }
                    }
                }
            }
    
            return result;
        }
        
    }
}
