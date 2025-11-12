using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using static Algorithms.Utilities.Utils;

namespace Algorithms.Sections
{
    public static class Filters
    {
        public static Image<Gray, byte> ApplyFilter(Image<Gray, byte> initialImage, double[,] filter)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(initialImage.Size);
            int h = filter.GetLength(0);
            int w = filter.GetLength(1);
            for (int y = h/2; y < initialImage.Height - h/2; ++y)
            {
                for (int x = w/2; x < initialImage.Width - w/2; ++x)
                {
                    double sumPond = 0.0;
                    for(int i = -h/2; i <= h/2; ++i)
                    {
                        for(int j = -w/2; j <= w/2; ++j)
                        {
                            sumPond += filter[i + h / 2, j + w / 2] * initialImage.Data[y + i, x + j, 0];
                        }
                    }
                    outputImage.Data[y, x, 0] = Algorithms.Utilities.Utils.Clip(sumPond);
                }
            }
            return outputImage;
        }

        public static double[] GaussMask(double sigma)
        {
            int l = (int)Math.Ceiling(4 * sigma);
            
            if (l % 2 == 0)
                l++;

            double[] mask = new double[l];
            int center = l / 2;
            double sum = 0;

            for (int i = 0; i < l; i++)
            {
                int z = i - center;
                double value = (1.0 / Math.Sqrt(2 * Math.PI * sigma)) * 
                               Math.Exp(-(z * z) / (2 * sigma * sigma));
                mask[i] = value;
                sum += value;
            }

            for (int i = 0; i < l; i++)
            {
                mask[i] /= sum;
            }

            return mask;
        }

        public static Image<Gray, byte> GaussFiltering(Image<Gray, byte> image, double sigmaX, double sigmaY)
        {
            double[] maskX = GaussMask(sigmaX);
            Image<Gray, byte> tempImage = ApplyFilterHorizontal(image, maskX);
            
            double[] maskY = GaussMask(sigmaY);
            Image<Gray, byte> resultImage = ApplyFilterVertical(tempImage, maskY);

            return resultImage;
        }

        public static Image<Bgr, byte> GaussFiltering(Image<Bgr, byte> image, double sigmaX, double sigmaY)
        {
            Image<Gray, byte>[] channels = image.Split();

            Image<Gray, byte> blueFiltered = GaussFiltering(channels[0], sigmaX, sigmaY);
            Image<Gray, byte> greenFiltered = GaussFiltering(channels[1], sigmaX, sigmaY);
            Image<Gray, byte> redFiltered = GaussFiltering(channels[2], sigmaX, sigmaY);

            Image<Bgr, byte> result = new Image<Bgr, byte>(image.Width, image.Height);
            VectorOfMat vm = new VectorOfMat(blueFiltered.Mat, greenFiltered.Mat, redFiltered.Mat);
            CvInvoke.Merge(vm, result);

            return result;
        }

        private static Image<Gray, byte> ApplyFilterHorizontal(Image<Gray, byte> image, double[] mask)
        {
            int width = image.Width;
            int height = image.Height;
            int maskSize = mask.Length;
            int offset = maskSize / 2;

            Image<Gray, byte> result = new Image<Gray, byte>(width, height);
            byte[,,] imageData = image.Data;
            byte[,,] resultData = result.Data;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0;

                    for (int k = 0; k < maskSize; k++)
                    {
                        int pixelX = x + k - offset;

                        if (pixelX < 0) pixelX = 0;
                        if (pixelX >= width) pixelX = width - 1;

                        sum += imageData[y, pixelX, 0] * mask[k];
                    }

                    resultData[y, x, 0] = Algorithms.Utilities.Utils.Clip(sum);
                }
            }

            return result;
        }

        private static Image<Gray, byte> ApplyFilterVertical(Image<Gray, byte> image, double[] mask)
        {
            int width = image.Width;
            int height = image.Height;
            int maskSize = mask.Length;
            int offset = maskSize / 2;

            Image<Gray, byte> result = new Image<Gray, byte>(width, height);
            byte[,,] imageData = image.Data;
            byte[,,] resultData = result.Data;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0;

                    for (int k = 0; k < maskSize; k++)
                    {
                        int pixelY = y + k - offset;

                        if (pixelY < 0) pixelY = 0;
                        if (pixelY >= height) pixelY = height - 1;

                        sum += imageData[pixelY, x, 0] * mask[k];
                    }

                    resultData[y, x, 0] = Algorithms.Utilities.Utils.Clip(sum);
                }
            }

            return result;
        }
        
        public static Image<Gray, byte> SobelDiagonalEdges(Image<Gray, byte> image, double threshold, double deviationDegrees)
{
    int width = image.Width;
    int height = image.Height;

    double[,] Sx = new double[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
    double[,] Sy = new double[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

    Image<Gray, byte> result = new Image<Gray, byte>(width, height);
    byte[,,] imageData = image.Data;
    byte[,,] resultData = result.Data;

    double targetAngle = 45.0 * Math.PI / 180.0;
    double deviationRadians = deviationDegrees * Math.PI / 180.0;

    for (int y = 1; y < height - 1; y++)
    {
        for (int x = 1; x < width - 1; x++)
        {
            double fx = 0;
            double fy = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    byte pixelValue = imageData[y + i, x + j, 0];
                    fx += Sx[i + 1, j + 1] * pixelValue;
                    fy += Sy[i + 1, j + 1] * pixelValue;
                }
            }

            double gradientNorm = Math.Sqrt(fx * fx + fy * fy);

            if (gradientNorm > threshold)
            {
                double theta = Math.Atan2(fy, fx);
                
                if (theta < 0)
                    theta += Math.PI;

                double angleDiff1 = Math.Abs(theta - targetAngle);
                double angleDiff2 = Math.Abs(theta - (targetAngle + Math.PI));

                double minAngleDiff = Math.Min(angleDiff1, angleDiff2);

                if (minAngleDiff <= deviationRadians)
                {
                    resultData[y, x, 0] = 255;
                }
                else
                {
                    resultData[y, x, 0] = 0;
                }
            }
            else
            {
                resultData[y, x, 0] = 0;
            }
        }
    }

    return result;
}
    }
}
