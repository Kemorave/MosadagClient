using System;
using System.Collections.Generic;
using System.Text;

using MosadagClient.Model;

using Supabase.Gotrue.Interfaces;

namespace MosadagClient.Services
{
    public abstract class SaveLoginDataService
    {
        protected SaveLoginDataService()
        {
        }

        public abstract void DestroySession();
        public abstract LoginData? LoadSession();
        public abstract void SaveSession(LoginData session);
   
    }
}
