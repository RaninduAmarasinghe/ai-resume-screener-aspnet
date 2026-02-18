using UglyToad.PdfPig;

namespace AiResumeScreenerNet.Services;

public class PdfService
{
    public string ExtractText(Stream stream)
    {
        using var document = PdfDocument.Open(stream);

        var text = "";

        foreach (var page in document.GetPages())
        {
            text += page.Text;
        }

        return text;
    }
}