using TechChallenge.Application.BaseResponse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/send", (EmailDto email) =>
{
    BaseOutput<EmailDto> response = new()
    {
        IsSuccessful = true,
        Response = email
    };



    return Results.Ok(response);
})
.WithName("Email")
.Produces<BaseOutput<EmailDto>>(StatusCodes.Status200OK)
.WithTags("Emails")
.WithOpenApi();

app.Run();

internal record EmailDto
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}