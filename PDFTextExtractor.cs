using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PDFSplitterApp
{
    public class PDFTextExtractor
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly DocumentAnalysisClient _client;
        private readonly ILogger<PDFTextExtractor> _logger;

        public PDFTextExtractor(IConfiguration config, ILogger<PDFTextExtractor> logger)
        {
            _logger = logger;
            _endpoint = config["FormRecognizer:Endpoint"] ?? throw new Exception("FormRecognizer endpoint is missing.");
            _apiKey = config["FormRecognizer:ApiKey"] ?? throw new Exception("FormRecognizer API key is missing.");

            var credential = new AzureKeyCredential(_apiKey);
            _client = new DocumentAnalysisClient(new Uri(_endpoint), credential);
        }

        public async Task<List<string>> ExtractTextByPageAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", stream);

            if (!operation.HasCompleted) throw new Exception("Text extraction failed.");
            var result = operation.Value;

            List<string> pageTexts = new List<string>();
            foreach (var page in result.Pages)
            {
                List<string> lines = new List<string>();
                foreach (var line in page.Lines)
                {
                    lines.Add(line.Content);
                }
                pageTexts.Add(string.Join(" ", lines));
            }

            _logger.LogInformation("Text extraction completed successfully.");
            return pageTexts;
        }
    }
}
