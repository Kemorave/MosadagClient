using System;
using System.Collections.Generic;
using System.Text;

using Supabase.Gotrue;

namespace MosadagClient.Model
{
    public class LoginData
    {
        public Session?  Session { get; set; }
        public string? NopcommerceToken  { get; set; }
    }
}
