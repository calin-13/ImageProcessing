using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System;
using System.Drawing;

namespace Algorithms.Sections
{
    public class Segmentation
    {
        public static Image<Bgr, byte> HarrisCornerDetection(Image<Bgr, byte> img, double sigma, double threshold,
            double alpha)
        {
            Image<Gray, byte> gray = img.Convert<Gray, byte>();
            int width = img.Width;
            int height = img.Height;

            Image<Gray, byte> smoothed = Filters.GaussFiltering(gray, 1.0, 1.0);
            
            Image<Gray, float>  fx = new Image<Gray, float>(width, height);
            Image<Gray, float>  fy = new Image<Gray, float>(width, height);
            CvInvoke.Sobel(smoothed, fx, DepthType.Cv32F, 1, 0, 3);
            CvInvoke.Sobel(smoothed, fy, DepthType.Cv32F, 1, 0, 3);
            
            Image<Gray, float>  fx2 = new Image<Gray, float>(width, height);
            Image<Gray, float>  fy2 = new Image<Gray, float>(width, height);
            Image<Gray, float>  fxy = new Image<Gray, float>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float fxVal = fx.Data[y, x, 0];
                    float fyVal = fy.Data[y, x, 0];
                    fx2.Data[y, x, 0] = fxVal * fxVal;
                    fy2.Data[y, x, 0] = fyVal * fyVal;
                    fxy.Data[y, x, 0] = fxVal * fyVal;
                }
            }
            Image<Gray, float> Gx2 = new Image<Gray, float>(width, height);
            Image<Gray, float> Gy2 = new Image<Gray, float>(width, height);
            Image<Gray, float> Gxy = new Image<Gray, float>(width, height);

            int kernelSize = (int)(6 * sigma + 1);
            if (kernelSize % 2 == 0)
                kernelSize++;
            
            CvInvoke.GaussianBlur(fx2, Gx2, new Size(kernelSize, kernelSize), sigma);
            CvInvoke.GaussianBlur(fy2, Gy2, new Size(kernelSize, kernelSize), sigma);
            CvInvoke.GaussianBlur(fxy, Gxy, new Size(kernelSize, kernelSize), sigma);

            double[,] CRF = new double[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double gx2 = Gx2.Data[y, x, 0];
                    double gy2 = Gy2.Data[y, x, 0];
                    double gxy = Gxy.Data[y, x, 0];

                    double det = gx2 * gy2 - gxy * gxy;
                    double trace = gx2 + gy2;
                    CRF[y, x] = det - alpha * trace * trace;
                }
            }

            Image<Bgr, byte> result = img.Convert<Bgr, byte>();
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    
                    if (CRF[y, x] > threshold)
                    {
                        bool isMax = true;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                if (CRF[y + dy, x + dx] > CRF[y, x])
                                {
                                    isMax = false;
                                    break;
                                }
                            }

                            if (!isMax) break;
                        }

                        if (isMax)
                        {
                            CvInvoke.Circle(result, new System.Drawing.Point(x, y), 3, new MCvScalar(0, 0, 255), 2);
                        }

                    }
                }
            }
            return result;
        }
        public static Image<Bgr, byte> HarrisCornerDetection(Image<Gray, byte> image, double sigma, double threshold, double alpha)
        {
            Image<Bgr, byte> colorImage = image.Convert<Bgr, byte>();
            return HarrisCornerDetection(colorImage, sigma, threshold, alpha);
        }
    }
}
