using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        private static Image<Gray, byte> BinarizeImage(Image<Gray, byte> inputImage, byte threshold)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, x, 0] > threshold ? (byte)255 : (byte)0;
                }
            }
            
            return result;
        }

        public static Image<Gray, byte> Dilation(
            Image<Gray, byte> inputImage, 
            int h, 
            int w, 
            byte threshold, 
            int whiteObjects = 1)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> binaryImage = BinarizeImage(inputImage, threshold);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            
            int halfH = h / 2;
            int halfW = w / 2;
            
            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool found = false;
                    
                    for (int i = -halfH; i <= halfH && !found; i++)
                    {
                        for (int j = -halfW; j <= halfW && !found; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;
                            
                            if (ny >= 0 && ny < binaryImage.Height && 
                                nx >= 0 && nx < binaryImage.Width)
                            {
                                byte pixelValue = binaryImage.Data[ny, nx, 0];
                                
                                if (whiteObjects == 1)
                                {
                                    if (pixelValue == 255)
                                    {
                                        found = true;
                                    }
                                }
                                else
                                {
                                    if (pixelValue == 0)
                                    {
                                        found = true;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (whiteObjects == 1)
                    {
                        result.Data[y, x, 0] = found ? (byte)255 : (byte)0;
                    }
                    else
                    {
                        result.Data[y, x, 0] = found ? (byte)0 : (byte)255;
                    }
                }
            }
            
            return result;
        }

        public static Image<Gray, byte> Erosion(
            Image<Gray, byte> inputImage, 
            int h, 
            int w, 
            byte threshold, 
            int whiteObjects = 1)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> binaryImage = BinarizeImage(inputImage, threshold);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            
            int halfH = h / 2;
            int halfW = w / 2;
            
            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool foundOpposite = false;
                    
                    for (int i = -halfH; i <= halfH && !foundOpposite; i++)
                    {
                        for (int j = -halfW; j <= halfW && !foundOpposite; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;
                            
                            if (ny >= 0 && ny < binaryImage.Height && 
                                nx >= 0 && nx < binaryImage.Width)
                            {
                                byte pixelValue = binaryImage.Data[ny, nx, 0];
                                
                                if (whiteObjects == 1)
                                {
                                    if (pixelValue == 0)
                                    {
                                        foundOpposite = true;
                                    }
                                }
                                else
                                {
                                    if (pixelValue == 255)
                                    {
                                        foundOpposite = true;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (whiteObjects == 1)
                    {
                        result.Data[y, x, 0] = foundOpposite ? (byte)0 : (byte)255;
                    }
                    else
                    {
                        result.Data[y, x, 0] = foundOpposite ? (byte)255 : (byte)0;
                    }
                }
            }
            
            return result;
        }

        private static Image<Gray, byte> ErosionOnBinary(
            Image<Gray, byte> binaryImage, 
            int h, 
            int w, 
            int whiteObjects)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(binaryImage.Width, binaryImage.Height);
            int halfH = h / 2;
            int halfW = w / 2;
            
            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool foundOpposite = false;
                    
                    for (int i = -halfH; i <= halfH && !foundOpposite; i++)
                    {
                        for (int j = -halfW; j <= halfW && !foundOpposite; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;
                            
                            if (ny >= 0 && ny < binaryImage.Height && 
                                nx >= 0 && nx < binaryImage.Width)
                            {
                                byte pixelValue = binaryImage.Data[ny, nx, 0];
                                
                                if (whiteObjects == 1)
                                {
                                    if (pixelValue == 0) foundOpposite = true;
                                }
                                else
                                {
                                    if (pixelValue == 255) foundOpposite = true;
                                }
                            }
                        }
                    }
                    
                    if (whiteObjects == 1)
                    {
                        result.Data[y, x, 0] = foundOpposite ? (byte)0 : (byte)255;
                    }
                    else
                    {
                        result.Data[y, x, 0] = foundOpposite ? (byte)255 : (byte)0;
                    }
                }
            }
            
            return result;
        }

        private static Image<Gray, byte> DilationOnBinary(
            Image<Gray, byte> binaryImage, 
            int h, 
            int w, 
            int whiteObjects)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(binaryImage.Width, binaryImage.Height);
            int halfH = h / 2;
            int halfW = w / 2;
            
            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool found = false;
                    
                    for (int i = -halfH; i <= halfH && !found; i++)
                    {
                        for (int j = -halfW; j <= halfW && !found; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;
                            
                            if (ny >= 0 && ny < binaryImage.Height && 
                                nx >= 0 && nx < binaryImage.Width)
                            {
                                byte pixelValue = binaryImage.Data[ny, nx, 0];
                                
                                if (whiteObjects == 1)
                                {
                                    if (pixelValue == 255) found = true;
                                }
                                else
                                {
                                    if (pixelValue == 0) found = true;
                                }
                            }
                        }
                    }
                    
                    if (whiteObjects == 1)
                    {
                        result.Data[y, x, 0] = found ? (byte)255 : (byte)0;
                    }
                    else
                    {
                        result.Data[y, x, 0] = found ? (byte)0 : (byte)255;
                    }
                }
            }
            
            return result;
        }

        public static Image<Gray, byte> Opening(
            Image<Gray, byte> inputImage, 
            int h, 
            int w, 
            byte threshold, 
            int whiteObjects = 1)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> binaryImage = BinarizeImage(inputImage, threshold);
            Image<Gray, byte> erodedImage = ErosionOnBinary(binaryImage, h, w, whiteObjects);
            Image<Gray, byte> result = DilationOnBinary(erodedImage, h, w, whiteObjects);
            
            return result;
        }

        public static Image<Gray, byte> Closing(
            Image<Gray, byte> inputImage, 
            int h, 
            int w, 
            byte threshold, 
            int whiteObjects = 1)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> binaryImage = BinarizeImage(inputImage, threshold);
            Image<Gray, byte> dilatedImage = DilationOnBinary(binaryImage, h, w, whiteObjects);
            Image<Gray, byte> result = ErosionOnBinary(dilatedImage, h, w, whiteObjects);
            
            return result;
        }
         public static Image<Gray, byte> DilationGrayscale(Image<Gray, byte> inputImage, int h, int w)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            int halfH = h / 2;
            int halfW = w / 2;
            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte maxValue = 0;
                    
                    for (int i = -halfH; i <= halfH; i++)
                    {
                        for (int j = -halfW; j <= halfW; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;
                            
                            if (ny >= 0 && ny < inputImage.Height && 
                                nx >= 0 && nx < inputImage.Width)
                            {
                                byte pixelValue = inputImage.Data[ny, nx, 0];
                                if (pixelValue > maxValue)
                                {
                                    maxValue = pixelValue;
                                }
                            }
                        }
                    }
                    
                    result.Data[y, x, 0] = maxValue;
                }
            }
            
            return result;
        }

        public static Image<Gray, byte> ErosionGrayscale(Image<Gray, byte> inputImage, int h, int w)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            int halfH = h / 2;
            int halfW = w / 2;
            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte minValue = 255;
                    
                    for (int i = -halfH; i <= halfH; i++)
                    {
                        for (int j = -halfW; j <= halfW; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;
                            
                            if (ny >= 0 && ny < inputImage.Height && 
                                nx >= 0 && nx < inputImage.Width)
                            {
                                byte pixelValue = inputImage.Data[ny, nx, 0];
                                if (pixelValue < minValue)
                                {
                                    minValue = pixelValue;
                                }
                            }
                        }
                    }
                    
                    result.Data[y, x, 0] = minValue;
                }
            }
            
            return result;
        }

        public static Image<Gray, byte> MorphologicalGradient(Image<Gray, byte> inputImage, int h, int w)
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage));
            
            if (h <= 0 || w <= 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie pozitive");
            
            if (h % 2 == 0 || w % 2 == 0)
                throw new ArgumentException("Dimensiunile măștii trebuie să fie impare");

            Image<Gray, byte> dilated = DilationGrayscale(inputImage, h, w);
            Image<Gray, byte> eroded = ErosionGrayscale(inputImage, h, w);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);
            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int diff = dilated.Data[y, x, 0] - eroded.Data[y, x, 0];
                    result.Data[y, x, 0] = (byte)Math.Max(0, Math.Min(255, diff));
                }
            }
            
            return result;
        }
    }
}
