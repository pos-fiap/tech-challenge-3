using Microsoft.AspNetCore.Mvc;
using MinimalEmail;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TechChallenge.Application.BaseResponse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/send", ([FromBody] EmailDto email) =>
{
    BaseOutput<EmailDto> response = new()
    {
        IsSuccessful = true,
        Response = email
    };

    try
    {
        RabbitMQSender rabbitMQSender = new();
        rabbitMQSender.SendEmail(email);
    }
    catch (Exception ex)
    {
        response.IsSuccessful = false;
        response.AddError(ex.Message);
    }

    return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
})
.WithName("Email")
.Produces<BaseOutput<EmailDto>>(StatusCodes.Status200OK)
.Produces<BaseOutput<EmailDto>>(StatusCodes.Status400BadRequest)
.WithTags("Emails")
.WithOpenApi();

app.Run();

