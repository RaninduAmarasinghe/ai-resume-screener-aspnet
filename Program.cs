using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/resume/upload", (IFormFile file) =>
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("File is required");

        using var stream = file.OpenReadStream();
        using var document = PdfDocument.Open(stream);

        var text = "";

        foreach (var page in document.GetPages())
        {
            text += page.Text;
        }

        return Results.Ok(new
        {
            FileName = file.FileName,
            ExtractedTextLength = text.Length
        });
    })
    .DisableAntiforgery();

app.Run();