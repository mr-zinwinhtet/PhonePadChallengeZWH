using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PhonePad; 

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow our Vite frontend to communicate with this API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();

// Define our single POST endpoint
app.MapPost("/api/decode", (DecodeRequest request) =>
{
    var output = Keypad.OldPhonePad(request.Input);
    return Results.Ok(new { Output = output });
});

app.Run();

// Data Transfer Object
public class DecodeRequest
{
    public string Input { get; set; } = string.Empty;
}