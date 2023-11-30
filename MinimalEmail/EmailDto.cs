namespace MinimalEmail
{
    public record EmailDto
    {
        public string To { get; set; } = "ricardomacieldasilva@hotmail.com";
        public string Subject { get; set; } = "Teste de envio de email";
        public string Body { get; set; } = "Teste de envio de email Body";

    }
}
