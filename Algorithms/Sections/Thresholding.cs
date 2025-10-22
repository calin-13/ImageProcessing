using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        public static byte IntermeansThreshold(Image<Gray, byte> inputImage)
        {
            if (inputImage == null) throw new ArgumentNullException(nameof(inputImage));
            
            int[] hist = new int[256];
            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                    hist[inputImage.Data[y, x, 0]]++;
            
            int min = 0, max = 255;
            while (min < 256 && hist[min] == 0) min++;
            while (max >= 0 && hist[max] == 0) max--;
            if (min >= max) return (byte)min;
            
            double tPrev = 0;
            long n = 0, sum = 0;
            for (int k = min; k <= max; k++) { n += hist[k]; sum += (long)k * hist[k]; }
            if (n > 0) tPrev = sum / (double)n; else tPrev = (min + max) * 0.5;
            
            for (int it = 0; it < 100; it++)
            {
                int tFloor = (int)Math.Floor(tPrev);

                long c1 = 0, s1 = 0, c2 = 0, s2 = 0;
                for (int k = 0; k <= Math.Min(255, tFloor); k++) { c1 += hist[k]; s1 += (long)k * hist[k]; }
                for (int k = tFloor + 1; k <= 255; k++)       { c2 += hist[k]; s2 += (long)k * hist[k]; }

                double mu1 = (c1 == 0) ? 0.0 : s1 / (double)c1;
                double mu2 = (c2 == 0) ? 0.0 : s2 / (double)c2;
                
                double tNew = (c1 == 0) ? mu2 : (c2 == 0) ? mu1 : 0.5 * (mu1 + mu2);

                if (Math.Abs(tNew - tPrev) < 0.5) { tPrev = tNew; break; }
                tPrev = tNew;
            }

            int t = (int)Math.Round(tPrev);
            return (byte)Math.Max(0, Math.Min(255, t));
        }
    }
}
