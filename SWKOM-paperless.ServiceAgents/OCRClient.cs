using ImageMagick;
using SWKOM_paperless.ServiceAgents.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace SWKOM_paperless.ServiceAgents
{
    public class OCRClient : IOCRClient
    {
        private readonly string language;
        private readonly string tessDataPath;
        public OCRClient(string lan = "eng")
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            path = Path.Combine(path, "tessdata");
            this.tessDataPath = path.Replace("file:\\", "");
            Console.WriteLine(this.tessDataPath);
            this.language = lan;
        }
        public string OcrPdf(Stream pdf)
        {
            var stringBuilder = new StringBuilder();

            using (var magickImages = new MagickImageCollection())
            {
                magickImages.Read(pdf);
                foreach (var magickImage in magickImages)
                {
                    // Set the resolution and format of the image (adjust as needed)
                    magickImage.Density = new Density(300, 300);
                    magickImage.Format = MagickFormat.Png;

                    // Perform OCR on the image
                    using (var tesseractEngine = new TesseractEngine(this.tessDataPath, this.language, EngineMode.Default))
                    {
                        using (var page = tesseractEngine.Process(Pix.LoadFromMemory(magickImage.ToByteArray())))
                        {
                            var extractedText = page.GetText();
                            stringBuilder.Append(extractedText);
                        }
                    }
                }
            }


            return stringBuilder.ToString();

        }
    }
}
