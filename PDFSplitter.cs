using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDFSplitterApp
{
    public class PDFSplitter
    {
        public List<SplitDocument> SplitPdf(string pdfPath, List<string> pageTexts)
        {
            List<SplitDocument> documents = new List<SplitDocument>();
            List<int> currentPages = new List<int>();
            string currentText = "";

            int totalPages = pageTexts.Count;

            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
            {
                string text = pageTexts[pageIndex];

                if (IsBlankPage(text) || IsTitlePage(text))
                {
                    if (currentPages.Count > 0)
                    {
                        documents.Add(new SplitDocument
                        {
                            PageNumbers = new List<int>(currentPages),
                            TextContent = currentText
                        });
                        currentPages.Clear();
                        currentText = "";
                    }
                    // Skip adding blank or title pages to the current document
                    if (!IsBlankPage(text))
                    {
                        currentPages.Add(pageIndex);
                        currentText += text + "\n";
                    }
                }
                else
                {
                    currentPages.Add(pageIndex);
                    currentText += text + "\n";
                }
            }

            // Add the last document if any pages are left
            if (currentPages.Count > 0)
            {
                documents.Add(new SplitDocument
                {
                    PageNumbers = new List<int>(currentPages),
                    TextContent = currentText
                });
            }

            return documents;
        }

        public static void SaveSplitPdf(List<int> pageNumbers, string inputPdfPath, string outputPath)
        {
            using PdfDocument inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import);
            using PdfDocument outputPdf = new PdfDocument();

            foreach (int pageNum in pageNumbers)
            {
                PdfPage page = inputPdf.Pages[pageNum];
                outputPdf.AddPage(page);
            }
            outputPdf.Save(outputPath);
        }

        private bool IsBlankPage(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        private bool IsTitlePage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            List<string> keywords = new List<string>
            {
                "Exhibit",
                "Medical Record",
                "Police Report",
                "Earnings Evidence",
                "Medical Bill",
                "Property Damage Evidence"
            };

            foreach (var keyword in keywords)
            {
                if (text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }
    }

    public class SplitDocument
    {
        public List<int> PageNumbers { get; set; }
        public string TextContent { get; set; }
    }
}