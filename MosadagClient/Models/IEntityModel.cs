using System;
using System.Collections.Generic;
using System.Text;

namespace MosadagClient.Models
{
    public interface IEntityModel
    {
        long Id { get; set; }
    }
    public interface ISyncedEntityModel
    {
        long Id { get; set; }
        bool IsSynced { get; set; }
    }
}
