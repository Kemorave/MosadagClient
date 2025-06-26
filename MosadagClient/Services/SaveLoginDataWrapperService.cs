using MosadagClient.Model;

using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace MosadagClient.Services
{
    internal class SaveLoginDataWrapperService : IGotrueSessionPersistence<Session>
    {

        public SaveLoginDataWrapperService(SaveLoginDataService service)
        {
            Service = service;
        }

        public SaveLoginDataService Service { get; }

        public void DestroySession()
        {
            Service.DestroySession();
        }
        public  Session? LoadSession()
        {
            return Service.LoadSession()?.Session;
        }
        public  void SaveSession(Session session)
        {
            var data = Service.LoadSession()?? new LoginData();
            data.Session = session;
            Service.SaveSession(data);
        }

    }
}
