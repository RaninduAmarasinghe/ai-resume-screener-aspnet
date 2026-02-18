using Microsoft.AspNetCore.Http;
using AiResumeScreenerNet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<PdfService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/resume/analyze",
        (IFormFile file, string jobDescription, PdfService pdfService) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("Resume file is required");

            if (string.IsNullOrWhiteSpace(jobDescription))
                return Results.BadRequest("Job description is required");

            using var stream = file.OpenReadStream();
            var resumeText = pdfService.ExtractText(stream);

            var resumeWords = resumeText.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var jdWords = jobDescription.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var matchedWords = jdWords.Intersect(resumeWords).Count();

            var matchPercentage = jdWords.Length == 0
                ? 0
                : (double)matchedWords / jdWords.Length * 100;

            return Results.Ok(new
            {
                FileName = file.FileName,
                MatchPercentage = Math.Round(matchPercentage, 2),
                MatchedKeywords = matchedWords
            });
        })
    .DisableAntiforgery();

app.Run();