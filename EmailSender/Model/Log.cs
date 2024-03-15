using System.ComponentModel.DataAnnotations.Schema;

namespace EmailSender.Model
{
    [Table("log")]
    public class Log
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("content")]
        public string Content { get; set; } = string.Empty;
    }
}
