using BrokerService;
using Microsoft.AspNetCore.Mvc;
using MinimalEmail.BaseResponse;

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
        RabbitMQService rabbitMQSender = new("RabbitMQ .NET 8 Sender App");
        rabbitMQSender.SendEmailToQueue(email);
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

