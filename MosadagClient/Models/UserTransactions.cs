using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

using System;

namespace MosadagClient.Models
{
    [Table("Mosadag.UserTransactions")]
    public class UserTransactions : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("data")]
        public string? Data { get; set; }

        [Column("hashedId")]
        public string HashedId { get; set; } = null!;
    }
}