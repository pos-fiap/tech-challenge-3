namespace BrokerService
{
    public record EmailDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}
