using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PDFSplitterApp
{
    public class DocumentClassifier
    {
        private readonly OpenAIClient _openAiClient;
        private readonly string _deploymentName;
        private readonly ILogger<DocumentClassifier> _logger;
        private readonly List<string> _categories = new List<string>
        {
            "MedicalRecord",
            "PoliceReport",
            "EarningsEvidence",
            "MedicalBill",
            "PropertyDamageEvidence"
        };

        public DocumentClassifier(IConfiguration config, ILogger<DocumentClassifier> logger)
        {
            _logger = logger;

            string endpoint = config["AzureOpenAI:Endpoint"] ?? throw new Exception("Azure OpenAI endpoint is missing.");
            string apiKey = config["AzureOpenAI:ApiKey"] ?? throw new Exception("Azure OpenAI API key is missing.");
            _deploymentName = config["AzureOpenAI:DeploymentName"] ?? throw new Exception("Deployment name is missing.");

            var credential = new AzureKeyCredential(apiKey);
            _openAiClient = new OpenAIClient(new Uri(endpoint), credential);
        }

        public async Task<string> ClassifyDocumentAsync(string textContent)
        {
            string prompt = $@"
You are an assistant that classifies documents into one of the following categories:
- MedicalRecord
- PoliceReport
- EarningsEvidence
- MedicalBill
- PropertyDamageEvidence

Read the document text below and provide the most appropriate category from the list above. Respond only with the category name.

Document Text:
{textContent}
";

            try
            {
                var chatOptions = new ChatCompletionsOptions
                {
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, "You are an intelligent assistant."),
                        new ChatMessage(ChatRole.User, prompt)
                    },
                    MaxTokens = 10,
                    Temperature = 0.0f,
                    StopSequences = { "\n" }
                };

                Response<ChatCompletions> response = await _openAiClient.GetChatCompletionsAsync(_deploymentName, chatOptions);
                string category = response.Value.Choices[0].Message.Content.Trim();

                if (_categories.Contains(category))
                {
                    return category;
                }
                else
                {
                    _logger.LogWarning($"Unexpected category returned: {category}");
                    return "Uncategorized";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during document classification.");
                return "Uncategorized";
            }
        }
    }
}