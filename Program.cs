using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PDFSplitterApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Set up dependency injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, config);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get services
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var textExtractor = serviceProvider.GetService<PDFTextExtractor>();
            var splitter = serviceProvider.GetService<PDFSplitter>();
            var classifier = serviceProvider.GetService<DocumentClassifier>();

            string inputPdfPath = "input.pdf";  // Update with your PDF file path
            string outputDirectory = "output_pdfs";  // Output directory for split PDFs

            // Extract text from PDF
            Console.WriteLine("Extracting text from PDF...");
            var pageTexts = await textExtractor.ExtractTextByPageAsync(inputPdfPath);

            // Split the PDF
            Console.WriteLine("Splitting the PDF...");
            List<SplitDocument> splitDocuments = splitter.SplitPdf(inputPdfPath, pageTexts);
            Console.WriteLine($"Total documents found: {splitDocuments.Count}");

            // Process and save each document
            Console.WriteLine("Processing documents...");
            await ProcessAndSaveDocuments(splitDocuments, inputPdfPath, outputDirectory, classifier, logger);
            Console.WriteLine("Processing complete.");
        }

        private static void ConfigureServices(ServiceCollection services, IConfiguration config)
        {
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Add configuration
            services.AddSingleton(config);

            // Add services
            services.AddSingleton<PDFTextExtractor>();
            services.AddSingleton<PDFSplitter>();
            services.AddSingleton<DocumentClassifier>();
        }

        private static async Task ProcessAndSaveDocuments(
            List<SplitDocument> documents,
            string inputPdfPath,
            string outputDir,
            DocumentClassifier classifier,
            ILogger logger)
        {
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            for (int i = 0; i < documents.Count; i++)
            {
                SplitDocument doc = documents[i];
                string documentText = doc.TextContent;

                // Classify the document
                string category = await classifier.ClassifyDocumentAsync(documentText);

                // Save the split PDF
                string outputPath = Path.Combine(outputDir, $"document_{i + 1}_{category}.pdf");
                PDFSplitter.SaveSplitPdf(doc.PageNumbers, inputPdfPath, outputPath);
                Console.WriteLine($"Saved {outputPath}");
            }
        }
    }
}
