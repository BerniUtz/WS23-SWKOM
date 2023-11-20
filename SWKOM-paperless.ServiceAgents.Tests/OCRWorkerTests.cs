using SWKOM_paperless.ServiceAgents.Interfaces;
using System.IO;

namespace SWKOM_paperless.ServiceAgents.Tests
{
    public class Tests
    {
        private IOCRWorker worker;
        [SetUp]
        public void Setup()
        {
            worker = new OCRWorker();
        }

        [Test]
        public void checkOCRonPDF()
        {
            // Arrange
            var filePath = "./TEST.pdf";
            string text = "TEST123\n";

            // Act
            string result = null;
            using (var stream = File.OpenRead(filePath))
            {
                result = worker.OcrPdf(stream);
            }

            // Assert
            Assert.AreEqual(text, result);


        }
    }
}