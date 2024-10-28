# Splitify

## PDF Splitter and Categorizer

This application is a C# console program designed to process PDF files by splitting them into separate documents whenever it encounters a blank page or a title page. It leverages Azure Document Intelligence to extract text from each page, including OCR capabilities for scanned documents, and uses Azure OpenAI's GPT models to categorize each split document based on its content.

## Features

- **PDF Splitting**: Automatically splits a PDF file into multiple documents based on blank pages or title pages detected within the file.
- **Text Extraction**: Utilizes Azure Document Intelligence (Form Recognizer) to extract text from each page, including support for scanned images via OCR.
- **Document Categorization**: Employs Azure OpenAI's GPT models to categorize each split document into predefined categories based on its content.
- **Automated Processing**: Streamlines the workflow by processing and saving each document automatically with appropriate naming conventions.
- **Customization**: Allows customization of title page keywords and document categories to suit specific needs.

## Prerequisites

- **Azure Subscription**: An active Azure account with access to:
  - **Azure Document Intelligence (Form Recognizer)**
  - **Azure OpenAI Service**
- **.NET SDK**: .NET 6.0 SDK or higher installed on your machine.
- **Azure Resources**:
  - **Form Recognizer**: Obtain the Endpoint URL and API Key.
  - **Azure OpenAI**: Obtain the Endpoint URL, API Key, and Deployment Name for your GPT model.
- **NuGet Packages**: The following packages are required (included in the project file):
  - `Azure.AI.FormRecognizer`
  - `Azure.AI.OpenAI`
  - `PdfSharp`
  - `Microsoft.Extensions.Configuration`
  - `Microsoft.Extensions.Logging`
  - `Microsoft.Extensions.Configuration.Json`
  - `Microsoft.Extensions.DependencyInjection`

## Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/JervisAnthony/Splitify.git
   cd Splitify
   ```

2. **Install .NET SDK**

   Ensure that you have the .NET 6.0 SDK or higher installed. You can download it from the [official website](https://dotnet.microsoft.com/download).

3. **Restore Dependencies**

   ```bash
   dotnet restore
   ```

4. **Set Up Configuration**

   - **appsettings.json**

     Create an `appsettings.json` file in the project directory with the following content:

     ```json
     {
       "FormRecognizer": {
         "Endpoint": "https://<your-form-recognizer-endpoint>.cognitiveservices.azure.com/",
         "ApiKey": "<your-form-recognizer-api-key>"
       },
       "AzureOpenAI": {
         "Endpoint": "https://<your-openai-endpoint>.openai.azure.com/",
         "ApiKey": "<your-openai-api-key>",
         "DeploymentName": "<your-deployment-name>"
       }
     }
     ```

     **Important:** Replace the placeholders with your actual Azure service credentials.

   - **Secure Your Credentials**

     Do not commit `appsettings.json` with your credentials to any public repository. Consider using environment variables or a secrets manager for sensitive information.

5. **Place the Input PDF**

   - Copy the PDF file you wish to process into the project directory.
   - Update the `inputPdfPath` variable in `Program.cs` if your file has a different name or location.

## Usage

1. **Build the Application**

   ```bash
   dotnet build
   ```

2. **Run the Application**

   ```bash
   dotnet run
   ```

3. **Output**

   - The application will process the input PDF and create an `output_pdfs` directory (if it doesn't already exist).
   - Split and categorized PDF files will be saved in this directory with filenames like `document_1_MedicalRecord.pdf`.

## Configuration Details

- **Categories**

  The application categorizes documents into the following predefined types:

  - `MedicalRecord`
  - `PoliceReport`
  - `EarningsEvidence`
  - `MedicalBill`
  - `PropertyDamageEvidence`

  These categories are defined in `DocumentClassifier.cs` and can be customized as needed.

- **Title Page Detection**

  Title pages are detected based on the presence of specific keywords within the page text. The default keywords are:

  - "Exhibit"
  - "Medical Record"
  - "Police Report"
  - "Earnings Evidence"
  - "Medical Bill"
  - "Property Damage Evidence"

  You can modify these keywords in the `IsTitlePage` method within `PDFSplitter.cs`.

## Customization

- **Adding New Categories**

  To add new document categories:

  - Update the `_categories` list in `DocumentClassifier.cs`.
  - Adjust the prompt in the `ClassifyDocumentAsync` method to include the new categories.

- **Modifying Title Page Keywords**

  - Edit the `keywords` list in the `IsTitlePage` method in `PDFSplitter.cs` to include any additional keywords relevant to your documents.

- **Logging Levels**

  - Adjust the logging level in `Program.cs` under the `ConfigureServices` method to control the verbosity of logs.

    ```csharp
    builder.SetMinimumLevel(LogLevel.Information);
    ```

## Security Considerations

- **API Keys and Sensitive Data**

  - **Never commit API keys or sensitive data** to version control systems.
  - Use environment variables or secure secrets management solutions like Azure Key Vault.

- **Exception Handling**

  - The application includes basic error handling. For production use, enhance exception handling to cover all potential issues.

- **Data Privacy**

  - Ensure compliance with data protection regulations when processing sensitive documents.

## Troubleshooting

- **Common Errors**

  - **Authentication Errors**: Verify that your Azure credentials are correct and that your services are properly configured.
  - **File Not Found**: Ensure that the `inputPdfPath` is correct and the file exists.
  - **Permission Issues**: Run the application with sufficient permissions to read and write files in the project directory.

- **Logging**

  - Check the console output for logs that may help identify issues.
  - Increase the logging level to `LogLevel.Debug` for more detailed logs.


## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the project.
2. Create your feature branch: `git checkout -b feature/YourFeature`.
3. Commit your changes: `git commit -m 'Add YourFeature'`.
4. Push to the branch: `git push origin feature/YourFeature`.
5. Open a pull request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📧 Contact

If you have any questions or suggestions, feel free to contact me:

- **Email**: jervisaldanha.collabs@gmail.com
- **GitHub**: [JervisAnthony](https://github.com/JervisAnthony) 
